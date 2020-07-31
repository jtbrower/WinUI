﻿// MIT License
//
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
    using System.Runtime.InteropServices;

    public sealed partial class WindowHookManager
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Call the original window procedure that existed before it was replaced. </summary>
        ///
        /// <param name="wndProc">  The window procedure. </param>
        /// <param name="hWnd">     The window. </param>
        /// <param name="msg">      The message. </param>
        /// <param name="wParam">   The parameter. </param>
        /// <param name="lParam">   The parameter. </param>
        ///
        /// <returns>   An IntPtr. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [DllImport(NativeMethods.User32, SetLastError = true)]
        public static extern IntPtr CallWindowProc(IntPtr wndProc, IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    }
}