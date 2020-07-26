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
using System.Threading.Tasks;

namespace WinUI.Vm
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <content>   A ViewModel for the main page. This class cannot be inherited. </content>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public sealed partial class MainPageVm
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The require confirm for transparency above this amount. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private const uint C_RequireConfirmForTransparencyAboveThisAmount = 60;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Removes the window transparency described by mainPage. </summary>
        ///
        /// <param name="mainPage"> The main page. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        internal static void RemoveWindowTransparency(MainPageVm? mainPage)
        {
            //Assure the view didn't pass a null model
            if (mainPage == null) return;
            mainPage.OwnerWindow.RemoveTransparency();

            //Disable the button now that we have removed transparency.
            mainPage.RemoveWindowTransparencyButtonVm.IsEnabled = false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Size to content. </summary>
        ///
        /// <param name="mainPage"> The main page. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        internal static void SizeToContent(MainPageVm? mainPage)
        {
            //Assure the view didn't pass a null model
            if (mainPage == null) return;
            mainPage.OwnerWindow.SizeToContent();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds transparency to the MainPageVm Owner Window.  If the value is great than a given
        /// threshold then a confirmation dialog will be displayed.  You should be able to safely fire
        /// and forget the logic in this function.  It returns a Task because I always try to avoid the
        /// consequences of using async void methods.
        /// </summary>
        ///
        /// <param name="mainPage"> The main page. </param>
        ///
        /// <returns>   An asynchronous result. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        internal static async Task AddWindowTransparencyWithConfirmTask(MainPageVm? mainPage)
        {
            //Assure the view didn't pass a null model
            if (mainPage == null) return;

            var transparencyPercent = mainPage.WindowTransparencyPercent;

            if (transparencyPercent > C_RequireConfirmForTransparencyAboveThisAmount)
            {
                if (!await mainPage.DialogService.ConfirmAsync(
                    $"About Changing Transparency to {transparencyPercent}",
                    $"A Window Transparency of {transparencyPercent} could make it impossible to close the Window without a debugger.  Are you sure you want to continue?"))
                {
                    return;
                }
            }

            mainPage.OwnerWindow.SetTransparency(transparencyPercent);

            //If more than zero percent transparent then enable the button to remove transparency.
            mainPage.RemoveWindowTransparencyButtonVm.IsEnabled = transparencyPercent != 0;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Toggle non client visibility. </summary>
        ///
        /// <param name="mainPage"> The main page. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        internal static void ToggleNonClientVisibility(MainPageVm? mainPage)
        {
            //Assure the view didn't pass a null model
            if (mainPage == null) return;

            if (mainPage.ToggleNonClientVisibilityButtonVm.IsChecked)
                mainPage.OwnerWindow.ShowCustomNonClientArea();
            else
                mainPage.OwnerWindow.HideCustomNonClientArea();
        }
    }
}
