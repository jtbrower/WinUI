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

using Oceanside.WinUI.Base.Vm;

namespace WinUI.DemoApp.Vm
{

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A ViewModel for the main page. </summary>
    ///
    /// <seealso cref="WinUI.Vm.PropChangeBase"/>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public sealed partial class MainPageVm : PropChangeBase
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Number of box values. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private int _windowTransparencyPercent;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The toggle non client visibility button view model. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private ToggleButtonVm<MainPageVm> _toggleNonClientVisibilityButtonVm;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The size to content button view model. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private ButtonVm<MainPageVm> _sizeToContentButtonVm;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The add window transparency button view model. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private ButtonVm<MainPageVm> _addWindowTransparencyButtonVm;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The remove window transparency button view model. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private ButtonVm<MainPageVm> _removeWindowTransparencyButtonVm;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The window that owns this item. </summary>
        ///
        /// <value> The owner window. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private IExtWindow OwnerWindow { get; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes a new instance of the WinUI.Vm.MainPageVm class. </summary>
        ///
        /// <param name="dialogService">    The dialog service. </param>
        /// <param name="ownerWindow">      The window that owns this item. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public MainPageVm(
            IDialogService dialogService,
            IExtWindow ownerWindow)
        {
            OwnerWindow = ownerWindow;
            DialogService = dialogService;

            _toggleNonClientVisibilityButtonVm =
                new ToggleButtonVm<MainPageVm>(ToggleNonClientVisibility, "Show Non-Client Area", true);

            _addWindowTransparencyButtonVm =
                new ButtonVm<MainPageVm>(async mp => await AddWindowTransparencyWithConfirmTask(mp), "Set Transparency");

            _removeWindowTransparencyButtonVm =
                new ButtonVm<MainPageVm>(RemoveWindowTransparency, "Remove Transparency",false);

            _sizeToContentButtonVm =
                new ButtonVm<MainPageVm>(SizeToContent, "Size to Content");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the remove window transparency button view model. </summary>
        ///
        /// <value> The remove window transparency button view model. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public ButtonVm<MainPageVm> RemoveWindowTransparencyButtonVm
        {
            get => _removeWindowTransparencyButtonVm;
            set => SetProperty(ref _removeWindowTransparencyButtonVm, value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the size to content button view model. </summary>
        ///
        /// <value> The size to content button view model. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public ButtonVm<MainPageVm> SizeToContentButtonVm
        {
            get => _sizeToContentButtonVm;
            set => SetProperty(ref _sizeToContentButtonVm, value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the add window transparency button view model. </summary>
        ///
        /// <value> The add window transparency button view model. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public ButtonVm<MainPageVm> AddWindowTransparencyButtonVm
        {
            get => _addWindowTransparencyButtonVm;
            set => SetProperty(ref _addWindowTransparencyButtonVm, value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the toggle non client visibility button view model. </summary>
        ///
        /// <value> The toggle non client visibility button view model. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public ToggleButtonVm<MainPageVm> ToggleNonClientVisibilityButtonVm
        {
            get => _toggleNonClientVisibilityButtonVm;
            set => SetProperty(ref _toggleNonClientVisibilityButtonVm, value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the window transparency percent. </summary>
        ///
        /// <value> The window transparency percent. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public int WindowTransparencyPercent
        {
            get => _windowTransparencyPercent;
            set => SetProperty(ref _windowTransparencyPercent, value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the dialog service. </summary>
        ///
        /// <value> The dialog service. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public IDialogService DialogService { get; }

    }
}
