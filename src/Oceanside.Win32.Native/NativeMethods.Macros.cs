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
    /// <content>   A native methods. </content>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public static partial class NativeMethods
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Grab high word bits out of int. </summary>
        ///
        /// <param name="n">    An int to process. </param>
        ///
        /// <returns>   An int. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static int HIWORD(int n)
        {
            return (n >> 16) & 0xffff;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Grab high word bits out of IntPtr. </summary>
        ///
        /// <param name="n">    An IntPtr to process. </param>
        ///
        /// <returns>   An int. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static int HIWORD(IntPtr n)
        {
            return HIWORD(unchecked((int)(long)n));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Grab low word bits out of int. </summary>
        ///
        /// <param name="n">    An IntPtr to process. </param>
        ///
        /// <returns>   An int. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static int LOWORD(int n)
        {
            return n & 0xffff;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Grab low word bits out of IntPtr. </summary>
        ///
        /// <param name="n">    An IntPtr to process. </param>
        ///
        /// <returns>   An int. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static int LOWORD(IntPtr n)
        {
            return LOWORD(unchecked((int)(long)n));
        }
    }
}
