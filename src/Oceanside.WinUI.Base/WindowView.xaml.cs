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

namespace WinUI.CustomControls
{
    using Microsoft.UI.Xaml;
    using Microsoft.Toolkit.Uwp.UI.Controls;
    using Windows.Foundation;
    using WinUI.Vm;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <content>   Container for window contents. This class cannot be inherited. </content>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public sealed partial class WindowView : LayoutTransformControl
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The view model property. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static readonly DependencyProperty VmProperty = DependencyProperty.Register(nameof(Vm),
            typeof(WindowVm),
            typeof(WindowView),
            new PropertyMetadata(null, (sender, args) =>
            {
                if (!(sender is WindowView windowView)) return;

                AttachViewModelEventHandlers(windowView,args);

            }));

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Attach view model event handlers. </summary>
        ///
        /// <param name="windowView">   The window view. </param>
        /// <param name="args">         Dependency property changed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static void AttachViewModelEventHandlers(WindowView windowView, DependencyPropertyChangedEventArgs args)
        {
            //We listen to a couple event handlers on the view model that fire when this WindowView class
            // needs to perform an action such as Showing/Hiding the DropShadow or scaling content.
            if (args.OldValue is WindowVm oldWindowVm)
            {
                //We need to remove the handlers or else we could cause a memory leak.
                oldWindowVm.DropShadowVisibilityChangeRequested -= windowView.WindowVm_DropShadowVisibilityChangeRequested;
                oldWindowVm.ScaleContentRequested -= windowView.WindowVm_ScaleContentRequested;
            }
            if (args.NewValue is WindowVm newWindowVm)
            {
                //Just in case we somehow have handlers attached, remove them first.  No exceptions are thrown 
                // if we were not attached.
                newWindowVm.DropShadowVisibilityChangeRequested -= windowView.WindowVm_DropShadowVisibilityChangeRequested;
                newWindowVm.ScaleContentRequested -= windowView.WindowVm_ScaleContentRequested;

                //Attach
                newWindowVm.DropShadowVisibilityChangeRequested += windowView.WindowVm_DropShadowVisibilityChangeRequested;
                newWindowVm.ScaleContentRequested += windowView.WindowVm_ScaleContentRequested;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Window view model scale content requested. </summary>
        ///
        /// <param name="sender">   The sender. </param>
        /// <param name="scaleAt">  The scale at. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void WindowVm_ScaleContentRequested(object? _, double scaleAt)
        {
            ScaleContent(scaleAt);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Window view model drop shadow visibility change requested. </summary>
        ///
        /// <param name="sender">       The sender. </param>
        /// <param name="isVisible">    True if is visible, false if not. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void WindowVm_DropShadowVisibilityChangeRequested(object? _, bool isVisible)
        {
            if (isVisible)
                ChildGridDropShadow.ShowDropShadow();
            else 
                ChildGridDropShadow.HideDropShadow();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the view model. </summary>
        ///
        /// <value> The view model. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public WindowVm? Vm
        {
            get => GetValue(VmProperty) as WindowVm;
            set => SetValue(VmProperty, value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// This constructor should not be used in production.  It exists to please the Designer.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public WindowView()
        {
            InitializeComponent();

            //I don't like binding to statics like this so I pulled it out of the DataTemplate Selector
            // and brought it up close to the top where it could more easily be injected.  I started by
            // creating an interface called IApplicationStatics but I learned that it actually exists!
            // So I will just leave it at this for now.
            TypeNameDataTemplateSelector.FallbackResourceDictionary = Application.Current?.Resources;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Hides the drop shadow. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void HideDropShadow()
        {
            ChildGridDropShadow.HideDropShadow();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Shows the drop shadow. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ShowDropShadow()
        {
            ChildGridDropShadow.ShowDropShadow();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Infinity Size. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static readonly Size _infiniteSize = new Size(double.PositiveInfinity, double.PositiveInfinity);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Scale content. </summary>
        ///
        /// <param name="scaleAt">  The scale at. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ScaleContent(double scaleAt)
        {
            ContentScaleTransform.ScaleX = scaleAt;
            ContentScaleTransform.ScaleY = scaleAt;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The required height property. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static readonly DependencyProperty RequiredHeightProperty =
            DependencyProperty.Register(nameof(RequiredHeight),
                typeof(double),
                typeof(WindowView),
                new PropertyMetadata(0, (sender, args) =>
                {
                    if (!(sender is WindowView windowView)) return;
                    if (windowView.Vm == null) return;
                    windowView.Vm.RequiredHeight = args.NewValue == null ? 0 : (double)args.NewValue;
                }));

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the height of the required. </summary>
        ///
        /// <value> The height of the required. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public double RequiredHeight
        {
            get => (double)GetValue(RequiredHeightProperty);
            set => SetValue(RequiredHeightProperty, value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the width of the required. </summary>
        ///
        /// <value> The width of the required. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public double RequiredWidth
        {
            get => (double)GetValue(RequiredWidthProperty);
            set => SetValue(RequiredWidthProperty, value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The required width property. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static readonly DependencyProperty RequiredWidthProperty =
            DependencyProperty.Register(
                nameof(RequiredWidth),
                typeof(double),
                typeof(WindowView),
                new PropertyMetadata(0, (sender, args) =>
                {
                    if (!(sender is WindowView windowView)) return;
                    if (windowView.Vm == null) return;
                    windowView.Vm.RequiredWidth = args.NewValue == null ? 0 : (double)args.NewValue;
                }));

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Provides the behavior for the "Measure" pass of the layout cycle. We override this so that we
        /// can determine how much space all of the controls truly needed.  This information is useful
        /// when trying to emulate the WPF Window.SizeToContent feature.
        /// </summary>
        ///
        /// <param name="availableSize">    The available size that this object can give to child
        ///                                 objects. Infinity can be specified as a value to indicate that
        ///                                 the object will size to whatever content is available. </param>
        ///
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the
        /// allocated sizes for child objects or based on other considerations such as a fixed container
        /// size.
        /// </returns>
        ///
        /// <seealso cref="Microsoft.UI.Xaml.FrameworkElement.MeasureOverride(Size)"/>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override Size MeasureOverride(Size availableSize)
        {
            //Needed to support the SizeToContent feature.
            UpdateRequiredSize();

            //Now allow everything to determine what they actually want to ask for.
            return base.MeasureOverride(availableSize);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Updates the required size. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void UpdateRequiredSize()
        {
            //Use infinity to assure it returns the real space needs.  We then set our custom dependency 
            // property values so we know how much space to allocate when we resize the window to fit 
            // the content.  Note that using the DesiredSize property will not tell you the correct 
            // answer.
            var trueDesiredSize = base.MeasureOverride(_infiniteSize);
            SetValue(RequiredHeightProperty, trueDesiredSize.Height);
            SetValue(RequiredWidthProperty, trueDesiredSize.Width);
        }
    }
}
