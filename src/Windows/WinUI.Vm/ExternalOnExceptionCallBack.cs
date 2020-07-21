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
namespace WinUI.Vm
{
    using System;
    using System.Diagnostics;
    using System.Runtime.ExceptionServices;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Provides a way of handling exceptions without coupling this library to a logging framework or
    /// related interfaces.  Just set the static once and your free.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public static class ExternalOnExceptionCallBack
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets the on exception action.  Rather than include interfaces to libraries for the
        /// logging service, we using this Action to prevent coupling.
        /// </summary>
        ///
        /// <value> The s on exception action. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static Action<Exception>? OnExceptionAction { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Try notify of exception. </summary>
        ///
        /// <param name="e">    An Exception to process. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void TryNotifyOfException(Exception e)
        {
            if (OnExceptionAction == null)
            {
                Console.WriteLine(e.ToString());
#if DEBUG
                if (Debugger.IsAttached)
                    Debugger.Break();
#endif
                if (e is AggregateException aggregateException)
                {
                    e = aggregateException.Flatten();
                }
                ExceptionDispatchInfo.Capture(e.InnerException ?? e).Throw();
            }
            OnExceptionAction.Invoke(e);
        }
    }
}