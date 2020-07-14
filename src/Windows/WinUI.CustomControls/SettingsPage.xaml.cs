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
    using Microsoft.UI;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Hosting;
    using System;
    using System.Numerics;
    using WinUI.Vm;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <content>   The settings page. This class cannot be inherited. </content>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public sealed partial class SettingsPage : Page
    {
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

        public SettingsPageVm Vm
        {
            //The only reason this shows up is because I have to keep the constructor for the designer 
            // so that when WinUI does work with a designer someday it will be there.
#pragma warning disable CS8603 // Possible null reference return.
            get => GetValue(VmProperty) as SettingsPageVm;
#pragma warning restore CS8603 // Possible null reference return.
            private set => SetValue(VmProperty, value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The main window for the entire application. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private readonly IWpfWindow _mainWindow;

        //Note that this is only needed for the Designer which always causes headaches with Nullable.  Since
        // the designer doesn't work with WinUI right now I just make the Constructor private.
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        private SettingsPage()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
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

        public SettingsPage(SettingsPageVm vm, IWpfWindow window)
        {
            InitializeComponent();
            Vm = vm;
            _mainWindow = window;
            MainGrid.Loaded += MainGrid_Loaded;
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
            _mainWindow.RemoveBorder();
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
            _mainWindow.AddBorder();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by MaximizeWindow for click events. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void MaximizeWindow_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.Maximize();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by MainGrid for loaded events. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //What I am doing in this event handler is applying a drop shadow to the main content in a way
            // that a DropPanelShadow would work.  With the DropPanelShadow I noticed that it would not
            // Horizontally stretch (Vertical worked) and the shadow size does not properly report itself.
            // I had hoped I could figure out how to properly measure such that the shadow was included,
            // but it didn't work with this appraoch either.  I needed to know the total size of everything
            // inside of the MainGrid so that I could make the ShrinkToContent work properly.  I decided
            // to keep this code because it is good to know how to create a drop shadow without the use
            // of the UWP ToolKit.
            var compositor = ElementCompositionPreview.GetElementVisual(DropShadowCanvas).Compositor;
            var dropShadow = compositor.CreateDropShadow();
            dropShadow.Color = Colors.Black;
            dropShadow.BlurRadius = 4;
            dropShadow.Opacity = .5f;
            dropShadow.Offset = new Vector3(5F, 5F, 0);

            var mask = RectangleInstance.GetAlphaMask();
            dropShadow.Mask = mask;

            var spriteVisual = compositor.CreateSpriteVisual();
            spriteVisual.Size = new Vector2((float)DropShadowCanvas.ActualWidth, (float)DropShadowCanvas.ActualHeight);

            spriteVisual.Shadow = dropShadow;
            ElementCompositionPreview.SetElementChildVisual(DropShadowCanvas, spriteVisual);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by ShrinkToContent for click events./ </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SizeToContent_Click(object sender, RoutedEventArgs e)
        {
            //Note that for the time being, when I size to content I do not take into account 
            // the Windows non-client area so if SizeToContent is called without removing the
            // Non-Client space then it will be cramped.  For now, I just add this to 
            // remove confusion.
            _mainWindow.RemoveBorder();
            _mainWindow.SizeToContent();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by MinimizeWindow for click events. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void MinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.Minimize();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by RestoreWindow for click events. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void RestoreWindow_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.Restore();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by CloseWindow for click events. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.Close();
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
