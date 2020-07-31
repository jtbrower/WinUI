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

namespace WinUI.CustomControls.Behaviors
{
    using Microsoft.UI;
    using Microsoft.UI.Composition;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Hosting;
    using Microsoft.UI.Xaml.Media;
    using Microsoft.UI.Xaml.Shapes;
    using Microsoft.Xaml.Interactivity;
    using System;
    using System.Diagnostics;
    using System.Numerics;
    using Windows.UI;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A rounded drop shadow behavior. </summary>
    ///
    /// <seealso cref="Microsoft.Xaml.Interactivity.Behavior{Microsoft.UI.Xaml.FrameworkElement}"/>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class RoundedDropShadowBehavior : Behavior<FrameworkElement>
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Hides the drop shadow. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void HideDropShadow()
        {
            _dropShadowLogic?.HideDropShadow();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Shows the drop shadow. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ShowDropShadow()
        {
            _dropShadowLogic?.ShowDropShadow();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The corner radius property. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius),
                typeof(double),
                typeof(RoundedDropShadowBehavior),
                new PropertyMetadata(0.0));

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the shadow size. </summary>
        ///
        /// <value> The size of the shadow. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public double ShadowSize
        {
            get => (double)GetValue(ShadowSizeProperty);
            set => SetValue(ShadowSizeProperty, value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The shadow size property. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static readonly DependencyProperty ShadowSizeProperty =
            DependencyProperty.Register(
                nameof(ShadowSize), typeof(double), 
                typeof(RoundedDropShadowBehavior), 
                new PropertyMetadata(5.0));

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the opacity. </summary>
        ///
        /// <value> The opacity. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public double Opacity
        {
            get => (double)GetValue(OpacityProperty);
            set => SetValue(OpacityProperty, value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The opacity property. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static readonly DependencyProperty OpacityProperty =
            DependencyProperty.Register(
                nameof(Opacity), 
                typeof(double), 
                typeof(RoundedDropShadowBehavior), 
                new PropertyMetadata(.6));

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the blur radius. </summary>
        ///
        /// <value> The blur radius. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public double BlurRadius
        {
            get => (double)GetValue(BlurRadiusProperty);
            set => SetValue(BlurRadiusProperty, value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The blur radius property. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static readonly DependencyProperty BlurRadiusProperty =
            DependencyProperty.Register(
                nameof(BlurRadius), 
                typeof(double), 
                typeof(RoundedDropShadowBehavior), 
                new PropertyMetadata(4.0));

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the color of the shadow. </summary>
        ///
        /// <value> The color of the shadow. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public Color ShadowColor
        {
            get => (Color)GetValue(ShadowColorProperty);
            set => SetValue(ShadowColorProperty, value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The shadow color property. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static readonly DependencyProperty ShadowColorProperty =
            DependencyProperty.Register(
                nameof(ShadowColor), 
                typeof(Color), 
                typeof(RoundedDropShadowBehavior), 
                new PropertyMetadata(Colors.Black));

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the view model. </summary>
        ///
        /// <value> The view model. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public double CornerRadius
        {
            get => (double)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The drop shadow logic. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private DropShadowLogic? _dropShadowLogic;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Called after the behavior is attached to the
        /// <see cref="P:Microsoft.Xaml.Interactivity.Behavior.AssociatedObject" />.
        /// </summary>
        ///
        /// <remarks>
        /// Override this to hook up functionality to the
        /// <see cref="P:Microsoft.Xaml.Interactivity.Behavior.AssociatedObject" />
        /// </remarks>
        ///
        /// <seealso cref="Behavior{Microsoft.UI.Xaml.FrameworkElement}.OnAttached()"/>
        ///
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void OnAttached()
        {
            AssociatedObject.Loaded += AssociatedObject_Loaded;
            base.OnAttached();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by AssociatedObject for loaded events. </summary>
        ///
        /// <exception cref="InvalidOperationException">    Thrown when the requested operation is
        ///                                                 invalid. </exception>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(AssociatedObject.Parent is Panel parentPanel))
                throw new InvalidOperationException
                    ($"{nameof(RoundedDropShadowBehavior)} must be attached to a FrameworkElement with a Parent of type Panel");

            _dropShadowLogic = new DropShadowLogic(parentPanel, AssociatedObject);

            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            _dropShadowLogic?.InsertDropShadow(ShadowSize, CornerRadius, Opacity, BlurRadius, ShadowColor);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Called when the behavior is being detached from its
        /// <see cref="P:Microsoft.Xaml.Interactivity.Behavior.AssociatedObject" />.
        /// </summary>
        ///
        /// <remarks>
        /// Override this to unhook functionality from the
        /// <see cref="P:Microsoft.Xaml.Interactivity.Behavior.AssociatedObject" />
        /// </remarks>
        ///
        /// <seealso cref="Microsoft.Xaml.Interactivity.Behavior.OnDetaching()"/>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void OnDetaching()
        {
            try
            {
                _dropShadowLogic?.RemoveDropShadow();
            }
            //I would not want cleanup work to cause an exception.  There is nothing we can do, but change
            // the design if we see this occurring in development.
            catch (Exception e)
            {
                Debug.WriteLine($"{nameof(OnDetaching)} caused the following exception : {e}");
            }
            base.OnDetaching();
        }

        #region Logic for Drop Shadow

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   A drop shadow logic. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private class DropShadowLogic
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   The drop shadow host. </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            private Canvas? _dropShadowHost;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   The rounded rectangle mask. </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            private Rectangle? _roundedRectMask;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   The parent. </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            private readonly Panel _parent;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   The sprite visual. </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            private SpriteVisual? _spriteVisual;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   The element to shadow. </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            private readonly FrameworkElement _elementToShadow;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   The size animation expression. </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            private ExpressionAnimation? _sizeAnimationExpression;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Sometimes we must modify the original margin of the element being shadowed.  If the end user
            /// hides the shadow we need to use this saved value to revert any changes made to the original
            /// value.
            /// </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            private Thickness? _elementsOriginalMargin;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   The elements modified margin. </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            private Thickness? _elementsModifiedMargin;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Initializes a new instance of the
            /// WinUI.CustomControls.RoundedDropShadowBehavior.DropShadowLogic class.
            /// </summary>
            ///
            /// <param name="parent">           The parent. </param>
            /// <param name="elementToShadow">  The element to shadow. </param>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public DropShadowLogic(Panel parent,
                FrameworkElement elementToShadow)
            {
                _parent = parent;
                _elementToShadow = elementToShadow;
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Removes the drop shadow. </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            internal void RemoveDropShadow()
            {
                //Remove the elements that we added to the _parent.Children
                try
                {
                    if (_parent == null) return;

                    //Hiding the shadow will take care of a few steps needed to completely remove
                    // the shadow.
                    HideDropShadow();

                    var childCollection = _parent.Children;

                    if (childCollection.Contains(_roundedRectMask))
                        childCollection.Remove(_roundedRectMask);

                    if (childCollection.Contains(_dropShadowHost))
                        childCollection.Remove(_dropShadowHost);

                    _roundedRectMask = null;
                    _dropShadowHost = null;
                    _elementsOriginalMargin = null;
                }
                //I would not want cleanup work to cause an exception.  There is nothing we need to do, but
                // we developers need to know if it is happening during development.
                catch (Exception e)
                {
                    Debug.WriteLine($"{nameof(RemoveDropShadow)} caused the following exception : {e}");
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Hides the drop shadow. </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public void HideDropShadow()
            {
                _dropShadowHost?.SetValue(UIElement.VisibilityProperty, Visibility.Collapsed);
                _roundedRectMask?.SetValue(UIElement.VisibilityProperty, Visibility.Collapsed);

                if (_elementsOriginalMargin != null)
                    _elementToShadow.Margin = _elementsOriginalMargin.Value;

                if (_spriteVisual == null) return;

                _spriteVisual.StopAnimation("Size");
                _spriteVisual.IsVisible = false;

            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Shows the drop shadow. </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public void ShowDropShadow()
            {
                _dropShadowHost?.SetValue(UIElement.VisibilityProperty, Visibility.Visible);
                _roundedRectMask?.SetValue(UIElement.VisibilityProperty, Visibility.Visible);

                if (_elementsModifiedMargin != null)
                    _elementToShadow.Margin = _elementsModifiedMargin.Value;

                if (_spriteVisual == null) return;

                _spriteVisual.StartAnimation("Size", _sizeAnimationExpression);
                _spriteVisual.IsVisible = true;
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Given a parent Panel, this function will apply a drop shadow around a chosen child element of
            /// the panel.  At the time I wrote this function, the Windows Community ToolKit still did not
            /// provide a way to render rounded corners on the shadow, this function does.
            /// </summary>
            ///
            /// <exception cref="InvalidOperationException">    Thrown when the requested operation is
            ///                                                 invalid. </exception>
            ///
            /// <param name="shadowSize">           (Optional) Size of the shadow. </param>
            /// <param name="shadowCornerRadius">   (Optional) The shadow corner radius. </param>
            /// <param name="opacity">              (Optional) The opacity. </param>
            /// <param name="blurRadius">           (Optional) The blur radius. </param>
            /// <param name="shadowColor">          (Optional) The shadow color. </param>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            internal void InsertDropShadow(
                double shadowSize = 5.0,
                double shadowCornerRadius = 10.0,
                double opacity = .6,
                double blurRadius = 4.0,
                Color? shadowColor = null)
            {
                if (_dropShadowHost != null)
                    throw new InvalidOperationException(
                        $"{nameof(InsertDropShadow)} already attached a drop shadow.");

                //I choose to make a custom shadow over using the UWP ToolKit DropShadowPanel because the UWP
                // version does not have rounded corners (it has the properties but they do not work).
                var contentVisual = ElementCompositionPreview.GetElementVisual(_elementToShadow);
                var compositor = contentVisual.Compositor;

                //Create the shadow
                var dropShadow = compositor.CreateDropShadow();
                dropShadow.Color = shadowColor ?? Colors.Black;
                dropShadow.BlurRadius = (float)blurRadius;
                dropShadow.Opacity = (float)opacity;

                //Both the shadow margin and offset are based on the callers shadow size
                dropShadow.Offset = new Vector3((float)shadowSize, (float)shadowSize, 0);
                var requiredMargin = new Thickness(0, 0, shadowSize, shadowSize);

                _dropShadowHost = new Canvas() { Margin = requiredMargin };
                _parent.Children.Insert(0, _dropShadowHost);

                #region Assure Margin For Shadow
                //Important : after we attach the drop shadow, it will make the size of the control being shadowed
                // bigger, but when the control is measure, the size will appear the same.  That means that if you
                // use my other SizeToContent feature, it will not calculate the shadow size and it will be clipped.
                // For this reason, we modify the margin of the element being shadowed to assure that the bottom and
                // right margins have enough space for the shadowSize.  Some of you control experts probably know a
                // better way to do this?  It feels like there has to be a better way.
                var leftMargin = _elementToShadow.Margin.Left;
                var rightMargin = _elementToShadow.Margin.Right >= 
                    shadowSize ? _elementToShadow.Margin.Right : shadowSize;
                var bottomMargin = _elementToShadow.Margin.Bottom >= 
                    shadowSize ? _elementToShadow.Margin.Bottom : shadowSize;
                var topMargin = _elementToShadow.Margin.Top;

                //Save the current margin and apply the new one
                _elementsOriginalMargin = _elementToShadow.Margin;
                _elementsModifiedMargin = new Thickness(leftMargin, topMargin, rightMargin, bottomMargin);
                _elementToShadow.Margin = _elementsModifiedMargin.Value;
                #endregion

                //If the caller wants the shadow to have rounded corners, we need to perform this additional logic.
                if (shadowCornerRadius > 0.0)
                {
                    //The rectangle is used as a mask to round the corners.
                    _roundedRectMask = new Rectangle
                    {
                        RadiusX = shadowCornerRadius,
                        RadiusY = shadowCornerRadius,
                        Margin = requiredMargin,
                        Fill = shadowColor.HasValue ? new SolidColorBrush(shadowColor.Value) : new SolidColorBrush(Colors.Black)
                    };
                    //Bind both the height and width to the host properties so resizing does not cause problems.
                    _roundedRectMask.SetBinding(FrameworkElement.WidthProperty, new Microsoft.UI.Xaml.Data.Binding()
                    {
                        // TODO figure out why output indicates Canvas.ActualWidthProperty does not exist Note that the
                        // Debug output will warn that ActualWidthProperty does not exist on Canvas.  It cannot be
                        // correct because if I change it to anything else (or remove it) the drop shadow will not be
                        // visible.
                        Path = new PropertyPath(nameof(FrameworkElement.ActualWidthProperty)),
                        Source = _dropShadowHost
                    });
                    _roundedRectMask.SetBinding(FrameworkElement.HeightProperty, new Microsoft.UI.Xaml.Data.Binding()
                    {
                        // TODO figure out why output indicates Canvas.ActualHeightProperty does not exist As I stated
                        // above, debug will warn that the ActualHeightProperty does not exist, but that cannot be
                        // correct.
                        Path = new PropertyPath(nameof(FrameworkElement.ActualHeightProperty)),
                        Source = _dropShadowHost
                    });

                    _parent.Children.Insert(0, _roundedRectMask);

                    //Grab the mask and assign it to the drop shadow.  This will cause the rounded corners.
                    dropShadow.Mask = _roundedRectMask.GetAlphaMask();
                }

                //Create the sprite visual and attach the shadow
                _spriteVisual = compositor.CreateSpriteVisual();
                _spriteVisual.Size = contentVisual.Size;
                _spriteVisual.CenterPoint = contentVisual.CenterPoint;
                _spriteVisual.Shadow = dropShadow;

                //Attach the shadow
                ElementCompositionPreview.SetElementChildVisual(_dropShadowHost, _spriteVisual);

                //A little anal about refactor proofing, but I refactor constantly.
                var animatedObjectName = nameof(contentVisual);

                //Assure that the shadow sizing follows the item it is shadowing
                _sizeAnimationExpression = compositor.CreateExpressionAnimation($"{animatedObjectName}.Size");
                _sizeAnimationExpression.SetReferenceParameter(animatedObjectName, contentVisual);
                _spriteVisual.StartAnimation("Size", _sizeAnimationExpression);
            }
        }
        #endregion
    }
}
