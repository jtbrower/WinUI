// MIT License
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
namespace WinUI.Native
{
    using Microsoft.UI.Xaml;
    using System;
    using System.Diagnostics;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using WinRT;
    using Windows.Foundation;
    using PInvoke;

    public static class NativeMethods
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// If you want the debugger to break when a PInvoke error has been detected, set this to true.
        /// Note that I am currently seeing a false error detected when the Window border has been
        /// removed and an attempt to set the transparency is made.  The command seems to work fine but
        /// the return value is not as expected.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static readonly bool S_BreakOnError = false;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   A bit-field of flags for specifying window styles. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [Flags]
        internal enum WindowStyles : uint
        {
            //https://docs.microsoft.com/en-us/windows/win32/winmsg/window-styles
            WS_OVERLAPPED = 0x0,
            WS_POPUP = 0x80000000,
            WS_MINIMIZE = 0x20000000,
            WS_VISIBLE = 0x10000000,
            WS_MAXIMIZE = 0x1000000,
            WS_BORDER = 0x800000,
            WS_CAPTION = 0xC00000,
            WS_CHILD = 0x40000000,
            WS_CHILDWINDOW = WS_CHILD,
            WS_CLIPCHILDREN = 0x2000000,
            WS_CLIPSIBLINGS = 0x4000000,
            WS_DISABLED = 0x8000000,
            WS_DLGFRAME = 0x400000,
            WS_GROUP = 0x20000,
            WS_SYSMENU = 0x80000,
            WS_THICKFRAME = 0x40000,
            WS_TABSTOP = 0x10000,
            WS_MINIMIZEBOX = 0x20000,
            WS_MAXIMIZEBOX = 0x10000,
            WS_TILED = 0x0,
            WS_SIZEBOX = 0x40000,
            WS_EX_DLGMODALFRAME = 0x1,
            WS_EX_NOPARENTNOTIFY = 0x4,
            WS_EX_TOPMOST = 0x8,
            WS_EX_TRANSPARENT = 0x20,
            WS_EX_APPWINDOW = 0x40000,
            WS_EX_LAYERED = 0x80000,
            WS_EX_LAYOUTRTL = 0x400000,
            WS_EX_COMPOSITED = 0x2000000,
            WS_EX_NOACTIVATE = 0x8000000,
            WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
            WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Values that represent enum window show states. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        internal enum EnumWindowShowStates : int
        {
            Hide = 0,

            Normal = 1,

            Minimized = 2,

            Maximized = 3,

            ShowNoActivateRecentPosition = 4,

            Show = 5,

            MinimizeActivateNext = 6,

            MinimizeNoActivate = 7,

            ShowNoActivate = 8,

            Restore = 9,

            ShowDefault = 10,

            ForceMinimize = 11
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   A platform safe. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static class PlatformSafe
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Sets window 32. </summary>
            ///
            /// <param name="hWnd">         The window. </param>
            /// <param name="nIndex">       Zero-based index of the. </param>
            /// <param name="dwNewLong">    The new long. </param>
            ///
            /// <returns>   An IntPtr. </returns>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            [DllImport("user32.dll", SetLastError = true, EntryPoint = "SetWindowLong")]
            private static extern IntPtr SetWindow32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Sets window 64. </summary>
            ///
            /// <param name="hWnd">         The window. </param>
            /// <param name="nIndex">       Zero-based index of the. </param>
            /// <param name="dwNewLong">    The new long. </param>
            ///
            /// <returns>   An IntPtr. </returns>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            [DllImport("user32.dll", SetLastError = true, EntryPoint = "SetWindowLongPtr")]
            private static extern IntPtr SetWindow64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Gets a window. </summary>
            ///
            /// <param name="hWnd">     The window. </param>
            /// <param name="nIndex">   Zero-based index of the. </param>
            ///
            /// <returns>   The window. </returns>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            internal static IntPtr GetWindow(IntPtr hWnd, int nIndex)
            {
                return Environment.Is64BitProcess ?
                    GetWindowLongPtr(hWnd, nIndex) :
                    GetWindowLong(hWnd, nIndex);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Gets window long. </summary>
            ///
            /// <param name="hWnd">     The window. </param>
            /// <param name="nIndex">   Zero-based index of the. </param>
            ///
            /// <returns>   The window long. </returns>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            [DllImport("user32.dll", EntryPoint = "GetWindowLong", SetLastError = true)]
            private static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Gets window long pointer. </summary>
            ///
            /// <param name="hWnd">     The window. </param>
            /// <param name="nIndex">   Zero-based index of the. </param>
            ///
            /// <returns>   The window long pointer. </returns>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", SetLastError = true)]
            private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Sets a window. </summary>
            ///
            /// <param name="hWnd">         The window. </param>
            /// <param name="nIndex">       Zero-based index of the. </param>
            /// <param name="dwNewLong">    The new long. </param>
            ///
            /// <returns>   An IntPtr. </returns>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            internal static IntPtr SetWindow(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
            {
                return Environment.Is64BitProcess ?
                    SetWindow64(hWnd, nIndex, dwNewLong) :
                    SetWindow32(hWnd, nIndex, dwNewLong);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Calculates the non-client size.  This is the space where any border, menu or titlebar is
        /// located.
        /// </summary>
        ///
        /// <param name="hWnd">             The window. </param>
        /// <param name="nonClientSize">    [out] Size of the non client area in Physical Pixels or
        ///                                 device units. </param>
        /// <param name="hasMenu">          (Optional) True if has menu, false if not. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool GetNonClientSize(this IntPtr hWnd, out Size nonClientSize)
        {
            //A client rect sitting at 0,0 with calculated height and width.
            nonClientSize = new Size();
            if (!User32.GetClientRect(hWnd, out var clientRect))
            {
                WriteGlobalErrorMsgIfSet();
                return false;
            }

            if (!User32.GetWindowRect(hWnd, out var windowRect))
            {
                WriteGlobalErrorMsgIfSet();
                return false;
            }

            var windowHeight = windowRect.GetHeight();
            var windowWidth = windowRect.GetWidth();
            var clientHeight = clientRect.GetHeight();
            var clientWidth = clientRect.GetWidth();

            nonClientSize.Height = windowHeight - clientHeight;
            nonClientSize.Width = windowWidth - clientWidth;
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The lwa alpha. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private const int LWA_ALPHA = 0x2;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The gwl exstyle. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private const int GWL_EXSTYLE = -20;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The gwl style. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private const int GWL_STYLE = -16;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Updates the window described by hWnd. </summary>
        ///
        /// <param name="hWnd"> The window. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "UpdateWindow")]
        public static extern bool UpdateWindow(IntPtr hWnd);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets layered window attributes. </summary>
        ///
        /// <param name="hwnd">     The hwnd. </param>
        /// <param name="crKey">    The carriage return key. </param>
        /// <param name="bAlpha">   The alpha. </param>
        /// <param name="dwFlags">  The flags. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [DllImport("user32.dll")]
        private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Move window. </summary>
        ///
        /// <param name="hWnd">     The window. </param>
        /// <param name="X">        The X coordinate. </param>
        /// <param name="Y">        The Y coordinate. </param>
        /// <param name="nWidth">   The width. </param>
        /// <param name="nHeight">  The height. </param>
        /// <param name="bRepaint"> True to repaint. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Shows the window. </summary>
        ///
        /// <param name="hWnd">     The window. </param>
        /// <param name="nCmdShow"> The command show. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Removes the window border described by windowHandle. </summary>
        ///
        /// <param name="windowHandle"> Handle of the window. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void RemoveWindowBorder(this Window window, IntPtr windowHandle)
        {
            SetWindowFlags(windowHandle, GWL_STYLE, WindowStyles.WS_VISIBLE | WindowStyles.WS_POPUP);

            //Needed to force a repaint because other sain methods are not working.
            window.HackForceRepaint(windowHandle);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   An IntPtr extension method that maximize window. </summary>
        ///
        /// <param name="windowHandle"> Handle of the window. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void MaximizeWindow(this IntPtr windowHandle)
        {
            ShowWindow(windowHandle, (int)EnumWindowShowStates.Maximized);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   An IntPtr extension method that minimize window. </summary>
        ///
        /// <param name="windowHandle"> Handle of the window. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void MinimizeWindow(this IntPtr windowHandle)
        {
            ShowWindow(windowHandle, (int)EnumWindowShowStates.Minimized);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   An IntPtr extension method that restore window. </summary>
        ///
        /// <param name="windowHandle"> Handle of the window. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void RestoreWindow(this IntPtr windowHandle)
        {
            ShowWindow(windowHandle, (int)EnumWindowShowStates.Restore);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the last window 32 error message. </summary>
        ///
        /// <param name="msg">  [out] The message. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static bool GetLastWin32ErrorMessage(out string? msg)
        {
            var error = Marshal.GetLastWin32Error();
            var errorSet = error != 0;

            if (!errorSet) { msg = null; return false; }
            msg = LastWin32ErrorMessage;
            return errorSet;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets current procedure main window handle. </summary>
        ///
        /// <returns>   The current procedure main window handle. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static IntPtr GetCurrentProcMainWindowHandle()
        {
            using var process = Process.GetCurrentProcess();
            return process.MainWindowHandle;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a message describing the last window windows error. </summary>
        ///
        /// <value> A message describing the last window 32 error. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string LastWin32ErrorMessage =>
            new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error()).Message;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets window flags. </summary>
        ///
        /// <param name="windowHandle"> Handle of the window. </param>
        /// <param name="index">        Zero-based index of the. </param>
        /// <param name="styles">       The styles. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        internal static void SetWindowFlags(IntPtr windowHandle, int index, WindowStyles styles)
        {
            var desiredStyle = new IntPtr(unchecked((int)styles));
            var success = IntPtr.Zero != PlatformSafe.SetWindow(windowHandle, index, desiredStyle);
            WriteLineIf(!success, LastWin32ErrorMessage);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Writes a line if. </summary>
        ///
        /// <param name="condition">    True to condition. </param>
        /// <param name="msg">          [out] The message. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static void WriteLineIf(bool condition, string? msg)
        {
            if (!condition || msg == null) return;
            Debug.WriteLine(msg);
            if (!S_BreakOnError) return;
#if DEBUG
            //Note that this call is not removed automatically like Debug.WriteLine.  If hit in production
            // it will throw an exception.  Lesson learned years ago!
            Debugger.Break();
#endif
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Writes the line if error set. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static void WriteGlobalErrorMsgIfSet()
        {
            if (GetLastWin32ErrorMessage(out var msg))
            {
                Debug.WriteLine(msg);
                if (!S_BreakOnError) return;
#if DEBUG
                //Note that this call is not removed automatically like Debug.WriteLine.  If hit in production
                // it will throw an exception.  Lesson learned years ago!
                Debugger.Break();
#endif
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   A Window extension method that move by. </summary>
        ///
        /// <param name="window">       The window to act on. </param>
        /// <param name="windowHandle"> Handle of the window. </param>
        /// <param name="x">            The x coordinate. </param>
        /// <param name="y">            The y coordinate. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void MoveBy(this IntPtr windowHandle, int x, int y)
        {
            if (!PInvoke.User32.GetWindowRect(windowHandle, out var r))
            {
                WriteGlobalErrorMsgIfSet();
                return;
            }
            MoveWindow(windowHandle, r.left + x, r.top + y, r.GetWidth(), r.GetHeight(), true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   A Window extension method that shrink by. </summary>
        ///
        /// <param name="window">       The window to act on. </param>
        /// <param name="windowHandle"> Handle of the window. </param>
        /// <param name="x">            The x coordinate. </param>
        /// <param name="y">            The y coordinate. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void ShrinkBy(this Window window, IntPtr windowHandle, int x, int y)
        {
            var r = GetWindowRect(window, windowHandle);
            MoveWindow(windowHandle, r.Left, r.Top, r.GetWidth() - x, r.GetHeight() - y, true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   A Window extension method that resize window. </summary>
        ///
        /// <param name="window">       The window to act on. </param>
        /// <param name="windowHandle"> Handle of the window. </param>
        /// <param name="x">            The x coordinate. </param>
        /// <param name="y">            The y coordinate. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void ResizeWindow(this IntPtr windowHandle, int x, int y)
        {
            //Using SWP_NOMOVE causes the location coordinates (0,0) to be ignored.
            if (!PInvoke.User32.SetWindowPos(windowHandle, IntPtr.Zero, 0, 0, x, y, PInvoke.User32.SetWindowPosFlags.SWP_NOZORDER | PInvoke.User32.SetWindowPosFlags.SWP_NOMOVE))
            {
                WriteGlobalErrorMsgIfSet();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// An ugly hack that will add one pixel to the Window height, then remove the added pixel to
        /// force the Window to update itself and its contents.  See the function for more details on the
        /// hack.
        /// </summary>
        ///
        /// <param name="window">       The window to act on. </param>
        /// <param name="windowHandle"> Handle of the window. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static void HackForceRepaint(this Window window, IntPtr windowHandle)
        {
            var r = GetWindowRect(window, windowHandle);

            var width = r.GetWidth();
            var height = r.GetHeight();

            // Here is the hack, add 1 pixel to either the width or the height and the redraw will happen. 
            // The use case for this 'temporary' hack comes from the fact that when I remove the Window
            // border and titlebar, I found that the bottom area of the window was not hit testable for my
            // DragMove feature.  I tried to call various functions such as RedrawWindow but so far nothing
            // would cause the update except changing the size of the window.  The side effect of this hack
            // would be that when the window is in the maximized state, adding another pixel pushes it off
            // screen.  So I decided to make the call twice, removing the added pixel. Fixing this is low
            // priority in the big picture of work.
            PInvoke.User32.SetWindowPos(windowHandle, IntPtr.Zero, 0, 0, width, height + 1,
                PInvoke.User32.SetWindowPosFlags.SWP_NOZORDER | PInvoke.User32.SetWindowPosFlags.SWP_NOMOVE);
            PInvoke.User32.SetWindowPos(windowHandle, IntPtr.Zero, 0, 0, width, height - 1,
                PInvoke.User32.SetWindowPosFlags.SWP_NOZORDER | PInvoke.User32.SetWindowPosFlags.SWP_NOMOVE);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   A Window extension method that resize window. </summary>
        ///
        /// <param name="windowHandle"> Handle of the window. </param>
        /// <param name="x">            The x coordinate. </param>
        /// <param name="y">            The y coordinate. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void ResizeWindow(this Window window, int x, int y)
        {
            window.GetHandle().ResizeWindow(x, y);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   A Window extension method that gets window rectangle. </summary>
        ///
        /// <param name="window">       The window to act on. </param>
        /// <param name="windowHandle"> Handle of the window. </param>
        ///
        /// <returns>   The window rectangle. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static RECT GetWindowRect(this Window window, IntPtr windowHandle)
        {
            var success = GetWindowRect(new HandleRef(window, windowHandle), out var rct);
            WriteLineIf(!success, LastWin32ErrorMessage);
            return rct;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   A Window extension method that gets window rectangle. </summary>
        ///
        /// <param name="hWnd">     The window. </param>
        /// <param name="lpRect">   [out] The rectangle. </param>
        ///
        /// <returns>   The window rectangle. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(HandleRef hWnd, out RECT lpRect);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   A rectangle. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   A RECT extension method that gets a width. </summary>
        ///
        /// <param name="r">    The r to act on. </param>
        ///
        /// <returns>   The width. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static int GetWidth(this RECT r)
        {
            return r.Right - r.Left;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   A RECT extension method that gets a width. </summary>
        ///
        /// <param name="r">    The r to act on. </param>
        ///
        /// <returns>   The width. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static int GetWidth(this PInvoke.RECT r)
        {
            return r.right - r.left;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   A RECT extension method that gets a height. </summary>
        ///
        /// <param name="r">    The r to act on. </param>
        ///
        /// <returns>   The height. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static int GetHeight(this RECT r)
        {
            return r.Bottom - r.Top;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   A RECT extension method that gets a height. </summary>
        ///
        /// <param name="r">    The r to act on. </param>
        ///
        /// <returns>   The height. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static int GetHeight(this PInvoke.RECT r)
        {
            return r.bottom - r.top;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Clears the window flags. </summary>
        ///
        /// <param name="windowHandle"> Handle of the window. </param>
        /// <param name="index">        Zero-based index of the. </param>
        /// <param name="styles">       The styles. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        internal static void ClearWindowFlags(IntPtr windowHandle, int index, WindowStyles styles)
        {
            var desiredStyle = PlatformSafe.GetWindow(windowHandle, index);
            WriteLineIf(GetLastWin32ErrorMessage(out var msg), msg);

            if (IntPtr.Size == 4)
                desiredStyle = new IntPtr(unchecked(desiredStyle.ToInt32() & ~(int)styles));
            else
                desiredStyle = new IntPtr(unchecked(desiredStyle.ToInt64() & ~(int)styles));

            var success = IntPtr.Zero != PlatformSafe.SetWindow(windowHandle, index, desiredStyle);
            WriteLineIf(!success, LastWin32ErrorMessage);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   A Window extension method that gets a handle. </summary>
        ///
        /// <param name="window">   The window to act on. </param>
        ///
        /// <returns>   The handle. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static IntPtr GetHandle(this Window window)
        {
            WindowWrapper windowWrapper = WindowWrapper.FromAbi(window.ThisPtr);
            return windowWrapper.WindowHandle;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Adds a window border. </summary>
        ///
        /// <param name="window">       The window to act on. </param>
        /// <param name="windowHandle"> Handle of the window. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void AddWindowBorder(this Window window, IntPtr windowHandle)
        {
            SetWindowFlags(windowHandle, GWL_STYLE, WindowStyles.WS_OVERLAPPEDWINDOW);

            //If you do not call this then the window won't have any content showing and it will not be
            // useable.
            window.Activate();

            //Needed to force a repaint because other sain methods are not working.
            window.HackForceRepaint(windowHandle);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets window transparent. </summary>
        ///
        /// <exception cref="InvalidOperationException">    Thrown when the requested operation is
        ///                                                 invalid. </exception>
        ///
        /// <param name="windowHandle">         Handle of the window. </param>
        /// <param name="percentTransparent">   The percent transparent. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void SetWindowTransparency(this IntPtr windowHandle, int percentTransparent)
        {
            //Sanity Check
            if (percentTransparent > 100 || percentTransparent < 0)
            {
                throw new InvalidOperationException($"{nameof(percentTransparent)} valid values are 0-100.");
            }

            SetWindowFlags(windowHandle, GWL_EXSTYLE, WindowStyles.WS_EX_LAYERED);

            //Calculate and set alpha attribute
            var alpha = (byte)(0xFF * (100 - percentTransparent) / 100);
            var failed = !SetLayeredWindowAttributes(windowHandle, 0, alpha, LWA_ALPHA);

            string? msg = null;
            WriteLineIf(failed && GetLastWin32ErrorMessage(out msg), msg);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Removes the window transparency described by windowHandle. </summary>
        ///
        /// <param name="windowHandle"> Handle of the window. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void RemoveWindowTransparency(this IntPtr windowHandle)
        {
            ClearWindowFlags(windowHandle, GWL_EXSTYLE, WindowStyles.WS_EX_LAYERED);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Interface for window native. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("EECDBF0E-BAE9-4CB6-A68E-9598E1CB57BB")]
        private interface IWindowNative
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Gets the handle of the window. </summary>
            ///
            /// <value> The window handle. </value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            IntPtr WindowHandle { get; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// See https://github.com/microsoft/microsoft-ui-xaml/issues/2564#issuecomment-638420195 for
        /// information regarding the source of the WindowWrapper class and the MIT License where the
        /// code was located https://github.com/microsoft/WinUI-3-Demos/blob/master/LICENSE.
        /// </summary>
        ///
        /// <seealso cref="Oceanside.Views.WinUI.WindowInterop.IWindowNative"/>
        /// <seealso cref="Oceanside.Views.WinUI.Interop.IWindowNative"/>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [Guid("EECDBF0E-BAE9-4CB6-A68E-9598E1CB57BB")]
        private class WindowWrapper : IWindowNative
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   A vftbl. </summary>
            ///
            /// <seealso cref="Oceanside.Views.WinUI.Interop.IWindowNative"/>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            [Guid("EECDBF0E-BAE9-4CB6-A68E-9598E1CB57BB")]
            public struct Vftbl
            {
                ////////////////////////////////////////////////////////////////////////////////////////////////////
                /// <summary>   Gets window handle 0. </summary>
                ///
                /// <param name="thisPtr">      this pointer. </param>
                /// <param name="windowHandle"> [out] Handle of the window. </param>
                ///
                /// <returns>   An int. </returns>
                ////////////////////////////////////////////////////////////////////////////////////////////////////

                public delegate int _get_WindowHandle_0(IntPtr thisPtr, out IntPtr windowHandle);

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                /// <summary>   Zero-based index of the unknown vftbl. </summary>
                ////////////////////////////////////////////////////////////////////////////////////////////////////

                internal WinRT.Interop.IUnknownVftbl IUnknownVftbl;

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                /// <summary>   The get window handle 0. </summary>
                ////////////////////////////////////////////////////////////////////////////////////////////////////

                public _get_WindowHandle_0 get_WindowHandle_0;

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                /// <summary>   The abi to projection vftable. </summary>
                ////////////////////////////////////////////////////////////////////////////////////////////////////

                public static readonly Vftbl AbiToProjectionVftable;

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                /// <summary>   The abi to projection vftable pointer. </summary>
                ////////////////////////////////////////////////////////////////////////////////////////////////////

                public static readonly IntPtr AbiToProjectionVftablePtr;

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                /// <summary>
                /// Initializes static members of the Oceanside.Views.WinUI.WindowInterop.WindowWrapper class.
                /// </summary>
                ////////////////////////////////////////////////////////////////////////////////////////////////////

                static Vftbl()
                {
                    AbiToProjectionVftable = new Vftbl
                    {
                        IUnknownVftbl = WinRT.Interop.IUnknownVftbl.AbiToProjectionVftbl,
                        get_WindowHandle_0 = Do_Abi_get_WindowHandle_0
                    };
                    AbiToProjectionVftablePtr = Marshal.AllocHGlobal(Marshal.SizeOf<Vftbl>());
                    Marshal.StructureToPtr(AbiToProjectionVftable, AbiToProjectionVftablePtr, false);
                }

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                /// <summary>   Executes the abi get window handle 0 operation. </summary>
                ///
                /// <param name="thisPtr">      this pointer. </param>
                /// <param name="windowHandle"> [out] Handle of the window. </param>
                ///
                /// <returns>   An int. </returns>
                ////////////////////////////////////////////////////////////////////////////////////////////////////

                private static int Do_Abi_get_WindowHandle_0(IntPtr thisPtr, out IntPtr windowHandle)
                {
                    windowHandle = default;
                    try
                    {
                        windowHandle = ComWrappersSupport.FindObject<IWindowNative>(thisPtr).WindowHandle;
                    }
                    catch (Exception ex)
                    {
                        return Marshal.GetHRForException(ex);
                    }
                    return 0;
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Initializes this  from the given abi. </summary>
            ///
            /// <param name="thisPtr">  this pointer. </param>
            ///
            /// <returns>   An ObjectReference{Vftbl} </returns>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            internal static ObjectReference<Vftbl> FromAbi(IntPtr thisPtr)
            {
                return ObjectReference<Vftbl>.FromAbi(thisPtr);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Implicit cast that converts the given IObjectReference to a WindowWrapper. </summary>
            ///
            /// <param name="obj">  The object. </param>
            ///
            /// <returns>   The result of the operation. </returns>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static implicit operator WindowWrapper(IObjectReference obj)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return (obj != null) ? new WindowWrapper(obj) : null;
#pragma warning restore CS8603 // Possible null reference return.
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   The object. </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            protected readonly ObjectReference<Vftbl> _obj;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Gets the object reference. </summary>
            ///
            /// <value> The object reference. </value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public IObjectReference ObjRef => _obj;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Gets this pointer. </summary>
            ///
            /// <value> this pointer. </value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public IntPtr ThisPtr => _obj.ThisPtr;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Converts this  to an interface. </summary>
            ///
            /// <typeparam name="I">    Generic type parameter. </typeparam>
            ///
            /// <returns>   An ObjectReference{I} </returns>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public ObjectReference<I> AsInterface<I>()
            {
                return _obj.As<I>();
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Gets as. </summary>
            ///
            /// <typeparam name="A">    Type of a. </typeparam>
            ///
            /// <returns>   A. </returns>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public A As<A>()
            {
                return _obj.AsType<A>();
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Initializes a new instance of the Oceanside.Views.WinUI.WindowInterop.WindowWrapper class.
            /// </summary>
            ///
            /// <param name="obj">  The object. </param>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public WindowWrapper(IObjectReference obj) : this(obj.As<Vftbl>()) { }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Initializes a new instance of the Oceanside.Views.WinUI.WindowInterop.WindowWrapper class.
            /// </summary>
            ///
            /// <param name="obj">  The object. </param>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            internal WindowWrapper(ObjectReference<Vftbl> obj)
            {
                _obj = obj;
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Gets the handle of the window. </summary>
            ///
            /// <value> The window handle. </value>
            ///
            /// <seealso cref="Oceanside.Views.WinUI.Interop.IWindowNative.WindowHandle"/>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public IntPtr WindowHandle
            {
                get
                {
                    Marshal.ThrowExceptionForHR(_obj.Vftbl.get_WindowHandle_0(ThisPtr, out var windowHandle));

                    return windowHandle;
                }
            }

        }
    }
}
