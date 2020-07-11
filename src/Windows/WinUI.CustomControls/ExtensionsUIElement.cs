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
    using Windows.Foundation;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   The extensions user interface element. </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    internal static class ExtensionsUIElement
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
            var scale = (float)element.XamlRoot.RasterizationScale;
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
            return (int)Math.Round(length*scale, MidpointRounding.AwayFromZero);
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
            var scale = (float)element.XamlRoot.RasterizationScale;
            return new Point(point.X / scale, point.Y / scale);
        }
    }
}
