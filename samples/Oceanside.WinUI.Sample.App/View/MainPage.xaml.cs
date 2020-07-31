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

namespace WinUI.DemoApp.View
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using WinUI.DemoApp.Vm;
 
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <content>   A main page. This class cannot be inherited. </content>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public sealed partial class MainPage : Page
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The view model property. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static readonly DependencyProperty VmProperty = DependencyProperty.Register(nameof(Vm),
            typeof(MainPageVm),
            typeof(MainPage),
            new PropertyMetadata(null));

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the view model. </summary>
        ///
        /// <value> The view model. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public MainPageVm? Vm
        {
            get => GetValue(VmProperty) as MainPageVm;
            set => SetValue(VmProperty, value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes a new instance of the WinUI.CustomControls.MainPage class. </summary>
        ///
        /// <param name="vm">   The view model. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public MainPage()
        {
            InitializeComponent();
        }
    }
}
