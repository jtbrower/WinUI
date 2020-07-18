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
namespace WinUI.CustomControls.Behaviors
{
    using Microsoft.UI.Xaml.Controls.Primitives;
    using Microsoft.Xaml.Interactivity;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// A swallow button double tap behavior assures that a button double tap does not fire the
    /// double tap event handler that is attached elsewhere.  For example, if a button is in a grid
    /// and you attached a double tap handler to the grid to maximize a window and you didn't want a
    /// double tap of a button to cause that event handler to fire (its confusing) then attach this
    /// behaviour as it will mark the event handled.
    /// </summary>
    ///
    /// <seealso cref="Microsoft.Xaml.Interactivity.Behavior{ButtonBase}"/>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class SwallowButtonDoubleTapBehavior : Behavior<ButtonBase>
    {
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
        /// <seealso cref="Microsoft.Xaml.Interactivity.Behavior{Microsoft.UI.Xaml.Controls.Button}.OnAttached()"/>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.DoubleTapped += AssociatedObject_DoubleTapped; 
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by AssociatedObject for double tapped events. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Double tapped routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void AssociatedObject_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            e.Handled = true;
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
            AssociatedObject.DoubleTapped -= AssociatedObject_DoubleTapped;
            base.OnDetaching();
        }
    }
}
