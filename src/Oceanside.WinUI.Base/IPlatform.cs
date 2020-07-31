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

namespace WinUI.CustomControls
{
    using Microsoft.UI.Xaml;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// I am placing platform specific needs in this file.  An example is a need for Dialogs to have
    /// access to the XamlRoot without tying them to some static member thats tough to test.  Since the
    /// Microsoft.UI.Xaml namespace is not .Net Standard compliant, it is a platform specific need.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public interface IPlatform
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the XAML root. </summary>
        ///
        /// <value> The XAML root. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        XamlRoot XamlRoot { get; }
    }
}
