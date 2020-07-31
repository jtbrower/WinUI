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

namespace WinUI.Native
{
    using System;

    public static partial class NativeMethods
    {
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
        /// <summary>   Sets window styles by logical or'ing them in with the existing values. </summary>
        ///
        /// <param name="windowHandle"> Handle of the window. </param>
        /// <param name="index">        Zero-based index of the. </param>
        /// <param name="styles">       The styles. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        internal static void SetWindowStyles(IntPtr windowHandle, int index, WindowStyles styles)
        {
            var desiredStyle = new IntPtr(unchecked((int)styles));
            var success = IntPtr.Zero != PlatformSafe.SetWindow(windowHandle, index, desiredStyle);
            WriteLineIf(!success, LastWin32ErrorMessage);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Clears the window styles. </summary>
        ///
        /// <param name="windowHandle"> Handle of the window. </param>
        /// <param name="index">        Zero-based index of the. </param>
        /// <param name="styles">       The styles. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        internal static void ClearWindowStyles(IntPtr windowHandle, int index, WindowStyles styles)
        {
            var desiredStyle = PlatformSafe.GetWindow(windowHandle, index);
            WriteLineIf(GetLastWin32ErrorMessage(out var msg), msg);

            desiredStyle = IntPtr.Size == 4
                ? new IntPtr(unchecked(desiredStyle.ToInt32() & ~(int)styles))
                : new IntPtr(unchecked(desiredStyle.ToInt64() & ~(int)styles));

            var success = IntPtr.Zero != PlatformSafe.SetWindow(windowHandle, index, desiredStyle);
            WriteLineIf(!success, LastWin32ErrorMessage);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   An IntPtr extension method that hides the window 32 non client area. </summary>
        ///
        /// <param name="windowHandle"> Handle of the window. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void HideWin32NonClientArea(this IntPtr windowHandle)
        {
            SetWindowStyles(windowHandle, GWL_STYLE, WindowStyles.WS_VISIBLE | WindowStyles.WS_POPUP);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// A Window extension method that shows the window 32 non client area. WARNING YOU NEED TO CALL
        /// Window.Activate after calling this method.
        /// </summary>
        ///
        /// <param name="windowHandle"> Handle of the window. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void ShowWin32NonClientArea(this IntPtr windowHandle)
        {
            SetWindowStyles(windowHandle, GWL_STYLE, WindowStyles.WS_OVERLAPPEDWINDOW);
        }
    }
}
