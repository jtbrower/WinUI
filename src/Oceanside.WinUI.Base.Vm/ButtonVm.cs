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

namespace Oceanside.WinUI.Base.Vm
{
    using System;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A ViewModel for the button. </summary>
    ///
    /// <typeparam name="T">    Generic type parameter. </typeparam>
    ///
    /// <seealso cref="WinUI.Vm.PropChangeBase"/>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class ButtonVm<T> : PropChangeBase
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The display content. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private object _displayContent;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   True if is visible, false if not. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool _isVisible = true;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   True if is enabled, false if not. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool _isEnabled = true;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes a new instance of the WinUI.Vm.ButtonVm{T} class. </summary>
        ///
        /// <param name="clickedCmd">       The 'clicked' command. </param>
        /// <param name="displayContent">   The display content. </param>
        /// <param name="isEnabled">        (Optional) True if is enabled, false if not. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public ButtonVm(Action<T> clickedCmd, object displayContent, bool isEnabled=true)
        {
            ClickedCmd=new DelegateCmd<T>(clickedCmd);
            _displayContent = displayContent;
            _isEnabled = isEnabled;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the 'clicked' command. </summary>
        ///
        /// <value> The 'clicked' command. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public DelegateCmd<T> ClickedCmd { get; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the display content. </summary>
        ///
        /// <value> The display content. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public object DisplayContent
        {
            get => _displayContent;
            set => SetProperty(ref _displayContent, value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a value indicating whether this  is visible. </summary>
        ///
        /// <value> True if this  is visible, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a value indicating whether this  is enabled. </summary>
        ///
        /// <value> True if this  is enabled, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }
    }
}