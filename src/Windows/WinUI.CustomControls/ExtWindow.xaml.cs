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
    using WinUI.Native.Hooks;
    using System.Diagnostics;
    using Windows.Graphics.Display;
    using WinUI.Vm;
    using static PInvoke.User32;
    using Microsoft.UI.Xaml.Input;
    using Microsoft.UI.Xaml.Media;
    using System.Linq;
    using Microsoft.UI.Xaml.Controls.Primitives;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using PInvoke;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <content>
    /// A window that attempts to mimic some of the features found in a WPF Window.
    /// </content>
    /// <seealso cref="Microsoft.UI.Xaml.Window"/>
    /// <seealso cref="WinUI.CustomControls.IExtWindow"/>
    /// <seealso cref="WinUI.CustomControls.IPlatform"/>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public sealed partial class ExtWindow : IExtWindow, IPlatform, INotifyPropertyChanged
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The window procedure callback. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private readonly WindowHookManager.WndProcCallback _wndProcCallback;

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
        /// Initializes a new instance of the WinUI.CustomControls.ExtWindow class.
        /// </summary>
        ///
        /// <param name="windowView">   The window root. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public ExtWindow()
        {
            InitializeComponent();

            //I don't like binding to statics like this so I pulled it out of the DataTemplate Selector
            // and brought it up close to the top where it could more easily be injected.  I started by
            // creating an interface called IApplicationStatics but I learned that it actually exists!
            // So I will just leave it at this for now.
            TypeNameDataTemplateSelector.FallbackResourceDictionary = Application.Current?.Resources;

            //Grab the handle and remove the built-in Win32 TitleBar and border
            Handle = this.GetHandle();
            Handle.HideWin32NonClientArea();

            //This is how you hook WndProc using the helper classes.  The helper classes will unhook
            // when you close the Window.
            _wndProcCallback = new WindowHookManager.WndProcCallback(WndProcCallback);
            WindowHookManager.AddWndProcCallback(Handle, _wndProcCallback);

            //A custom handler we fire when minimized/maximized and other states are entered.
            WindowStateChanged += ExtWindow_WindowStateChanged;

            RootContainer.DoubleTapped += RootContainer_DoubleTapped;
            RootContainer.Loaded += RootContainer_Loaded;
            RootContainer.Unloaded += RootContainer_Unloaded;

            SizeChanged += ExtWindow_SizeChanged;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Callback, called when the window procedure. </summary>
        ///
        /// <param name="message">  [in,out] The message. </param>
        ///
        /// <returns>   An int. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool WndProcCallback(ref WindowHookManager.HookMessage message)
        {
            //Debug.WriteLine(message.Msg);
            
            //If you do not handle the message then return false
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Occurs when a property value changes. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public event PropertyChangedEventHandler? PropertyChanged;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The view model. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private WindowVm? _vm;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the view model. </summary>
        ///
        /// <value> The view model. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public WindowVm? Vm
        {
            //Notice that our Window's view model cannot be a DependencyProperty because a WinUI Window is
            // not a DependencyObject.  For  that reason we must using INotifyPropertyChanged.
            get { return _vm; }
            set
            {
                if(_vm == value)return;
                _vm = value;
                OnPropertyChanged();                
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the property changed action. </summary>
        ///
        /// <param name="propertyName"> (Optional) Name of the property. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler == null) return;
            //Lock to prevent garbage collection before we invoke
            lock (handler)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Extent window state changed. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="state">        An EnumWindowState to process. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ExtWindow_WindowStateChanged(object? sender, EnumWindowState state)
        {
            //If there is a TitleBar then assure the IsMaximized flag is updated.
            var titleBar = Vm?.TitleBarVm;
            if (titleBar == null) return;
            titleBar.IsMaximized = state == EnumWindowState.Maximized;
        }

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
                var windowPlacement = GetWindowPlacement(Handle);
                var showStyle = windowPlacement.showCmd;

                if (showStyle == WindowShowStyle.SW_MINIMIZE ||
                    showStyle == WindowShowStyle.SW_FORCEMINIMIZE ||
                    showStyle == WindowShowStyle.SW_SHOWMINIMIZED ||
                    showStyle == WindowShowStyle.SW_SHOWMINNOACTIVE) return EnumWindowState.Minimized;


                if (showStyle == WindowShowStyle.SW_MAXIMIZE ||
                    showStyle == WindowShowStyle.SW_SHOWMAXIMIZED) return EnumWindowState.Maximized;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return EnumWindowState.Other;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by RootContainer for unloaded events. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void RootContainer_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_displayInformation == null) return;
            _displayInformation.DpiChanged -= DisplayInfo_DpiChanged;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by RootContainer for loaded events. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void RootContainer_Loaded(object sender, RoutedEventArgs e)
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
            if (ClientContentControl.XamlRoot == null)
            {
                Debug.WriteLine($"{nameof(GetDpiScale)} called before {nameof(WindowView)}.XamlRoot was available.");
                return null;
            }
            return (ClientContentControl.XamlRoot.RasterizationScale * 96.0) / GetDpiForWindow(Handle);
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
        /// <summary>   Event handler. Called by RootContainer for double tapped events. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="args">     Double tapped routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void RootContainer_DoubleTapped(object sender, DoubleTappedRoutedEventArgs args)
        {
            //Grab the position that was double tapped and see if there are any types in the path of 
            // type ButtonBase.  If so then do not allow the double tap of a button to turn into 
            // a Window Maximize or Restore because its confusing to the end user.
            var doubleTapPosition = args.GetPosition(RootContainer);

            //This method already checks for IsHitTestVisible by default.
            var uiElementsUnderPosition = VisualTreeHelper.FindElementsInHostCoordinates(doubleTapPosition, ClientContentControl);
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

        public XamlRoot XamlRoot => RootContainer.XamlRoot;

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
            if (Vm == null)
            {
                Debug.WriteLine($"{nameof(ScaleWindowContent)} found {nameof(ExtWindow)}.{nameof(Vm)} is null.  Unable to scale.");
                return;
            }
            Vm.ScaleContent(scaleAt);
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
            if(Vm == null)
            {
                Debug.WriteLine($"{nameof(SizeToContent)} found {nameof(ExtWindow)}.{nameof(Vm)} is null.  Unable to resize.");
                return;
            }

            if (Vm.RequiredHeight <= 0 || Vm.RequiredHeight == double.PositiveInfinity || Vm.RequiredWidth <= 0 || Vm.RequiredWidth == double.PositiveInfinity)
            {
                Debug.WriteLine($"{nameof(SizeToContent)} found {nameof(Vm.RequiredWidth)}x{nameof(Vm.RequiredHeight)} = {Vm.RequiredWidth}x{Vm.RequiredHeight}.  Unable to resize.");
                return;
            }

            //Convert to device units
            var clientHeightInDevicePixels = ClientContentControl.DipToDevice(Vm.RequiredHeight);
            var clientWidthInDevicePixels = ClientContentControl.DipToDevice(Vm.RequiredWidth);

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
            if (Vm == null) return;

            //If the current state matches the requested leave.
            if (Vm.TitleBarVm.IsVisible == isVisible) return;

            //Removing or showing the DropShadow is somewhat similar to removing or showing the Win32 
            // Window border
            Vm.ShowDropShadow(isVisible);

            Vm.TitleBarVm.IsVisible = isVisible;

            //Force the content to recompute its space requirements
            Content?.UpdateLayout();

            //If we hid the non-client area then there will be extra empty space around the Window's Content
            // that we can remove with this call.  If we are adding the non-client area back in, this call
            // assures it has enough space to render.
            SizeToContent();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Shows the cursor. </summary>
        ///
        /// <param name="makeVisible">  True to show, false to hide the make. </param>
        ///
        /// <seealso cref="IExtWindow.ShowCursor(bool)"/>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ShowCursor(bool makeVisible)
        {
            User32.ShowCursor(makeVisible);
        }
    }
}
