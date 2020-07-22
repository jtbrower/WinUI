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
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A ViewModel for the main page. </summary>
    ///
    /// <seealso cref="WinUI.Vm.PropChangeBase"/>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class MainPageVm : PropChangeBase
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Number of box values. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private int _numberBoxValue;
        private ToggleButtonVm? _toggleNonClientVisibilityButtonVm;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes a new instance of the WinUI.Vm.MainPageVm class. </summary>
        ///
        /// <param name="dialogService">    The dialog service. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public MainPageVm(IDialogService dialogService)
        {
            DialogService = dialogService;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the number of box values. </summary>
        ///
        /// <value> The total number of box value. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public int NumberBoxValue { get => _numberBoxValue; set => SetProperty(ref _numberBoxValue, value); }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the dialog service. </summary>
        ///
        /// <value> The dialog service. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public IDialogService DialogService { get; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the toggle non client visibility button view model. </summary>
        ///
        /// <value> The toggle non client visibility button view model. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public ToggleButtonVm? ToggleNonClientVisibilityButtonVm
        {
            get => _toggleNonClientVisibilityButtonVm;
            set => SetProperty(ref _toggleNonClientVisibilityButtonVm, value);
        }
    }
}
