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
    using Microsoft.UI.Xaml.Input;
    using PInvoke;
    using System;
    using WinUI.Native;
    using static PInvoke.User32;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A drag move feature that I plan on changing into a behavior. </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class DragMoveFeature
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The identifier of captured pointer. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private uint? _idOfCapturedPointer;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The prior contact x coordinate. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private int _priorContactX;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The prior contact y coordinate. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private int _priorContactY;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The window content. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private UIElement? _windowContent;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The window. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private readonly Window _window;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Handle of the window. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private readonly IntPtr _windowHandle;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Initializes a new instance of the WinUI.CustomControls.DragMoveFeature class.
        /// </summary>
        ///
        /// <param name="window">       The window. </param>
        /// <param name="windowHandle"> Handle of the window. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public DragMoveFeature(Window window, IntPtr windowHandle)
        {
            _window = window;
            _windowHandle = windowHandle;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Pointer pressed. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Pointer routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (_windowContent == null) return;

            //Save the id of the pointer we will be tracking.
            _idOfCapturedPointer = e.Pointer.PointerId;

            //Capture it for dragging
            _windowContent.CapturePointer(e.Pointer);

            //Calculate the current contact location.
            GetCurrentContactPosition(e, out _priorContactX, out _priorContactY);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Pointer released. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Pointer routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            var capturedPointerId = _idOfCapturedPointer;
            if (!capturedPointerId.HasValue) return;

            //Make sure the pointer that is being released is the one we are using to drag
            if (capturedPointerId.Value != e.Pointer.PointerId) return;

            //Release capture
            _windowContent?.ReleasePointerCapture(e.Pointer);
            _idOfCapturedPointer = null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Pointer moved. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Pointer routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            //Make sure we are tracking this pointer for dragging
            var capturedPointerId = _idOfCapturedPointer;
            if (!capturedPointerId.HasValue) return;
            if (capturedPointerId.Value != e.Pointer.PointerId) return;

            //Calculate the capture location.
            GetCurrentContactPosition(e, out var x, out var y);

            //Determine how much the pointer changed
            var dX = x - _priorContactX;
            var dY = y - _priorContactY;

            //Save for next time
            _priorContactX = x;
            _priorContactY = y;

            //Move the window.
            _window.MoveBy(_windowHandle, dX, dY);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Calculates the contact position. </summary>
        ///
        /// <param name="e">    Pointer routed event information. </param>
        /// <param name="x">    [out] an int to fill in. </param>
        /// <param name="y">    [out] an int to fill in. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void GetCurrentContactPosition(PointerRoutedEventArgs e, out int x, out int y)
        {
            //Init
            x = 0;
            y = 0;

            //We need to know how much padding and border and titebar area surrounds the window because 
            // the pointer position we obtain is relative to the window.  There are many ways to do this
            // but I found many of them do not work for all cases.  This seems to do the trick.
            void GetNonClientSizes(RECT windowRect, out int borderWidth, out int borderHeight)
            {
                var clientToScreenPoint = new POINT();
                ClientToScreen(_windowHandle, ref clientToScreenPoint);

                borderWidth = clientToScreenPoint.x - windowRect.left;
                borderHeight = clientToScreenPoint.y - windowRect.top;
            }

            //Just because of nullable
            if (_windowContent == null) return;

            //Grab the current position and convert it to device units.  Note that at first I thought that
            // e.GetCurrentPoint(null).RawPosition would provide this but I was wrong.
            var p = e.GetCurrentPoint(null).Position;
            var deviceX = _windowContent.DipToDevice(p.X);
            var deviceY = _windowContent.DipToDevice(p.Y);

            //Grab the entire windows rect and calculate the non-client sizes.
            GetWindowRect(_windowHandle, out var windowRect);
            GetNonClientSizes(windowRect, out var nonClientWidth, out var nonClientHeight);

            //Current X coordinate in device units is offset from the Windows space from the left edge of the
            // screen and since the pointer position was given from the left of the client area we must also
            // add in the non-client area width.
            x = windowRect.left + deviceX + nonClientWidth;
            //Same story for the Y axis.
            y = windowRect.top + deviceY + nonClientHeight;

        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Attach drag move handlers. </summary>
        ///
        /// <param name="windowContent">    The window content. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void AttachDragMoveHandlers(UIElement windowContent)
        {
            _windowContent = windowContent;
            _windowContent.PointerPressed += PointerPressed;
            _windowContent.PointerMoved += PointerMoved;
            _windowContent.PointerReleased += PointerReleased;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Removes the drag move handlers. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void RemoveDragMoveHandlers()
        {
            if (_windowContent == null) return;
            _windowContent.PointerPressed -= PointerPressed;
            _windowContent.PointerMoved -= PointerMoved;
            _windowContent.PointerReleased -= PointerReleased;
        }
    }
}
