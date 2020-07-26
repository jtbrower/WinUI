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
            new PropertyMetadata(null));

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
        /// This constructor should not be used in production.  It exists to please the Designer
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
        /// <summary>   Size ofwindowViewite. </summary>
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
