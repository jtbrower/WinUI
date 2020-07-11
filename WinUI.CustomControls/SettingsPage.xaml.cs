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
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using System;
    using WinUI.Native;
    using WinUI.Vm;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <content>   The settings page. This class cannot be inherited. </content>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public sealed partial class SettingsPage : Page
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// I spent a rediculous amount of time making the mistake of trying to make the WindowHandle a
        /// dependency property.  I made that change with a bunch of other updates such that when the
        /// application started throwing exceptions I had no clue what it was having an issue with.  It
        /// did not like the IntPtr as it is an unsupported type.
        /// </summary>
        ///
        /// <value> The window handle. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private readonly IntPtr _windowHandle = IntPtr.Zero;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The view model property. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static readonly DependencyProperty VmProperty = DependencyProperty.Register(nameof(Vm),
            typeof(SettingsPageVm),
            typeof(SettingsPage),
            new PropertyMetadata(null));

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the view model. </summary>
        ///
        /// <value> The view model. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public SettingsPageVm? Vm
        {
            get => GetValue(VmProperty) as SettingsPageVm;
            set => SetValue(VmProperty, value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The window property. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static readonly DependencyProperty WindowProperty = DependencyProperty.Register(nameof(Window),
            typeof(Window),
            typeof(SettingsPage),
            new PropertyMetadata(null));

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the window. </summary>
        ///
        /// <value> The window. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public Window? Window
        {
            get => GetValue(WindowProperty) as Window;
            set => SetValue(WindowProperty, value);
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Initializes a new instance of the WinUI.CustomControls.SettingsPage class.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public SettingsPage()
        {
            //This constructor is to please the designer when one works with WinUI
            InitializeComponent();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes a new instance of the WinUI.CustomControls.SettingsPage class. </summary>
        ///
        /// <param name="vm">       The view model. </param>
        /// <param name="window">   The window. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public SettingsPage(SettingsPageVm vm, Window window) : this()
        {
            Vm = vm;
            Window = window;
            _windowHandle = window.GetHandle();
        }

        //It's annoying when this warning is applied to event handler parameters.
#pragma warning disable IDE0060 // Remove unused parameter

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by RemoveWindowBorder for click events. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void RemoveWindowBorder_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckHandleAsync()) return;

            NativeMethods.RemoveWindowBorder(_windowHandle);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by AddWindowBorder for click events. </summary>
        ///
        /// <exception cref="InvalidOperationException">    Thrown when the requested operation is
        ///                                                 invalid. </exception>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void AddWindowBorder_Click(object sender, RoutedEventArgs e)
        {
            if (Window == null)
                throw new InvalidOperationException();
            if (!CheckHandleAsync()) return;

            NativeMethods.AddWindowBorder(Window, _windowHandle);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by MaximizeWindow for click events. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void MaximizeWindow_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckHandleAsync()) return;

            NativeMethods.MaximizeWindow(_windowHandle);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by MinimizeWindow for click events. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void MinimizeWindow_Click(object sender, RoutedEventArgs e)

        {
            if (!CheckHandleAsync()) return;

            NativeMethods.MinimizeWindow(_windowHandle);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by RestoreWindow for click events. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void RestoreWindow_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckHandleAsync()) return;

            NativeMethods.RestoreWindow(_windowHandle);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by SetWindowTransparency for click events. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public async void SetWindowTransparency_Click(object sender, RoutedEventArgs e)
        {
            if (Vm == null) return;

            if (Vm.NumberBoxValue > 50)
            {
                if (!await Vm.DialogService.ConfirmAsync(
                    $"About Changing Transparency to {Vm.NumberBoxValue}",
                    $"A Window Transparency of {Vm.NumberBoxValue} could make it impossible to close the Window without a debugger.  Are you sure you want to continue?")) { return; }
            }
            if (!CheckHandleAsync()) return;

            NativeMethods.SetWindowTransparency(_windowHandle, Vm.NumberBoxValue);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by RemoveWindowTransparency for click events. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void RemoveWindowTransparency_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckHandleAsync()) return;

            NativeMethods.RemoveWindowTransparency(_windowHandle);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Determines if we can check handle asynchronous. </summary>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool CheckHandleAsync()
        {
            if (_windowHandle != IntPtr.Zero) return true;
            var _ = Vm?.DialogService.WarnAsync("WindowHandle", $"Hmm, the {nameof(_windowHandle)} property was null.  That's interesting, please let Jason Brower know about this.");
            return false;
        }
#pragma warning restore IDE0060 // Remove unused parameter
    }
}
