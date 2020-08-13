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

namespace Oceanside.WinUI.Base
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Media;
    using System;
    using System.Collections.Generic;
    using Windows.Foundation;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   The extensions user interface element. </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public static class ExtensionsUIElement
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   An UIElement extension method that dip point to device point. </summary>
        ///
        /// <param name="element">  The operation. </param>
        /// <param name="point">    The transform from device. </param>
        ///
        /// <returns>   A Point. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        internal static Point DipToDevice(this UIElement element, Point point)
        {
            var scale = element.XamlRoot.RasterizationScale;
            return new Point(point.X * scale, point.Y * scale);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   An UIElement extension method that dip point to device point. </summary>
        ///
        /// <param name="element">  The operation. </param>
        /// <param name="length">   The length. </param>
        ///
        /// <returns>   A Point. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        internal static int DipToDevice(this UIElement element, double length)
        {
            var scale = element.XamlRoot.RasterizationScale;
            return (int)Math.Round(length * scale, MidpointRounding.AwayFromZero);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Device point to dip point. </summary>
        ///
        /// <param name="element">  The operation. </param>
        /// <param name="point">    The transform from device. </param>
        ///
        /// <returns>   A PortablePoint. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        internal static Point DeviceToDip(this UIElement element, Point point)
        {
            var scale = element.XamlRoot.RasterizationScale;
            return new Point(point.X / scale, point.Y / scale);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the ancestors in this collection. </summary>
        ///
        /// <param name="dependencyObject"> The dependencyObject to act on. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the ancestors in this collection.
        /// </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static IEnumerable<DependencyObject> GetAncestors(this DependencyObject dependencyObject)
        {
            var parent = VisualTreeHelper.GetParent(dependencyObject);

            while (parent != null)
            {
                yield return parent;
                parent = VisualTreeHelper.GetParent(parent);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   An UIElement extension method that query if 'child' is descendant of. </summary>
        ///
        /// <param name="child">    The child to act on. </param>
        /// <param name="of">       The of. </param>
        ///
        /// <returns>   True if descendant of, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static bool IsDescendantOf(this UIElement child, UIElement of)
        {
            if (of == null || child == null) return false;

            DependencyObject parent;
            do
            {
                parent = VisualTreeHelper.GetParent(child);
                if (of == parent) return true;
            } while (parent != null);
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   An UIElement extension method that gets bounding rectangle. </summary>
        ///
        /// <exception cref="InvalidOperationException">    Thrown when the requested operation is
        ///                                                 invalid. </exception>
        ///
        /// <param name="uiElement">    The uiElement to act on. </param>
        /// <param name="window">       The window. </param>
        ///
        /// <returns>   The bounding rectangle. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static Rect GetBoundingRect(this UIElement uiElement, Window window)
        {
            //The rectangle will be relative to the Window's content because the Window itself is
            // not a UIElement.
            var windowContent = window.Content;

            if (uiElement == windowContent)
                return new Rect(0, 0, windowContent.ActualSize.X, windowContent.ActualSize.Y);

            if (!uiElement.IsDescendantOf(windowContent))
                throw new InvalidOperationException($"{nameof(window)} does not contain {nameof(uiElement)}.");

            var windowContentPoint = uiElement.TransformToVisual(windowContent).TransformPoint(new Point());

            var bottomRight = new Point(uiElement.ActualSize.X, uiElement.ActualSize.Y);
            var uiElementPoint = uiElement.TransformToVisual(windowContent).TransformPoint(bottomRight);

            return new Rect(windowContentPoint, uiElementPoint);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// If you keep walking up to the very first Framework element attached to the Window you will
        /// find that it is many layers above the Window.Content.  This function iterates starting at
        /// Window.Content and keeps walking until it has found the topmost FrameworkElement in the XAML
        /// tree.
        /// </summary>
        ///
        /// <param name="window">   The window. </param>
        ///
        /// <returns>   The root element. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static FrameworkElement? GetRootElement(this Window window)
        {
            return window.Content is not FrameworkElement fe ? null : fe.GetRootElement();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Keep walking up the XAML tree using the FrameworkElement.Parent property until null is hit.
        /// </summary>
        ///
        /// <param name="element">  The operation. </param>
        ///
        /// <returns>   The root element. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static FrameworkElement? GetRootElement(this FrameworkElement element)
        {
            var parent = element;
            while (parent != null)
            {
                if (!(parent.Parent is FrameworkElement fe)) return parent;
                parent = fe;
            }
            return null;
        }
    }
}
