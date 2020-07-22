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
namespace WinUI.CustomControls
{
    using Microsoft.Toolkit.Uwp.Helpers;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using System;
    using WinUI.Vm;

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

        public MainPageVm Vm
        {
            //The only reason this shows up is because I have to keep the constructor for the designer 
            // so that when WinUI does work with a designer someday it will be there.
#pragma warning disable CS8603 // Possible null reference return.
            get => GetValue(VmProperty) as MainPageVm;
#pragma warning restore CS8603 // Possible null reference return.
            private set => SetValue(VmProperty, value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The main window for the entire application. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private readonly IExtWindow _mainWindow;

        //Note that this is only needed for the Designer which always causes headaches with Nullable.  Since
        // the designer doesn't work with WinUI right now I just make the Constructor private.
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        private MainPage()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {
            //This constructor is to please the designer when one works with WinUI
            InitializeComponent();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes a new instance of the WinUI.CustomControls.MainPage class. </summary>
        ///
        /// <param name="vm">       The view model. </param>
        /// <param name="window">   The window. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public MainPage(MainPageVm vm, IExtWindow window)
        {
            InitializeComponent();
            Vm = vm;
            _mainWindow = window;
        }

        //It's annoying when this warning is applied to event handler parameters.
#pragma warning disable IDE0060 // Remove unused parameter

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by RemoveFauxTitleBar for click events. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void RemoveFauxTitleBar_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.ChangeFauxTitlebarVisibility(false);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by AddFauxTitleBar for click events. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void AddFauxTitleBar_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.ChangeFauxTitlebarVisibility(true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by ShrinkToContent for click events./ </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SizeToContent_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.SizeToContent();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by SetWindowTransparency for click events. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public async void SetWindowTransparency_Click(object sender, RoutedEventArgs e)
        {
            if (Vm.NumberBoxValue > 50)
            {
                if (!await Vm.DialogService.ConfirmAsync(
                    $"About Changing Transparency to {Vm.NumberBoxValue}",
                    $"A MainWindow Transparency of {Vm.NumberBoxValue} could make it impossible to close the MainWindow without a debugger.  Are you sure you want to continue?")) { return; }
            }
            _mainWindow.SetTransparency(Vm.NumberBoxValue);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by RemoveWindowTransparency for click events. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void RemoveWindowTransparency_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.RemoveTransparency();
        }

        //Ignoring this because they are applying it to event handlers too!
#pragma warning restore IDE0060 // Remove unused parameter
    }
}
