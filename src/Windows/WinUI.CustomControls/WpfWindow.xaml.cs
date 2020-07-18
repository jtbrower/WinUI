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
using Microsoft.UI.Xaml;
using System;
using WinUI.Native;
using System.Diagnostics;
using System.Linq;
using Windows.Graphics.Display;
using Microsoft.UI.Xaml.Media;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.Foundation;
using Microsoft.UI.Xaml.Controls;

namespace WinUI.CustomControls
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A window that attempts to mimic some of the features found in a WPF Window. </summary>
    ///
    /// <seealso cref="Microsoft.UI.Xaml.Window"/>
    /// <seealso cref="WinUI.CustomControls.IWpfWindow"/>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public sealed partial class WpfWindow : IWpfWindow
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets a value indicating whether the automatic DPI content scaling is enabled.  This
        /// will scale content without making it appear blury as happens with a non DPI aware application.
        /// </summary>
        ///
        /// <value> True if enable automatic DPI content scaling, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SetAutoDpiContentScaling(bool enable)
        {
            if (_autoDpiContentScaling == enable) return;
            _autoDpiContentScaling = enable;
        }

        private bool _autoDpiContentScaling = true;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// When the Window's content is first loaded, we save the DPI scale and attempt to make the
        /// content look like it does on the first monitor it is loaded on.  Anytime the end user moves
        /// this window to another screen, the DPI changed event will fire and we will scale the Window's
        /// content accordingly.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private double? _dpiScaleOnFirstActivation;

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
            Activated += WpfWindow_Activated;
            RootGrid.Loaded += RootGrid_Loaded;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by RootGrid for loaded events. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void RootGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var displayInfo = DisplayInformation.GetForCurrentView();
            displayInfo.DpiChanged += DisplayInfo_DpiChanged;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by WpfWindow for activated events. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="args">     Window activated event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void WpfWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            //We only needed to be notified on the first activation.
            Activated -= WpfWindow_Activated;

            //The current scale is captured and used as a comparison for the times when the DPI changes.
            _dpiScaleOnFirstActivation = GetDpiScale();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets DPI scale. </summary>
        ///
        /// <returns>   The DPI scale. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private double GetDpiScale()
        {
            return (RootGrid.XamlRoot.RasterizationScale * 96.0) / PInvoke.User32.GetDpiForWindow(Handle);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Scale content for DPI. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ScaleContentForDpi()
        {
            var scale = GetDpiScale();
            //I saw this hit inifinity once when closing a window, it caused an exception.  
            if (double.IsInfinity(scale) || scale <= 0 || scale == _dpiScaleOnFirstActivation) scale = 1.0;
            ScaleWindowContent(scale);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Displays an information DPI changed. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="args">     The arguments. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void DisplayInfo_DpiChanged(DisplayInformation sender, object args)
        {
            if (!_autoDpiContentScaling) return;

            ScaleContentForDpi();
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
            // The reason why I use a Grid is so that it will stretch out to fill the entire Window and the
            // event handlers for drag move can be attached to that. If the end user blows away the
            // WpfWindow content it will affect the drag move handlers.  So replacing the Window.Content
            // with this implementation lets me achieve those goals.
            get => RootGrid.Children.FirstOrDefault();
            set => SetContent(value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets a content. </summary>
        ///
        /// <param name="content">  The content. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SetContent(UIElement? content)
        {
            //Grab the current.
            var currentContent = RootGrid.Children.FirstOrDefault();

            //Clear current
            if (currentContent != null)
                RootGrid.Children.RemoveAt(0);

            //Add the new
            if (content != null)
                RootGrid.Children.Add(content);

            //With a content change, we need to make sure we did not attach a LayoutTransformControl.
            // If we did, its probably best to disassociate the Conent from this control.
            if (currentContent is LayoutTransformControl ltc) ltc.Child = null;

            //If the scale is not the same as it was when this control was first loaded, then
            // apply a transform.
            var currentScale = GetDpiScale();
            if (_dpiScaleOnFirstActivation.HasValue && _dpiScaleOnFirstActivation != currentScale)
                ScaleWindowContent(currentScale);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Note that this gives me the power to handle DPI changes as the end user moves this window
        /// from the DPI it was loaded at to another DPI.  As that DPI changes we attempt to scale the
        /// content to account for the size differences the new DPI would cause.  I accomplish that by
        /// adding a scale transform to the Content.
        /// </summary>
        ///
        /// <param name="scaleAt">  The scale to use on the Window's content. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ScaleWindowContent(double scaleAt)
        {
            //Grab the current.
            var currentContent = RootGrid.Children.FirstOrDefault();
            if (currentContent == null) return;

            //If there is already a transform applied then use the existing.
            if (currentContent is LayoutTransformControl lt)
            {
                //If it is a 1 to 1 scale then we could just set it to 1 but instead I am 
                // removing the LayoutTransformControl.  If you find this causes a jerky
                // UI experience then try leaving the Transform in place and just setting
                // it to 1.
                if(scaleAt == 1.0)
                {
                    var child = lt.Child;
                    RootGrid.Children.RemoveAt(0);
                    lt.Child = null;
                    RootGrid.Children.Add(child);
                }
                //See if we need to update the existing scale.
                else if (lt.Transform is ScaleTransform st && st.ScaleX != scaleAt && st.ScaleX != scaleAt)
                {
                    st.ScaleX = scaleAt;
                    st.ScaleY = scaleAt;
                    return;
                }
                return;
            }

            //The layout transform control requires a FrameworkElement.
            if (!(currentContent is FrameworkElement fe)) return;

            //This is the scale magic
            var scaleTransform = new ScaleTransform
            {
                CenterX = .5,
                CenterY = .5,
                ScaleX = scaleAt,
                ScaleY = scaleAt
            };

            //We already know we have a Child because we called FirstOrDefault and checked for null above.
            RootGrid.Children.RemoveAt(0);

            //Determine what to use for the content alignment.
            var horizontalContentAlignment = fe.HorizontalAlignment;
            var verticalContentAlignment = fe.VerticalAlignment;
            if(fe is Control control)
            {
                horizontalContentAlignment = control.HorizontalContentAlignment;
                verticalContentAlignment = control.VerticalContentAlignment;
            }
            
            //Wrap the content with a transform
            var layoutTransformControl = new LayoutTransformControl
            {
                RenderTransformOrigin = new Point(.5f, .5f),
                HorizontalAlignment = fe.HorizontalAlignment,
                VerticalAlignment = fe.VerticalAlignment,
                HorizontalContentAlignment = horizontalContentAlignment,
                VerticalContentAlignment = verticalContentAlignment,
                Child = fe,
                Transform = scaleTransform
            };
            //Add it back to the grid.
            RootGrid.Children.Add(layoutTransformControl);
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

            //Convert to device units
            var clientHeightInDevicePixels = RootGrid.DipToDevice(sizeRequired.Height);
            var clientWidthInDevicePixels = RootGrid.DipToDevice(sizeRequired.Width);

            //Now we need to calculate the non-client area of the Window.  This is only important when a Window has a border and or a
            // titlebar (or menu).  In other words, if the RemoveWindowBorder method has been called on the Window then the nonClientSize
            // should be zero.
            if (!Handle.GetNonClientSize(out var nonClientSize))
            {
                Debug.WriteLine($"Unable to determine the non-client size needed in {nameof(SizeToContent)}");
                return;
            }

            Handle.ResizeWindow((int)(clientWidthInDevicePixels + nonClientSize.Width), (int)(clientHeightInDevicePixels + nonClientSize.Height));
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

        public Window AsWindow() => this;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Removes the border. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void RemoveBorder()
        {
            Handle.RemoveWindowBorder();
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
