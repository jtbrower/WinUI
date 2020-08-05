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

namespace Oceanside.Win32.Native.Hooks
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Linq;

    public sealed partial class WindowHookManager : IDisposable
    {
        #region Public Functions
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Callback, called when the window procedure. </summary>
        ///
        /// <param name="message">  [in,out] The message. </param>
        ///
        /// <returns>   A bool. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public delegate bool WndProcCallback(ref HookMessage message);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Dictionary that contains all of the hooked Windows and their associated callbacks related to
        /// the current application.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static readonly ConcurrentDictionary<IntPtr, HookedWindow> ApplicationsHookedWindows
            = new ConcurrentDictionary<IntPtr, HookedWindow>();

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Adds a window procedure callback to 'callback'. </summary>
        ///
        /// <param name="windowHandle"> Handle of the window. </param>
        /// <param name="callback">     The callback. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void AddWndProcCallback(IntPtr windowHandle, WndProcCallback callback)
        {
            //If we already have hooks associated with this windowHandle then just
            // add the callback
            if (ApplicationsHookedWindows.TryGetValue(windowHandle, out var hookedWindow))
            {
                hookedWindow.AttachCallback(callback);
                hookedWindow.OnClosed += HookedWindow_OnDestroyed;
            }
            else
            {
                hookedWindow = new HookedWindow(windowHandle, callback);
                ApplicationsHookedWindows[windowHandle] = hookedWindow;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Hooked window on disposed. </summary>
        ///
        /// <param name="sender">   The sender. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static void HookedWindow_OnDestroyed(object? sender, EventArgs e)
        {
            if (!(sender is HookedWindow hookedWindow)) return;
            if (ApplicationsHookedWindows.ContainsKey(hookedWindow.WindowHandle))
                _ = ApplicationsHookedWindows.TryRemove(hookedWindow.WindowHandle, out var _);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Removes the window procedure callback. </summary>
        ///
        /// <param name="windowHandle"> Handle of the window. </param>
        /// <param name="callback">     The callback. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void RemoveWndProcCallback(IntPtr windowHandle, WndProcCallback callback)
        {
            if (ApplicationsHookedWindows.TryGetValue(windowHandle, out var hookedWindow))
            {
                hookedWindow.DetachCallback(callback);
                if (hookedWindow.RegisteredWndProcCallbacks.Count == 0)
                {
                    hookedWindow.ClearAllCallbacks();
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ///
        /// <seealso cref="IDisposable.Dispose()"/>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Dispose()
        {
            //Note that I am calling ToList because we are about to modify this dictionary
            foreach (var hook in ApplicationsHookedWindows.Select(h => h.Value).ToList())
            {
                //Remove the event handler so we are not notified of a disposal we already know about.
                hook.OnClosed -= HookedWindow_OnDestroyed;
                hook.ClearAllCallbacks();
                _ = ApplicationsHookedWindows.TryRemove(hook.WindowHandle, out var _);
            }
        }
        #endregion
    }
}
