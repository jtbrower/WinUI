﻿// MIT License
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
    using System;
    using WinUI.Native;
    using System.Diagnostics;
    using System.Linq;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A window that attempts to mimic some of the features found in a WPF Window. </summary>
    ///
    /// <seealso cref="Microsoft.UI.Xaml.Window"/>
    /// <seealso cref="WinUI.CustomControls.IWpfWindow"/>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public sealed partial class WpfWindow : IWpfWindow
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the handle. </summary>
        ///
        /// <value> The handle. </value>
        ///
        /// <seealso cref="WinUI.CustomControls.IWpfWindow.Handle"/>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public IntPtr Handle { get; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes a new instance of the WinUI.DemoApp.WpfWindow class. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public WpfWindow()
        {
            InitializeComponent();

            Handle = this.GetHandle();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by RootGrid for double tapped events. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Double tapped routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void RootGrid_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            //I need a setting for this to enable or disable.
            Handle.MaximizeWindow();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the XAML root. </summary>
        ///
        /// <value> The XAML root. </value>
        ///
        /// <seealso cref="WinUI.CustomControls.IWpfWindow.XamlRoot"/>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public XamlRoot XamlRoot => RootGrid.XamlRoot;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// The Window.Content was redeclared as a new property so we could change the setter to prevent
        /// an unsuspecting victim from blowing over the top of the WindowRootGrid that this class
        /// depends on.  I will admit, it is slightly wonky because the XAML for the WpfWindow is setting 
        /// the content to our WindowRootGrid type.  BTW, I actually didn't want or need the XAML for this
        /// class but I stumbled upon an odd bug that I need to report.  I was unable to use the custom
        /// WindowRootGrid type in this code behind, it through access violations in native code, but if
        /// you use the same custom type in XAML it works fine.  I have not encountered anything like that
        /// before.
        /// 
        /// So there are at least two reasons why I took control over the Window.Content.  One was to 
        /// apply a DragMove() handler to it and the other was because I needed a custom Grid where I 
        /// could calculate the true size needed to fit the contents of the grid so that I could provide
        /// the WPF.SizeToContent feature.
        /// </summary>
        ///
        /// <value> The content. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        new public UIElement? Content
        {
            get => RootGrid.Children.FirstOrDefault();
            set=> SetContent(value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets a content. </summary>
        ///
        /// <param name="content">  The content. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SetContent(UIElement? content)
        {
            // The reason why I use a Grid is so that it will stretch out to fill the entire Window and the
            // event handlers for drag move can be attached to that. If the end user blows away the
            // WpfWindow content it will affect the drag move handlers.  So replacing the Window.Content
            // with this implementation lets me achieve those goals.
            RootGrid.Children.Clear();
            if (content == null) return;
            RootGrid.Children.Add(content);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Mimic WPF's SizeToContent feature.  Note that I have a current limitation where I do not account
        /// for the non-client space requirements of a Window so it only works if the Window Border and 
        /// Titlebar have already been removed.  
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SizeToContent()
        {
            if (!RootGrid.TrueDesiredSize.HasValue)
            {
                Debug.WriteLine($"{nameof(SizeToContent)} called before {nameof(RootGrid.TrueDesiredSize.HasValue)} had a value.");
                return;
            }
            var sizeRequired = RootGrid.TrueDesiredSize.Value;
            
            if (sizeRequired.Width <= 0 || sizeRequired.Height <= 0)
            {
                Debug.WriteLine($"An attempt to resize the Window with a (Width x Height) of {sizeRequired.Width}x{sizeRequired.Height}.");
                return;
            }

            //TODO : account for the Window's non-client space requirements.  

            //Convert to device units
            var heightInDevicePixels = RootGrid.DipToDevice(sizeRequired.Height);
            var widthInDevicePixels = RootGrid.DipToDevice(sizeRequired.Width);

            Handle.ResizeWindow(widthInDevicePixels, heightInDevicePixels);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   
        ///   When the end user needs to cast the Interface to a Window, they can call this 
        ///   method to avoid the warnings. 
        /// </summary>
        ///
        /// <returns>   A Window. </returns>
        ///
        /// <seealso cref="IWpfWindow.AsWindow()"/>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public Window AsWindow()=>this;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Removes the border. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void RemoveBorder()
        {
            this.RemoveWindowBorder(Handle);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Adds border. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void AddBorder()
        {
            this.AddWindowBorder(Handle);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Maximizes this.  </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Maximize()
        {
            Handle.MaximizeWindow();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Minimizes this.  </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Minimize()
        {
            Handle.MinimizeWindow();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Restores this. </summary>
        ///
        /// <seealso cref="IWpfWindow.Restore()"/>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Restore()
        {
            Handle.RestoreWindow();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Resizes. </summary>
        ///
        /// <param name="width">    The width. </param>
        /// <param name="height">   The height. </param>
        ///
        /// <seealso cref="IWpfWindow.Resize(int,int)"/>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Resize(int width, int height)
        {
            Handle.ResizeWindow(width, height);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets a transparency. </summary>
        ///
        /// <param name="percentTransparent">   The percent transparent. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SetTransparency(int percentTransparent)
        {
            Handle.SetWindowTransparency(percentTransparent);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Removes the transparency. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void RemoveTransparency()
        {
            Handle.RemoveWindowTransparency();
        }
    }
}
