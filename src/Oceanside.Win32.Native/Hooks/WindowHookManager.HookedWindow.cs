// MIT License
//
// Copyright (C) 2020 Oceanside Software Corporation (R)  Prosper, TX
// Author  : Jason T. Brower
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

namespace WinUI.Native.Hooks
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices;
    using static PInvoke.User32;

    public sealed partial class WindowHookManager
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   When hooking into a Windows WndProc, it is tracked through this class. </summary>
        ///
        /// <seealso cref="System.IDisposable"/>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private class HookedWindow
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// When the WM_DESTROY message is received by the WndProc, this event is called.  When this
            /// event fires the associated Window has been closed but any children could still be in the
            /// process of closing.
            /// </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            internal event EventHandler? OnClosed;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Note that when sub-classing using an approach that existed before ComCtl32.dll version 6, the
            /// Window procedure can only be replaced once.  For that reason, if the caller is done with
            /// their custom procedure, they can deactivate it so none of the callbacks associated with the
            /// Window Handle will be called.  However, if another callback is registered, we need to keep
            /// our custom internal wndproc hooked so that we can service the callers newly registered
            /// callback.
            /// </summary>
            ///
            /// <value> True if this  is deactivated, false if not. </value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            private bool _isDeActivated;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// When IsDestroyed is true, the window handle associated with the custom wndproc is either
            /// closed or is closing.  At this point we mark IsDeactived to true but also re-install the
            /// original hook as the window is going down.
            /// </summary>
            ///
            /// <value> True if this  is destroyed, false if not. </value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            private bool _isDestroyed;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Window procedure delegate. </summary>
            ///
            /// <param name="hWnd">     The window. </param>
            /// <param name="msg">      The message. </param>
            /// <param name="wParam">   The parameter. </param>
            /// <param name="lParam">   The parameter. </param>
            ///
            /// <returns>   An IntPtr. </returns>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            private delegate IntPtr WndProcDelegate(IntPtr hWnd, HookMsgType msg, IntPtr wParam, IntPtr lParam);

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   The hooked window procedure. </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            private readonly WndProcDelegate _delegateOfCustomWndProc;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   The hooked window procedure pointer. </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            private readonly IntPtr _hookedWndProcPtr;
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   The original window procedure. </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            private readonly IntPtr _originalWndProc;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Gets the handle of the window. </summary>
            ///
            /// <value> The window handle. </value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            internal IntPtr WindowHandle { get; }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   The callbacks associated with this particular window. </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            internal IList<WndProcCallback> RegisteredWndProcCallbacks { get; } = new List<WndProcCallback>();

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Synchronize access to the Callbacks collection. </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            private readonly object _callbacksLocker = new object();

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Initializes a new instance of the WinUI.Native.HookedWindow class. </summary>
            ///
            /// <param name="handle">   The handle. </param>
            /// <param name="callback"> The callback. </param>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            internal HookedWindow(IntPtr handle, WndProcCallback callback)
            {
                lock (_callbacksLocker)
                {
                    RegisteredWndProcCallbacks.Add(callback);
                    _isDeActivated = false;
                }
                WindowHandle = handle;

                //Get a native pointer to the delegate
                _delegateOfCustomWndProc = CustomWndProc;

                //Grab a pointer to the delegate that will be called when we sub-class W
                _hookedWndProcPtr = Marshal.GetFunctionPointerForDelegate(_delegateOfCustomWndProc);

                //I am not entirely sure if this call is absolutely necessary because _hookedWndProcPtr is
                // scoped to this class rather than local.  The idea here is to prevent the GC from cleaning
                // up the Thunk that is returned by asking for a pointer to the delegate.  Worst case 
                // scenario (I believe) is that I am calling this line when it is not needed and since the call
                // only protects it from the point it is called until the first time it is used below, this 
                // should be a non-issue.  Yet I never like feeling absolutely certain about calling into 
                // NativeCode.  Don't worry as much as I do, but keep it in mind.
                GC.KeepAlive(_hookedWndProcPtr);

                //Install the hook.  Note that SetWindowLongPtr is  compatible with both 32-bit and 64-bit 
                // calling code.
                _originalWndProc = SetWindowLongPtr(WindowHandle,
                    WindowLongIndexFlags.GWL_WNDPROC, _hookedWndProcPtr);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Callback, called when the attach. </summary>
            ///
            /// <exception cref="InvalidOperationException">    Thrown when the requested operation is
            ///                                                 invalid. </exception>
            ///
            /// <param name="callback"> The callback. </param>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            internal void AttachCallback(WndProcCallback callback)
            {
                if (_isDestroyed)
                    throw new InvalidOperationException($"{nameof(AttachCallback)} called on a hook where {nameof(_isDestroyed)} is true.");

                lock (_callbacksLocker)
                {
                    if (!RegisteredWndProcCallbacks.Contains(callback))
                    {
                        RegisteredWndProcCallbacks.Add(callback);
                        _isDeActivated = false;
                        return;
                    }
                }
                Debug.WriteLine(
                    $"An attempt was made to add a duplicate {nameof(WndProcCallback)} to a Window with the handle = {WindowHandle}.  Duplicate was skipped.");
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Callback, called when the detach. </summary>
            ///
            /// <param name="callback"> The callback. </param>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            internal void DetachCallback(WndProcCallback callback)
            {
                //Not throwing an exception if _isDeActivated is true 
                if (_isDeActivated) return;
                lock (_callbacksLocker)
                {
                    if (RegisteredWndProcCallbacks.Contains(callback))
                    {
                        RegisteredWndProcCallbacks.Remove(callback);
                        _isDeActivated = !RegisteredWndProcCallbacks.Any();
                        return;
                    }
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Custom window procedure. </summary>
            ///
            /// <param name="hWnd">     The window. </param>
            /// <param name="msg">      The message. </param>
            /// <param name="wParam">   The parameter. </param>
            /// <param name="lParam">   The parameter. </param>
            ///
            /// <returns>   An IntPtr. </returns>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            private IntPtr CustomWndProc(IntPtr hWnd, HookMsgType msg, IntPtr wParam, IntPtr lParam)
            {
                var m = new HookMessage(hWnd, msg, wParam, lParam);

                //Deactivated means there are zero callbacks and destroyed means that the WM_DESTROY message
                // has already been processed and there is no need to call our custom logic.  I think there
                // are a few messages that can still come in after WM_DESTROY, such as WM_NCDESTROY that is
                // sent after all child Windows have been destroyed.  Note that we try to unhook our custom
                // logic upon a WM_DESTROY by replacing it with the original WndProc but race conditions 
                // could affect that timing.
                if (_isDeActivated || _isDestroyed)
                {
                    return CallWindowProc(_originalWndProc, m.HWnd, (uint)m.HookMsgType, m.WParam, m.LParam);
                }

                //Grab a copy of the callbacks so we do not remain inside the lock while we are looping through
                // and calling back into unknown code that could (but hopefully not) be lengthy.  In rare race
                // conditions this could cause a slight delay in the amount of time it takes to "UnRegister"
                // a callback if code desired to do so.
                WndProcCallback[]? callbacksCopy = null;
                lock (_callbacksLocker)
                {
                    callbacksCopy = RegisteredWndProcCallbacks.ToArray();
                }

                if (callbacksCopy != null)
                {
                    //Loop through all callbacks until one callback handles the message or we exhaust all
                    // items in the loop.  If a callback handles the message we can return the result so
                    // long as it is not a DESTROY message, then we need to unhook.
                    foreach (var callback in callbacksCopy)
                    {
                        if (!callback(ref m)) continue;

                        if (msg != HookMsgType.DESTROY) return m.Result;

                        break;
                    }
                }

                //This one makes me nervous, keep an eye on it.  Of the many examples I happened upon, they
                // were re-hooking the WndProc with the original loop and for that reason I will do the
                // same.  If you are seeing exceptions, this could be a race condition of some sort.
                if (msg == HookMsgType.DESTROY) Destroy();

                //Note that this is still called after we call Destroy to unhook this routine because
                // it needs to receive the DESTROY message or else the window will not close.  Also
                // notice that we are sending the values inside  of the local HookMessage rather than
                // the ones that came into this CustomWndProc.  This lets other callbacks change the
                // parameter values if needed.
                return CallWindowProc(_originalWndProc, m.HWnd, (uint)m.HookMsgType, m.WParam, m.LParam);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Clears all callbacks associated with this Window but leaves the custom internal wndproc in
            /// place so that any future callbacks can still be called by the internal WndProc.
            /// </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public void ClearAllCallbacks()
            {
                //See https://docs.microsoft.com/en-us/windows/win32/controls/subclassing-overview regarding
                // Subclassing Controls Prior to ComCtl32.dll version 6.  A custom WndProc can only be
                // registered one time so we leave our custom one in place and clear and no longer call the
                // provided callbacks until new ones are registered again.  
                if (_isDeActivated) return;

                //Clear existing attached callbacks for good measure
                lock (_callbacksLocker)
                {
                    RegisteredWndProcCallbacks.Clear();
                    _isDeActivated = true;
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Deactivates all callbacks and processes the WM_DESTROY message by returning the original
            /// Window procedure.  Note that this should only be called if no other callbacks need to be
            /// associated with the provided Window Handle for the life of the application.  If future
            /// callbacks need to be attached to the given Window Handle then call the public DeActive
            /// method.  This is due to.
            /// </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            private void Destroy()
            {
                if(_isDestroyed)return;
                _isDestroyed = true;

                ClearAllCallbacks();

                //Return the original WndProc procedure.
                SetWindowLongPtr(WindowHandle, WindowLongIndexFlags.GWL_WNDPROC, _originalWndProc);

                //Notify the hook manager that this instance should no longer be tracked in its static 
                // application-wide hooked windows dictionary because the Window is closed.
                OnClosed?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}