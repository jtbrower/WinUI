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

namespace Oceanside.Win32.Native.Hooks
{
    using System;
    using System.Runtime.InteropServices;

    public sealed partial class WindowHookManager
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// This is the message that is passed to the public caller when a custom WndProc hook is installed.
        /// </summary>
        ///
        /// <seealso cref="System.IDisposable"/>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public class HookMessage
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Initializes a new instance of the Oceanside.Win32.Native.WindowHookManager.Message class.
            /// </summary>
            ///
            /// <param name="hWnd">     The h window. </param>
            /// <param name="msg">      The message. </param>
            /// <param name="wparam">   The w parameter. </param>
            /// <param name="lparam">   The l parameter. </param>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public HookMessage(IntPtr hWnd, HookMsgType msg, IntPtr wparam, IntPtr lparam)
            {
                HWnd = hWnd;
                HookMsgType = msg;
                WParam = wparam;
                LParam = lparam;
                Result = IntPtr.Zero;
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Gets or sets the window. </summary>
            ///
            /// <value> The h window. </value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public IntPtr HWnd { get; set; }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Gets or sets the type of the window 32 message. </summary>
            ///
            /// <value> The type of the window 32 message. </value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public HookMsgType HookMsgType { get; set; }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Gets or sets the parameter. </summary>
            ///
            /// <value> The w parameter. </value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public IntPtr WParam { get; set; }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Gets or sets the parameter. </summary>
            ///
            /// <value> The l parameter. </value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public IntPtr LParam { get; set; }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Gets or sets the result. </summary>
            ///
            /// <value> The result. </value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public IntPtr Result { get; set; }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Gets l parameter. </summary>
            ///
            /// <param name="cls">  The cls. </param>
            ///
            /// <returns>   The l parameter. </returns>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public object? GetLParam(Type cls)
            {
                return Marshal.PtrToStructure(LParam, cls);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Returns the fully qualified type name of this instance. </summary>
            ///
            /// <returns>   The fully qualified type name. </returns>
            ///
            /// <seealso cref="System.ValueType.ToString()"/>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public override string ToString()
            {
                return HookMsgType.ToString();
            }
        }
    }
}