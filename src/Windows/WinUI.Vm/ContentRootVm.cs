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
namespace WinUI.Vm
{
    public class ContentRootVm : PropChangeBase
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The title bar view model. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private TitleBarVm _titleBarVm;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the title bar view model. </summary>
        ///
        /// <value> The title bar view model. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public TitleBarVm TitleBarVm { get => _titleBarVm; set => SetProperty(ref _titleBarVm, value); }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes a new instance of the WinUI.Vm.ContentRootVm class. </summary>
        ///
        /// <param name="titleBarVm">   The title bar view model. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public ContentRootVm(TitleBarVm titleBarVm)
        {
            _titleBarVm = titleBarVm;
        }
    }
}
