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
    public static partial class NativeMethods
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   LWA_ALPHA </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private const int LWA_ALPHA = 0x2;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   GWL_EXSTYLE. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private const int GWL_EXSTYLE = -20;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   GWL_STYLE </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private const int GWL_STYLE = -16;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The kernel 32. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public const string Kernel32 = "kernel32.dll";

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The user 32. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public const string User32 = "user32.dll";
    }
}
