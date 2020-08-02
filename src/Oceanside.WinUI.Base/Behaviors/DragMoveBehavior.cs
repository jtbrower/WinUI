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

namespace Oceanside.WinUI.Base.Behaviors
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.Xaml.Interactivity;
    using Microsoft.UI.Xaml.Input;
    using PInvoke;
    using System;
    using Oceanside.Win32.Native;
    using static PInvoke.User32;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A drag move behavior. </summary>
    ///
    /// <seealso cref="Microsoft.Xaml.Interactivity.Behavior{Microsoft.UI.Xaml.Controls.Panel}"/>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class DragMoveBehavior : Behavior<Panel>
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets a value indicating whether we allow drag on maximized window.
        /// </summary>
        ///
        /// <value> True if allow drag on maximized window, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool AllowDragOnMaximizedWindow
        {
            get => (bool)GetValue(AllowDragOnMaximizedWindowProperty);
            set => SetValue(AllowDragOnMaximizedWindowProperty, value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The shadow size property. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static readonly DependencyProperty AllowDragOnMaximizedWindowProperty =
            DependencyProperty.Register(
                nameof(AllowDragOnMaximizedWindow), typeof(bool),
                typeof(DragMoveBehavior),
                new PropertyMetadata(false, (sender, args) =>
                {
                    if (!(args.NewValue is bool allowDrag)) return;
                    //DragMove logic will be null when first created.  For that reason, the initial state is set
                    // in the loaded event.
                    if (!(sender is DragMoveBehavior dragMoveBehavior && dragMoveBehavior._dragMoveLogic is not null)) return;
                    dragMoveBehavior._dragMoveLogic.AllowDragOnMaximizedWindow = allowDrag;
                }));

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets the millisecond delay before drag move can begin.  This is the amount of time
        /// that must pass from the PointerDown event until the Window is actually moved.  Increasing this
        /// delay helps to prevent converting touch and mouse interactions into DragMove events that were 
        /// intended to be single presses.
        /// </summary>
        ///
        /// <value> The millisecond delay before drag move can begin. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public int MillisecondDelayBeforeDragMoveCanBegin
        {
            get { return (int)GetValue(MillisecondDelayBeforeDragMoveCanBeginProperty); }
            set { SetValue(MillisecondDelayBeforeDragMoveCanBeginProperty, value); }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The millisecond delay before drag move can begin property. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static readonly DependencyProperty MillisecondDelayBeforeDragMoveCanBeginProperty =
            DependencyProperty.Register(
                nameof(MillisecondDelayBeforeDragMoveCanBegin), 
                typeof(int), 
                typeof(DragMoveBehavior),
                new PropertyMetadata(50, (sender, args) =>
                {
                    if (!(args.NewValue is int delay)) return;
                    //DragMove logic will be null when first created.  For that reason, the initial state is set
                    // in the loaded event.
                    if (!(sender is DragMoveBehavior dragMoveBehavior && dragMoveBehavior._dragMoveLogic is not null)) return;
                    dragMoveBehavior._dragMoveLogic.MillisecondDelayBeforeDragMoveCanBegin = delay;
                }));

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets the distance in pixels change before drag move can begin.  Using the Pythagorean
        /// Theorem to calculate the distance in pixels, it is compared to and must be at or above this
        /// value before the DragMove operation will begin.  Increasing this value conserves CPU cycles
        /// and deadens the sensitivity.  It is used in conjunction with the
        /// <see cref="MillisecondDelayBeforeDragMoveCanBegin" /> property.
        /// </summary>
        ///
        /// <value> The distance in pixels change before drag move can begin. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public int DistanceInPixelsChangeBeforeDragMoveCanBegin
        {
            get { return (int)GetValue(DistanceInPixelsChangeBeforeDragMoveCanBeginProperty); }
            set { SetValue(DistanceInPixelsChangeBeforeDragMoveCanBeginProperty, value); }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The distance in pixels change before drag move can begin property. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static readonly DependencyProperty DistanceInPixelsChangeBeforeDragMoveCanBeginProperty =
            DependencyProperty.Register(
                nameof(DistanceInPixelsChangeBeforeDragMoveCanBegin), 
                typeof(int), 
                typeof(DragMoveBehavior),
                new PropertyMetadata(6, (sender, args) =>
                {
                    if (!(args.NewValue is int pixelCount)) return;
                    //DragMove logic will be null when first created.  For that reason, the initial state is set
                    // in the loaded event.
                    if (!(sender is DragMoveBehavior dragMoveBehavior && dragMoveBehavior._dragMoveLogic is not null)) return;
                    dragMoveBehavior._dragMoveLogic.DistanceInPixelsChangeBeforeDragMoveCanBegin = pixelCount;
                }));

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The drag move logic. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private DragMoveLogic? _dragMoveLogic;

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
        /// <seealso cref="Microsoft.Xaml.Interactivity.Behavior{Microsoft.UI.Xaml.Controls.Panel}.OnAttached()"/>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void OnAttached()
        {
            //Once loaded we need the Window handle.
            AssociatedObject.Loaded += AssociatedObject_Loaded;
            base.OnAttached();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by AssociatedObject for loaded events. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.Loaded -= AssociatedObject_Loaded;

            //Note how I obtain the Window.  I would prefer to locate the Window by grabbing the Window
            // that this behavior's AssociatedObject is attached to, but at this time I do not know
            // how to do that.
            var windowHandle = GetActiveWindow();

            _dragMoveLogic = new DragMoveLogic(windowHandle)
            {
                AllowDragOnMaximizedWindow = AllowDragOnMaximizedWindow,
                MillisecondDelayBeforeDragMoveCanBegin = MillisecondDelayBeforeDragMoveCanBegin,
                DistanceInPixelsChangeBeforeDragMoveCanBegin=DistanceInPixelsChangeBeforeDragMoveCanBegin
            };
            _dragMoveLogic.AttachDragMoveHandlers(AssociatedObject);
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
            _dragMoveLogic?.RemoveDragMoveHandlers();
            base.OnDetaching();
        }

        //Logic for DragMove in a private namespace to direct user to use the DragMoveBehavior class 
        // instead.
        #region DragMoveLogic

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// I tucked all of the logic for the DragMove into this class so that it would not be tied
        /// specifically the Microsoft.Xaml.Interactivity namespace and related classes.  It would be
        /// easier to decouple the logic if someone needed to use it in another framework.  
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private class DragMoveLogic
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
            /// <summary>   Handle of the window. </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            private readonly IntPtr _windowHandle;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   The tick when pointer pressed. </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            private DateTime? _timeWhenPointerPressed;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   The milliseconds after pointer pressed required before drag move. </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            internal int MillisecondDelayBeforeDragMoveCanBegin { get; set; } = 50;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Gets or sets the distance in pixels change before drag move can begin. </summary>
            ///
            /// <value> The distance in pixels change before drag move can begin. </value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            internal int DistanceInPixelsChangeBeforeDragMoveCanBegin { get; set; } = 5;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   True if has met time threshold to begin drag move, false if not. </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            private bool _hasMetTimeThresholdToBeginDragMove;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   True to allow, false to suppress the drag on maximized window. </summary>
            ///
            /// <value> True if allow drag on maximized window, false if not. </value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            internal bool AllowDragOnMaximizedWindow { get; set; }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Initializes a new instance of the
            /// Oceanside.WinUI.Base.Behaviors.DragMoveBehavior.DragMoveLogic class.
            /// </summary>
            ///
            /// <param name="windowHandle">                 Handle of the window. </param>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public DragMoveLogic(IntPtr windowHandle)
            {
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
                _hasMetTimeThresholdToBeginDragMove = false;
                _timeWhenPointerPressed = DateTime.Now;

                if (_windowContent == null) return;

                //One side effect of providing DragMove is that when using touch, the interaction with the
                // Window can accidentally cause the DragMove to fire.  This can be pretty annoying and it 
                // will confuse the end user.  For example, if they double tap on a slight angle, that 
                // double tap can turn into a DragMove when the end user expected the Window to either Maximize
                // or Restore.
                //
                // The applications that I develop are usually full-screen KIOSK apps.  When the Window is 
                // full-screen I do not want any interaction to cause the Window to move.  This reduces
                // the amount of misinterpreted DragMoves that can occur.  There are other approaches that
                // I can combine with this so consider this an area for growth when needed.  I have provided
                // the _allowDragOnMaximizedWindow flag for you to make your own decision on whether to
                // allow drag on maximized Windows.
                if (!AllowDragOnMaximizedWindow && _windowHandle.IsMaximized()) return;

                //Save the id of the pointer we will be tracking.
                _idOfCapturedPointer = e.Pointer.PointerId;

                //Calculate the current contact location.
                GetCurrentContactPosition(e, out var x, out var y);
                _priorContactX = x;
                _priorContactY = y;
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
                _timeWhenPointerPressed = null;
                _hasMetTimeThresholdToBeginDragMove = false;
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
                if (!_timeWhenPointerPressed.HasValue) return;

                //Grab the pointer Id that we use to drag so we do not confuse it with another one.
                var capturedPointerId = _idOfCapturedPointer;
                if (!capturedPointerId.HasValue) return;
                if (capturedPointerId.Value != e.Pointer.PointerId) return;

                if (!_hasMetTimeThresholdToBeginDragMove)
                {
                    // This helps throttle movement from the initial PointerPressed event.  On touch screens,
                    //  people are usually not touching directly downward upon the screen.  It's often on a
                    //  slight angle and that slight angle can accidentally cause the DragMove event to fire
                    //  when they were not intending to do so.  This assures that enough time has passed
                    //  before the DragMove begins.
                    var diff = DateTime.Now.Subtract(_timeWhenPointerPressed.Value);
                    if (diff.TotalMilliseconds < MillisecondDelayBeforeDragMoveCanBegin)
                        return;

                    _hasMetTimeThresholdToBeginDragMove = true;
                }

                //Calculate the capture location.
                GetCurrentContactPosition(e, out var x, out var y);

                //Determine how much the pointer changed
                var dX = x - _priorContactX;
                var dY = y - _priorContactY;

                //We met a time threshold to begin the move, but now we check to see if the distance threshold
                // has been met.
                var distance = (int)Math.Sqrt(dX * dX + dY * dY);
                if (distance < DistanceInPixelsChangeBeforeDragMoveCanBegin) return;

                //Save for next time
                _priorContactX = x;
                _priorContactY = y;

                //Move the window.
                _windowHandle.MoveBy(dX, dY);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Calculates the contact position. This works with a mouse pointer AND touch.  You might be
            /// tempted to call User32.GetCursorPos() but it will not work with touch.
            /// </summary>
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

                //We need to know how much padding and border and TitleBar area surrounds the window because 
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
        #endregion //DragMoveLogic
    }
}
