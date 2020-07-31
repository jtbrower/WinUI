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
    /// <content>   A native methods. </content>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public static partial class NativeMethods
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets current thread identifier. </summary>
        ///
        /// <returns>   The current thread identifier. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [DllImport(Kernel32, ExactSpelling = true)]
        public static extern uint GetCurrentThreadId();

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   This function should only be used on 32-bit platforms. </summary>
        ///
        /// <param name="hWnd">         The window. </param>
        /// <param name="nIndex">       Zero-based index of the. </param>
        /// <param name="dwNewLong">    The new long. </param>
        ///
        /// <returns>   An IntPtr. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [DllImport(User32, SetLastError = true)]
        private static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   This function should only be used on 64-bit platforms. </summary>
        ///
        /// <param name="hWnd">         The window. </param>
        /// <param name="nIndex">       Zero-based index of the. </param>
        /// <param name="dwNewLong">    The new long. </param>
        ///
        /// <returns>   An IntPtr. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [DllImport(User32, SetLastError = true)]
        private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   This function should only be used on 32-bit platforms. </summary>
        ///
        /// <param name="hWnd">     The window. </param>
        /// <param name="nIndex">   Zero-based index of the. </param>
        ///
        /// <returns>   The window long. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [DllImport(User32, SetLastError = true)]
        private static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   This function should only be used on 64-bit platforms. </summary>
        ///
        /// <param name="hWnd">     The window. </param>
        /// <param name="nIndex">   Zero-based index of the. </param>
        ///
        /// <returns>   The window long pointer. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [DllImport(User32, SetLastError = true)]
        private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

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

        [DllImport(User32, SetLastError = true)]
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

        [DllImport(User32, SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Shows the window. </summary>
        ///
        /// <param name="hWnd">     The window. </param>
        /// <param name="nCmdShow"> The command show. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [DllImport(User32, SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }
}
