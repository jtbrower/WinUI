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
    using PI = PInvoke.User32;

    public static partial class NativeMethods
    {
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
        /// <summary>   An IntPtr extension method that query if 'windowHandle' is maximized. </summary>
        ///
        /// <param name="windowHandle"> Handle of the window. </param>
        ///
        /// <returns>   True if maximized, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool IsMaximized(this IntPtr windowHandle)
        {
            //This command will throw a Win32 exception if something goes wrong.  Prevent the exception and just return
            // false, but assure its written to the Debug pipe.
            try
            {
                var windowPlacement = PI.GetWindowPlacement(windowHandle);
                var showStyle = windowPlacement.showCmd;
                return showStyle == PI.WindowShowStyle.SW_MAXIMIZE || showStyle == PI.WindowShowStyle.SW_SHOWMAXIMIZED;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return false;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   An IntPtr extension method that maximize window. </summary>
        ///
        /// <param name="windowHandle"> Handle of the window. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void MaximizeWindow(this IntPtr windowHandle)
        {
            _ = ShowWindow(windowHandle, (int)EnumWindowShowStates.Maximized);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   An IntPtr extension method that minimize window. </summary>
        ///
        /// <param name="windowHandle"> Handle of the window. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void MinimizeWindow(this IntPtr windowHandle)
        {
            _ = ShowWindow(windowHandle, (int)EnumWindowShowStates.Minimized);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   An IntPtr extension method that restore window. </summary>
        ///
        /// <param name="windowHandle"> Handle of the window. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void RestoreWindow(this IntPtr windowHandle)
        {
            _ = ShowWindow(windowHandle, (int)EnumWindowShowStates.Restore);
        }
    }
}
