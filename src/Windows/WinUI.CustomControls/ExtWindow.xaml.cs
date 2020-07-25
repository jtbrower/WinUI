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
    using System;
    using WinUI.Native;
    using System.Diagnostics;
    using Windows.Graphics.Display;
    using Microsoft.UI.Xaml.Controls;
    using WinUI.Vm;
    using PInvoke;
    using Microsoft.UI.Xaml.Input;
    using Microsoft.UI.Xaml.Media;
    using System.Linq;
    using Microsoft.UI.Xaml.Controls.Primitives;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <content>
    /// A window that attempts to mimic some of the features found in a WPF Window.
    /// </content>
    /// <seealso cref="Microsoft.UI.Xaml.Window"/>
    /// <seealso cref="WinUI.CustomControls.IExtWindow"/>
    /// <seealso cref="WinUI.CustomControls.IPlatform"/>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public sealed partial class ExtWindow : IExtWindow, IPlatform
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   True to automatically DPI content scaling. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool _autoDpiContentScaling = true;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// When the Window's content is first loaded, we save the DPI scale and attempt to make the
        /// content look like it does on the first monitor it is loaded on.  Anytime the end user moves
        /// this window to another screen, the DPI changed event will fire and we will scale the Window's
        /// content accordingly.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private double? _dpiScaleOnLoaded;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Information describing the display that needs to be saved so the DPI changed handler can be
        /// detached.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private DisplayInformation? _displayInformation;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Occurs when Window State Changed. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public event EventHandler<EnumWindowState>? WindowStateChanged;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Note that I am going to make a best attempt at trying to keep this in sync with the actual
        /// Win32 windows Show State but I am cautious in that this could be a tough challenge to meet in
        /// all edge cases.  The primary purpose of state track is to help assure that we know whether to
        /// show the Maximize or Restore button in the TitleBar.  So be cautious until this is hardened
        /// through use.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public EnumWindowState _cachedWindowState = EnumWindowState.Other;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the handle. </summary>
        ///
        /// <value> The handle. </value>
        ///
        /// <seealso cref="WinUI.CustomControls.IExtWindow.Handle"/>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public IntPtr Handle { get; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Should not be called by production code, this exists only to please the designer.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public ExtWindow()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {
            InitializeComponent();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Initializes a new instance of the WinUI.CustomControls.ExtWindow class.
        /// </summary>
        ///
        /// <param name="contentRoot">   The window root. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public ExtWindow(ContentRoot contentRoot) : this()
        {
            InitializeComponent();

            //ContentRoot is the top level representation of the customizable client content.
            ContentRoot = contentRoot;

            //Grab the handle and remove the TitleBar and border
            Handle = this.GetHandle();
            Handle.HideWin32NonClientArea();

            //If the content has a TitelBar then assign the event handlers for the buttons
            var titleBarVm = ContentRoot.Vm?.TitleBarVm;
            if (titleBarVm != null)
            {
                titleBarVm.CloseWindowCmd = new DelegateCmd(Close);
                titleBarVm.MinimizeWindowCmd = new DelegateCmd(Minimize);
                titleBarVm.MaximizeWindowCmd = new DelegateCmd(Maximize);
                titleBarVm.RestoreWindowCmd = new DelegateCmd(Restore);
            }

            WindowStateChanged += ExtWindow_WindowStateChanged;


            RootContainer.Children.Add(ContentRoot);
            RootContainer.DoubleTapped += ContentRoot_DoubleTapped;


            ContentRoot.Loaded += ContentRoot_Loaded;
            ContentRoot.Unloaded += ContentRoot_Unloaded;


            SizeChanged += ExtWindow_SizeChanged;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Extent window state changed. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        An EnumWindowState to process. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ExtWindow_WindowStateChanged(object? sender, EnumWindowState e)
        {
            //If there is a TitleBar then assure the IsMaximized flag is updated.
            var titleBar = ContentRoot?.Vm?.TitleBarVm;
            if (titleBar == null) return;
            titleBar.IsMaximized = e == EnumWindowState.Maximized;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the root grid. </summary>
        ///
        /// <value> The root grid. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public ContentRoot ContentRoot { get; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by ExtWindow for size changed events. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="args">     Window size changed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ExtWindow_SizeChanged(object sender, WindowSizeChangedEventArgs args)
        {
            //The Window size changed, if this also means a state change (for example, from Minimized to
            // Maximized) then we fire the vent handler.
            var currentWindowState = GetWindowState();
            if (_cachedWindowState == currentWindowState) return;
            _cachedWindowState = currentWindowState;
            WindowStateChanged?.Invoke(this, _cachedWindowState);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// An IntPtr extension method that gets window state representing minimized, maximized or other.
        /// </summary>
        ///
        /// <param name="windowHandle"> Handle of the window. </param>
        ///
        /// <returns>   The window state. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private EnumWindowState GetWindowState()
        {
            //This command will throw a Win32 exception if something goes wrong.  Prevent the exception and just return
            // false, but assure its written to the Debug pipe.  I was going to put this entire method in the
            // NativeMethods library, but I wanted the EnumWindowStyle to be available in the View Models library
            // without coupling the view models to NativeMethods.  I also didn't want to couple native methods
            // to the view model library.  That said, I could either duplicate the type and cause more confusion
            // or put the method here.
            try
            {
                var windowPlacement = User32.GetWindowPlacement(Handle);
                var showStyle = windowPlacement.showCmd;

                if (showStyle == User32.WindowShowStyle.SW_MINIMIZE ||
                    showStyle == User32.WindowShowStyle.SW_FORCEMINIMIZE ||
                    showStyle == User32.WindowShowStyle.SW_SHOWMINIMIZED ||
                    showStyle == User32.WindowShowStyle.SW_SHOWMINNOACTIVE) return EnumWindowState.Minimized;


                if (showStyle == User32.WindowShowStyle.SW_MAXIMIZE ||
                    showStyle == User32.WindowShowStyle.SW_SHOWMAXIMIZED) return EnumWindowState.Maximized;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return EnumWindowState.Other;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by ContentRoot for unloaded events. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ContentRoot_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_displayInformation == null) return;
            _displayInformation.DpiChanged -= DisplayInfo_DpiChanged;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by ContentRoot for loaded events. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ContentRoot_Loaded(object sender, RoutedEventArgs e)
        {
            _displayInformation = DisplayInformation.GetForCurrentView();
            _displayInformation.DpiChanged += DisplayInfo_DpiChanged;
            //The current scale is captured and used as a comparison for the times when the DPI changes.
            _dpiScaleOnLoaded = GetDpiScale();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets DPI scale. </summary>
        ///
        /// <returns>   The DPI scale. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private double? GetDpiScale()
        {
            if (ContentRoot.XamlRoot == null)
            {
                Debug.WriteLine($"{nameof(GetDpiScale)} called before {nameof(ContentRoot)}.XamlRoot was available.");
                return null;
            }
            return (ContentRoot.XamlRoot.RasterizationScale * 96.0) / User32.GetDpiForWindow(Handle);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Scale content for DPI. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ScaleContentForDpi()
        {
            var scale = GetDpiScale();
            //I saw this hit infinity once when closing a window, it caused an exception.  
            if (
                !scale.HasValue || double.IsInfinity(scale.Value) || scale <= 0 ||
                scale == _dpiScaleOnLoaded)
            {
                scale = 1.0;
            }
            ScaleWindowContent(scale.Value);
        }

        //It's annoying when this warning is applied to event handler parameters.
#pragma warning disable IDE0060 // Remove unused parameter

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

            //Lesson Learned : note that we recommend using the custom TitleBar or your own version of it
            // instead of the built-in Win32 Window TitleBar and Border.  Trying to properly handle DPI
            // changed events while also using the Win32 TitleBar, brings on more corner case risk than
            // what it is worth.  The primary problem is the fact that when you have an application that
            // is DPI Aware and the framework automatically scales the TitelBar for you, it will shrink 
            // and expand, causing your client area to either not have enough space (when you are using
            // our SizeToContent feature its most noticeable) or you will have slightly too much space.
            //
            // Initially I thought the solution would be easy.  When the DPI changed and the TitleBar 
            // scaled under its own power, I would simply resize the Window to either add to or take away
            // from the total Window size.  However, this causes a pretty nasty bug where the DPI changed
            // event will keep firing, causing infinite recursion and the only way out of it is a task
            // manager kill.  If your using Visual Studio when this happens, you won't even have control
            // over Visual Studio.  It does not always happen, but it can be caused by dragging the 
            // Window slowly between two Displays where the DPI is different between the two.  For example
            // you might have one monitor scaled at 150% and the other at 100%.  This is most likely to 
            // cause it.  
            //
            // If you use the custom TitleBar that I provided, you do not need to worry about this
            // problem because I scale the TitleBar along with the client content so that it's uniform.
            // Using this approach you will have 100% control over the look and feel of your Window's.
            // From the content, the border, DropShadow to the TitleBar, you can easily change all  of
            // it and scale it on a DPI change without worrying how the TitleBar scaled.
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by ContentRoot for double tapped events. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Double tapped routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ContentRoot_DoubleTapped(object sender, DoubleTappedRoutedEventArgs args)
        {
            //Grab the position that was double tapped and see if there are any types in the path of 
            // type ButtonBase.  If so then do not allow the double tap of a button to turn into 
            // a Window Maximize or Restore because its confusing to the end user.
            var doubleTapPosition = args.GetPosition(ContentRoot);

            //This method already checks for IsHitTestVisible by default.
            var uiElementsUnderPosition = VisualTreeHelper.FindElementsInHostCoordinates(doubleTapPosition, ContentRoot);
            if (uiElementsUnderPosition.Any(e => e.GetType().IsSubclassOf(typeof(ButtonBase)))) return;

            if (Handle.IsMaximized())
            {
                Handle.RestoreWindow();
            }
            else
            {
                //I need a setting for this to enable or disable.
                Handle.MaximizeWindow();
            }
        }
        //Ignoring this because they are applying it to event handlers too!
#pragma warning restore IDE0060 // Remove unused parameter

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the XAML root. </summary>
        ///
        /// <value> The XAML root. </value>
        ///
        /// <seealso cref="WinUI.CustomControls.IExtWindow.XamlRoot"/>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public XamlRoot XamlRoot => ContentRoot.XamlRoot;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// The Window.Content was redeclared as a new property so we could change the setter to prevent
        /// an unsuspecting victim from blowing over the top of the ContentRoot that this class
        /// depends on.  I will admit, it is slightly wonky because the XAML for the ExtWindow is setting 
        /// the content to our ContentRoot type.  BTW, I actually didn't want or need the XAML for this
        /// class but I stumbled upon an odd bug that I need to report.  I was unable to use the custom
        /// ContentRoot type in this code behind, it through access violations in native code, but if
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

        new public FrameworkElement? Content
        {
            // The reason why I use a Grid is so that it will stretch out to fill the entire Window and the
            // event handlers for drag move can be attached to that. If the end user blows away the
            // ExtWindow content it will affect the drag move handlers.  So replacing the Window.Content
            // with this implementation lets me achieve those goals.
            get => ContentRoot.Content;
            set => SetContent(value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets a content. </summary>
        ///
        /// <param name="clientsContent">   The clients content. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SetContent(FrameworkElement? clientsContent)
        {
            //Determine what to use for the content alignment.
            if (clientsContent != null)
            {
                var horizontalContentAlignment = clientsContent.HorizontalAlignment;
                var verticalContentAlignment = clientsContent.VerticalAlignment;
                if (clientsContent is Control control)
                {
                    horizontalContentAlignment = control.HorizontalContentAlignment;
                    verticalContentAlignment = control.VerticalContentAlignment;
                }

                ContentRoot.HorizontalAlignment = clientsContent.HorizontalAlignment;
                ContentRoot.VerticalAlignment = clientsContent.VerticalAlignment;
                ContentRoot.HorizontalContentAlignment = horizontalContentAlignment;
                ContentRoot.VerticalContentAlignment = verticalContentAlignment;
            }
            ContentRoot.Content = clientsContent;
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
            ContentRoot.ScaleContent(scaleAt);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Mimic WPF's SizeToContent feature.  Note that I have a current limitation where I do not account
        /// for the non-client space requirements of a Window so it only works if the Window Border and 
        /// TitleBar have already been removed.  
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SizeToContent()
        {
            if (!ContentRoot.TrueDesiredSize.HasValue)
            {
                Debug.WriteLine($"{nameof(SizeToContent)} called before {nameof(ContentRoot.TrueDesiredSize)} had a value.  Unable to resize.");
                return;
            }

            //This value tells us the truth about how much space is needed to fully render the content in the grid.
            var sizeRequired = ContentRoot.TrueDesiredSize.Value;

            //If it lied to us then don't use it :)
            if (sizeRequired.Width <= 0 || sizeRequired.Height <= 0)
            {
                Debug.WriteLine($"An attempt to resize the Window with a (Width x Height) of {sizeRequired.Width}x{sizeRequired.Height}.");
                return;
            }

            //Convert to device units
            var clientHeightInDevicePixels = ContentRoot.DipToDevice(sizeRequired.Height);
            var clientWidthInDevicePixels = ContentRoot.DipToDevice(sizeRequired.Width);

            //Now we need to calculate the non-client area of the Window.  This is only important when a Window has a border and or a
            // TitleBar (or menu).  In other words, if the HideNonClientArea method has been called on the Window then the nonClientSize
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
        /// <seealso cref="IExtWindow.AsWindow()"/>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public Window AsWindow() => this;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Hides the window 32 non client area. </summary>
        ///
        /// <seealso cref="IExtWindow.HideWin32NonClientArea()"/>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void HideWin32NonClientArea()
        {
            Handle.HideWin32NonClientArea();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Shows the window 32 non client area. </summary>
        ///
        /// <seealso cref="IExtWindow.ShowWin32NonClientArea()"/>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ShowWin32NonClientArea()
        {
            this.ShowWin32NonClientArea(Handle);
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
        /// <seealso cref="IExtWindow.Restore()"/>
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
        /// <seealso cref="IExtWindow.Resize(int,int)"/>
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Enables the automatic scale on DPI change. </summary>
        ///
        /// <seealso cref="IExtWindow.EnableAutoScaleOnDpiChange()"/>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void EnableAutoScaleOnDpiChange()
        {
            SetAutoDpiContentScaling(true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Disables the automatic scale on DPI change. </summary>
        ///
        /// <seealso cref="IExtWindow.DisableAutoScaleOnDpiChange()"/>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void DisableAutoScaleOnDpiChange()
        {
            SetAutoDpiContentScaling(false);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets a value indicating whether the automatic DPI content scaling is enabled.  This
        /// will scale content without making it appear blurry as happens with a non DPI aware application.
        /// </summary>
        ///
        /// <value> True if enable automatic DPI content scaling, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SetAutoDpiContentScaling(bool enable)
        {
            if (_autoDpiContentScaling == enable) return;
            _autoDpiContentScaling = enable;

            //In this case the window has not been activated for the first time.
            if (!_dpiScaleOnLoaded.HasValue) return;

            //Disabled
            if (!(_autoDpiContentScaling && _dpiScaleOnLoaded.HasValue))
            {
                //This will remove any transform that has been added.
                ScaleWindowContent(1.0);
            }
            else
            {
                //See if the scale has changed since the window was first activated.  If it is different
                // then we need to apply a transform.
                var currentScale = GetDpiScale();
                if (!currentScale.HasValue) return;
                ScaleWindowContent(currentScale.Value);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Hides the custom non client area. </summary>
        ///
        /// <seealso cref="IExtWindow.HideCustomNonClientArea()"/>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void HideCustomNonClientArea()
        {
            ChangeCustomNonClientAreaVisibility(false);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Shows the custom non client area. </summary>
        ///
        /// <seealso cref="IExtWindow.ShowCustomNonClientArea()"/>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ShowCustomNonClientArea()
        {
            ChangeCustomNonClientAreaVisibility(true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Change custom non client area visibility. </summary>
        ///
        /// <param name="isVisible">    True if is visible, false if not. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ChangeCustomNonClientAreaVisibility(bool isVisible)
        {
            if (ContentRoot.Vm == null) return;

            //If the current state matches the requested leave.
            if (ContentRoot.Vm.TitleBarVm.IsVisible == isVisible) return;

            //Removing or showing the DropShadow is somewhat similar to removing or showing the Win32 
            // Window border
            if (isVisible)
                ContentRoot.ShowDropShadow();
            else
                ContentRoot.HideDropShadow();


            ContentRoot.Vm.TitleBarVm.IsVisible = isVisible;

            //Force the content to recompute its space requirements
            base.Content?.UpdateLayout();

            //If we hid the non-client area then there will be extra empty space around the Window's Content
            // that we can remove with this call.  If we are adding the non-client area back in, this call
            // assures it has enough space to render.
            SizeToContent();
        }
    }
}
