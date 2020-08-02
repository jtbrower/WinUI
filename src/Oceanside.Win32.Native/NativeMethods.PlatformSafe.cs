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

namespace Oceanside.Win32.Native
{
    using System;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <content>
    /// Assure GetWindow and SetWindow calls work on both 32 and 64 bit processors.
    /// </content>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public static partial class NativeMethods
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Assures that the correct functions are called when both 64-bit and 32-bit functions exist.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static class PlatformSafe
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Assures that either GetWindowLong or GetWindowLongPtr is called based on either 32-bit or 64-
            /// bit platform being executed.
            /// </summary>
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
            /// <summary>
            /// Assures that either SetWindowLong or SetWindowLongPtr is called based on either 32-bit or 64-
            /// bit platform being executed.
            /// </summary>
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
                    SetWindowLongPtr(hWnd, nIndex, dwNewLong) :
                    SetWindowLong(hWnd, nIndex, dwNewLong);
            }
        }
    }
}
