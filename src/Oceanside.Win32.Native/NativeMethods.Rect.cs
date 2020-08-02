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
    using System.Runtime.InteropServices;

    public static partial class NativeMethods
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   A rectangle. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   x position of upper-left corner. </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public int Left;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   y position of upper-left corner. </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public int Top;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   x position of lower-right corner. </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public int Right;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   y position of lower-right corner. </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public int Bottom;
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
    }
}
