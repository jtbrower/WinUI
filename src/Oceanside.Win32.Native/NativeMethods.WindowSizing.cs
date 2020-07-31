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
    using System.Diagnostics;
    using PI=PInvoke.User32;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A native methods. </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public static partial class NativeMethods
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Calculates the non-client size.  This is the space where any border, menu or TitleBar is
        /// located.
        /// </summary>
        ///
        /// <param name="hWnd">     The window. </param>
        /// <param name="width">    [out] width of the non client area in Physical Pixels or device units. </param>
        /// <param name="height">   [out] height of the non client area in Physical Pixels or device units. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool GetNonClientSize(this IntPtr hWnd, out double width, out double height)
        {
            //A client rect sitting at 0,0 with calculated height and width.
            width = 0;
            height = 0;
            
            if (!PI.GetClientRect(hWnd, out var clientRect))
            {
                WriteGlobalErrorMsgIfSet();
                return false;
            }

            if (!PI.GetWindowRect(hWnd, out var windowRect))
            {
                WriteGlobalErrorMsgIfSet();
                return false;
            }

            var windowHeight = windowRect.GetHeight();
            var windowWidth = windowRect.GetWidth();
            var clientHeight = clientRect.GetHeight();
            var clientWidth = clientRect.GetWidth();

            height = windowHeight - clientHeight;
            width = windowWidth - clientWidth;
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   A Window extension method that move by. </summary>
        ///
        /// <param name="windowHandle"> Handle of the window. </param>
        /// <param name="x">            The x coordinate. </param>
        /// <param name="y">            The y coordinate. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void MoveBy(this IntPtr windowHandle, int x, int y)
        {
            if (!PI.GetWindowRect(windowHandle, out var r))
            {
                WriteGlobalErrorMsgIfSet();
                return;
            }
            MoveWindow(windowHandle, r.left + x, r.top + y, r.GetWidth(), r.GetHeight(), true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   A Window extension method that shrinks a window by an amount x,y. </summary>
        ///
        /// <param name="windowHandle"> Handle of the window. </param>
        /// <param name="x">            The x coordinate. </param>
        /// <param name="y">            The y coordinate. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void ShrinkBy(this IntPtr windowHandle, int x, int y)
        {
            if (!PI.GetWindowRect(windowHandle, out var r))
            {
                WriteGlobalErrorMsgIfSet();
                return;
            }
            MoveWindow(windowHandle, r.left, r.top, r.GetWidth() - x, r.GetHeight() - y, true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Scale a Window by a percentage of its current size. </summary>
        ///
        /// <param name="windowHandle"> Handle of the window. </param>
        /// <param name="percent">      The percent. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void ScaleBy(this IntPtr windowHandle, double percent)
        {
            if (!PI.GetWindowRect(windowHandle, out var r))
            {
                WriteGlobalErrorMsgIfSet();
                return;
            }
            var height = (int)(r.GetHeight() * percent);
            var width = (int)(r.GetWidth() * percent);
            Debug.WriteLine($"Size {width},{height}");
            MoveWindow(windowHandle, r.left, r.top, width, height, true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   A Window extension method that resize window. </summary>
        ///
        /// <param name="windowHandle"> Handle of the window. </param>
        /// <param name="x">            The x coordinate. </param>
        /// <param name="y">            The y coordinate. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void ResizeWindow(this IntPtr windowHandle, int x, int y)
        {
            if (windowHandle.IsMaximized())
            {
                //Note that if the Window is currently maximized and we resize the Window, it will continue to 
                // believe it is in a maximized state.  To avoid this problem I call to assure it is
                // restored before I change the size, but it seems there has to be a better way to handle this.
                //
                // Note that there is a fatal and strange bug that occurs if the Window thinks it is in a 
                // maximized state and it crosses between two monitors that cause a DPI change.  It will cause
                // infinite recursive DPI changed events to fire to the point your only option to kill the app
                // is with the task manager.
                PI.ShowWindow(windowHandle, PI.WindowShowStyle.SW_SHOWNORMAL);
            }
            //Using SWP_NOMOVE causes the location coordinates (0,0) to be ignored.
            if (!PI.SetWindowPos(windowHandle, IntPtr.Zero, 0, 0, x, y, PI.SetWindowPosFlags.SWP_NOZORDER | PI.SetWindowPosFlags.SWP_NOMOVE))
            {
                WriteGlobalErrorMsgIfSet();
            }
        }
    }
}
