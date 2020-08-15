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
    /// <summary>   A ViewModel for the window. </summary>
    ///
    /// <seealso cref="WinUI.Vm.PropChangeBase"/>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class WindowVm : PropChangeBase
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Wired into the View to request a change in the drop shadow's visibility. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public event EventHandler<bool>? DropShadowVisibilityChangeRequested;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Wired into the View to request that the views content be scaled. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public event EventHandler<double>? ScaleContentRequested;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The title bar view model. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private TitleBarVm? _titleBarVm;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The window that owns this item. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private IExtWindow _ownerWindow;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The clients content. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private object? _clientsContent;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Height of the required. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private double _requiredHeight;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Width of the required. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private double _requiredWidth;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes a new instance of the WinUI.Vm.WindowVm class. </summary>
        ///
        /// <param name="titleBarVm">   The title bar view model. </param>
        /// <param name="ownerWindow">  The window that owns this item. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public WindowVm(TitleBarVm titleBarVm, IExtWindow ownerWindow)
        {
            _ownerWindow = ownerWindow;
            _titleBarVm = titleBarVm;

            _titleBarVm.CloseWindowCmd = new DelegateCmd(_ownerWindow.Close);
            _titleBarVm.MinimizeWindowCmd = new DelegateCmd(_ownerWindow.Minimize);
            _titleBarVm.MaximizeWindowCmd = new DelegateCmd(_ownerWindow.Maximize);
            _titleBarVm.RestoreWindowCmd = new DelegateCmd(_ownerWindow.Restore);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the clients content. </summary>
        ///
        /// <value> The clients content. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public object? ClientsContent
        {
            get => _clientsContent;
            set => SetProperty(ref _clientsContent, value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the title bar view model. </summary>
        ///
        /// <value> The title bar view model. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public TitleBarVm TitleBarVm { get => _titleBarVm; set => SetProperty(ref _titleBarVm, value); }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the height of the required. </summary>
        ///
        /// <value> The height of the required. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public double RequiredHeight
        {
            get => _requiredHeight;
            set => SetProperty(ref _requiredHeight, value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the width of the required. </summary>
        ///
        /// <value> The width of the required. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public double RequiredWidth { get => _requiredWidth; set => SetProperty(ref _requiredWidth, value); }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Shows the cursor. </summary>
        ///
        /// <param name="shouldShow">   True if should show. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ShowCursor(bool shouldShow)
        {
            _ownerWindow.ShowCursor(shouldShow);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Shows the drop shadow. </summary>
        ///
        /// <param name="shouldShow">   True if should show. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ShowDropShadow(bool shouldShow)
        {
            DropShadowVisibilityChangeRequested?.Invoke(this,shouldShow);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Scale content. </summary>
        ///
        /// <param name="scaleAt">  The scale at. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ScaleContent(double scaleAt)
        {
            ScaleContentRequested?.Invoke(this,scaleAt);
        }
    }
}
