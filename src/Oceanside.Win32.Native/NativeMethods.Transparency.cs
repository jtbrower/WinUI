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

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <content>   A native methods. </content>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public static partial class NativeMethods
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets window transparent. </summary>
        ///
        /// <exception cref="InvalidOperationException">    Thrown when the requested operation is
        ///                                                 invalid. </exception>
        ///
        /// <param name="windowHandle">         Handle of the window. </param>
        /// <param name="percentTransparent">   The percent transparent. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void SetWindowTransparency(this IntPtr windowHandle, int percentTransparent)
        {
            //Sanity Check
            if (percentTransparent > 100 || percentTransparent < 0)
            {
                throw new InvalidOperationException($"{nameof(percentTransparent)} valid values are 0-100.");
            }

            SetWindowStyles(windowHandle, GWL_EXSTYLE, WindowStyles.WS_EX_LAYERED);

            //Calculate and set alpha attribute
            var alpha = (byte)(0xFF * (100 - percentTransparent) / 100);
            var failed = !SetLayeredWindowAttributes(windowHandle, 0, alpha, LWA_ALPHA);

            string? msg = null;
            WriteLineIf(failed && GetLastWin32ErrorMessage(out msg), msg);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Removes the window transparency described by windowHandle. </summary>
        ///
        /// <param name="windowHandle"> Handle of the window. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void RemoveWindowTransparency(this IntPtr windowHandle)
        {
            ClearWindowStyles(windowHandle, GWL_EXSTYLE, WindowStyles.WS_EX_LAYERED);
        }
    }
}