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
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    public static partial class NativeMethods
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// If you want the debugger to break when a PInvoke error has been detected, set this to true.
        /// Note that I am currently seeing a false error detected when the Window border has been
        /// removed and an attempt to set the transparency is made.  The command seems to work fine but
        /// the return value is not as expected.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static readonly bool S_BreakOnError;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the last window 32 error message. </summary>
        ///
        /// <param name="msg">  [out] The message. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static bool GetLastWin32ErrorMessage(out string? msg)
        {
            var error = Marshal.GetLastWin32Error();
            var errorSet = error != 0;

            if (!errorSet) { msg = null; return false; }
            msg = LastWin32ErrorMessage;
            return errorSet;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Writes a line if. </summary>
        ///
        /// <param name="condition">    True to condition. </param>
        /// <param name="msg">          [out] The message. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static void WriteLineIf(bool condition, string? msg)
        {
            if (!condition || msg == null) return;
            Debug.WriteLine(msg);
            if (!S_BreakOnError) return;
#if DEBUG
            //Note that this call is not removed automatically like Debug.WriteLine.  If hit in production
            // it will throw an exception.  Lesson learned years ago!
            Debugger.Break();
#endif
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// If the condition is met and the global error is set, Marshal.GetLastWin32Error() will be
        /// written to the debug console, if DEBUG is defined.
        /// </summary>
        ///
        /// <param name="condition">    True to condition. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        internal static void WriteErrorIf(bool condition)
        {
            if(condition)
                WriteGlobalErrorMsgIfSet();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Writes the line if error set. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static void WriteGlobalErrorMsgIfSet()
        {
            if (GetLastWin32ErrorMessage(out var msg))
            {
                Debug.WriteLine(msg);
                if (!S_BreakOnError) return;
#if DEBUG
                //Note that this call is not removed automatically like Debug.WriteLine.  If hit in production
                // it will throw an exception.  Lesson learned years ago!
                Debugger.Break();
#endif
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a message describing the last window windows error. </summary>
        ///
        /// <value> A message describing the last window 32 error. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static string LastWin32ErrorMessage =>
            new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error()).Message;
    }
}
