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
    using Microsoft.Toolkit.Uwp.UI.Controls;
    using System.Linq;
    using Windows.Foundation;
    using WinUI.Vm;
    using Microsoft.UI.Xaml.Controls;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <content>   Container for window contents. This class cannot be inherited. </content>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public sealed partial class ContentRoot : LayoutTransformControl
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The view model property. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static readonly DependencyProperty VmProperty = DependencyProperty.Register(nameof(Vm),
            typeof(ContentRootVm),
            typeof(ContentRoot),
            new PropertyMetadata(null));

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the view model. </summary>
        ///
        /// <value> The view model. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public ContentRootVm? Vm
        {
            get => GetValue(VmProperty) as ContentRootVm;
            set => SetValue(VmProperty, value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// This constructor should not be used in production.  It exists to please the Designer
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public ContentRoot()
        {
            InitializeComponent();
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
        /// <summary>   Initializes a new instance of the WinUI.CustomControls.ContentRoot class. </summary>
        ///
        /// <param name="contentRootVm"> The window root view model. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public ContentRoot(ContentRootVm contentRootVm)
        {
            InitializeComponent();

            Vm = contentRootVm;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The content property. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(nameof(Content),
                typeof(FrameworkElement), typeof(ContentRoot), new PropertyMetadata(null, ContentPropertyChanged));

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the view model. </summary>
        ///
        /// <value> The view model. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public FrameworkElement? Content
        {
            get => GetValue(ContentProperty) as FrameworkElement;
            set => SetValue(ContentProperty, value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Content property changed. </summary>
        ///
        /// <param name="d">    A DependencyObject to process. </param>
        /// <param name="e">    Dependency property changed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        static void ContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ContentRoot contentRoot)) return;
            foreach (var child in contentRoot.ChildGrid.Children.Where(c => !c.Equals(contentRoot.TitleBarInstance)))
            {
                contentRoot.ChildGrid.Children.Remove(child);
            }

            if (!(e.NewValue is UIElement uiElement)) return;

            uiElement.SetValue(Grid.RowProperty, 1);
            contentRoot.ChildGrid.Children.Add(uiElement);
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Size of the infinite. </summary>
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
        /// <summary>
        /// If given infinity during a MeasureOverride, the grid desires this Size of space.
        /// </summary>
        ///
        /// <value> The size desired if given Positive Infinity to measure within. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public Size? TrueDesiredSize { get; private set; }

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
            //Use infinity to assure it returns the real space needs.
            TrueDesiredSize = base.MeasureOverride(_infiniteSize);
            return base.MeasureOverride(availableSize);
        }
    }
}
