// MIT License
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
namespace WinUI.Native
{
    using System;
    using System.Runtime.InteropServices;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <content>
    /// Assure GetWindow and SetWindow calls work on both 32 and 64 bit processors.
    /// </content>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public static partial class NativeMethods
    {
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
    }
}
