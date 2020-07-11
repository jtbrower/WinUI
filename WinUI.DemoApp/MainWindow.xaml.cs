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
namespace WinUI.DemoApp
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using WinUI.CustomControls;

    public sealed partial class MainWindow : IMainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public XamlRoot XamlRoot => Content.XamlRoot;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// The reason why I use a Grid is so that it will stretch out to fill the entire Window and the
        /// event handlers for drag move can be attached to that. If the end user blows away the
        /// MainWindow content it will affect the drag move handlers.  So as a quick fix I provided this 
        /// method.
        /// </summary>
        ///
        /// <param name="page"> The page. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SetPage(Page page)
        {
            MainGrid.Children.Add(page);
        }
    }
}
