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
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Media;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A type name data template selector. This class cannot be inherited. </summary>
    ///
    /// <seealso cref="Microsoft.UI.Xaml.Controls.DataTemplateSelector"/>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public sealed class TypeNameDataTemplateSelector : DataTemplateSelector
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets a dictionary of fallback resources to use as a search path when all other
        /// locations have been exhausted.  Typically Application.Current.Resources should be used.  This
        /// property helps avoid coupling the DataTemplateSelector to the static Application.Current
        /// property and makes it easy to test as a result.  Statics are bad, mmmkay.
        /// </summary>
        ///
        /// <value> A dictionary of fallback resources. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public ResourceDictionary? FallbackResourceDictionary { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Select template core. </summary>
        ///
        /// <param name="item">         The item. </param>
        /// <param name="container">    The container. </param>
        ///
        /// <returns>   A DataTemplate? </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override DataTemplate? SelectTemplateCore(object item, DependencyObject container)
        {
            //Application.Current could be null in testing, that's why I am checking it here.
            if (item == null || !(container is FrameworkElement frameworkElement)) return default;

            return FindDataTemplate(item.GetType().Name, frameworkElement, FallbackResourceDictionary);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Walks up the XAML tree checking the resources of a framework element and then that elements
        /// parental ancestors until a DataTemplate with the given key is found.  As a last resort, this
        /// function will.
        /// </summary>
        ///
        /// <param name="key">                  The key. </param>
        /// <param name="element">              The element. </param>
        /// <param name="fallbackResources">    The fallback resources to search if the template is not
        ///                                     found on the element or any of its parents.  I recommend
        ///                                     using Application.Current.Resources. </param>
        ///
        /// <returns>   The found data template. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static DataTemplate? FindDataTemplate(
            string key,
            FrameworkElement element,
            ResourceDictionary? fallbackResources = null)
        {
            var parent = element;
            object? resourceValue;

            //Recursive While Loop
            do
            {
                if (parent.Resources.TryGetValue(key, out resourceValue)) return resourceValue as DataTemplate;
                parent = VisualTreeHelper.GetParent(element) as FrameworkElement;
            } while (parent != null);

            //If no fallback was provided then we are done.
            if (fallbackResources == null) return default;

            //Fallback is typically Application.Current.Resources
            _ = fallbackResources.TryGetValue(key, out resourceValue);
            return resourceValue as DataTemplate;
        }
    }
}