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
using Microsoft.UI.Xaml;
using WinUI.Vm;

namespace WinUI.CustomControls
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <content>   A title bar. This class cannot be inherited. </content>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public sealed partial class TitleBar
    {
        public static readonly DependencyProperty VmProperty = DependencyProperty.Register(nameof(Vm),
            typeof(TitleBarVm),
            typeof(TitleBar),
            new PropertyMetadata(null));

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the view model. </summary>
        ///
        /// <value> The view model. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public TitleBarVm? Vm
        {
            get => GetValue(VmProperty) as TitleBarVm;
            set => SetValue(VmProperty, value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes a new instance of the WinUI.CustomControls.TitleBar class. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public TitleBar()
        {
            InitializeComponent();
        }
        public void CloseWindow_Click(object _, RoutedEventArgs e)
        {
            Vm?.CloseWindowCmd?.Execute(e);
        }
        public void MazimizeWindow_Click(object _, RoutedEventArgs e)
        {
            Vm?.MaximizeWindowCmd?.Execute(e);
        }
        public void RestoreWindow_Click(object _, RoutedEventArgs e)
        {
            Vm?.RestoreWindowCmd?.Execute(e);
        }
        public void MinimizeWindow_Click(object _, RoutedEventArgs e)
        {
            Vm?.MinimizeWindowCmd?.Execute(e);
        }
    }
}
