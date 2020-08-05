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
    using Microsoft.UI.Xaml.Input;
    using Microsoft.Xaml.Interactivity;
    using Oceanside.Win32.Native;
    using PInvoke;
    using System;
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
        /// A Normal CLR Property that must be set before the behaviors associated object is loaded. Note
        /// that this is a plain CLR property because IntPtr cannot be a dependency property.  Note that
        /// once the assoicated object is loaded, changing this value will have no effect and you
        /// wouldn't need or want to do that anyway.
        /// </summary>
        ///
        /// <value> The window handle. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public IntPtr WindowHandle { get;set;}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a value indicating whether we allow drag on maximized window. </summary>
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
                    // in the loaded event.  If it its changed after that, it would be handled here.
                    if (!(sender is DragMoveBehavior dragMoveBehavior && dragMoveBehavior.DragLogic is not null)) return;
                    dragMoveBehavior.DragLogic.AllowDragOnMaximizedWindow = allowDrag;
                }));

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets the millisecond delay before drag move can begin.  This is the amount of time
        /// that must pass from the PointerDown event until the Window is actually moved.  Increasing
        /// this delay helps to prevent converting touch and mouse interactions into DragMove events that
        /// were intended to be single presses.
        /// </summary>
        ///
        /// <value> The millisecond delay before drag move can begin. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public int MillisecondDelayBeforeDragMoveCanBegin
        {
            get => (int)GetValue(MillisecondDelayBeforeDragMoveCanBeginProperty);
            set => SetValue(MillisecondDelayBeforeDragMoveCanBeginProperty, value);
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
                    // in the loaded event.  If it its changed after that, it would be handled here.
                    if (!(sender is DragMoveBehavior dragMoveBehavior && dragMoveBehavior.DragLogic is not null)) return;

                    //The logic that handles dragging is in a unique class to make it easier to reuse in
                    // situations where people do not want to use a behavior.
                    dragMoveBehavior.DragLogic.MillisecondDelayBeforeDragMoveCanBegin = delay;
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
            get => (int)GetValue(DistanceInPixelsChangeBeforeDragMoveCanBeginProperty);
            set => SetValue(DistanceInPixelsChangeBeforeDragMoveCanBeginProperty, value);
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
                    // in the loaded event.   If it its changed after that, it would be handled here.
                    if (!(sender is DragMoveBehavior dragMoveBehavior && dragMoveBehavior.DragLogic is not null)) return;
                    dragMoveBehavior.DragLogic.DistanceInPixelsChangeBeforeDragMoveCanBegin = pixelCount;
                }));

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   This instance has all logic required to provide the feature of Window.DragMove.  It
        ///             is kept in a unique class apart from this behavior so that people can reuse it
        ///             without being forced to use the WinIU behavior type. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private DragMoveLogic? DragLogic { get; set; }

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

            //The logic is what truly provides the DragMove feature.
            DragLogic = new DragMoveLogic(WindowHandle)
            {
                //Properties in the logic mirror properties in the behavior.  When behavior properties are
                // updated, we update these properties.
                AllowDragOnMaximizedWindow = AllowDragOnMaximizedWindow,
                MillisecondDelayBeforeDragMoveCanBegin = MillisecondDelayBeforeDragMoveCanBegin,
                DistanceInPixelsChangeBeforeDragMoveCanBegin = DistanceInPixelsChangeBeforeDragMoveCanBegin
            };

            DragLogic.AttachDragMoveHandlers(AssociatedObject);
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
            DragLogic?.RemoveDragMoveHandlers();
            base.OnDetaching();
        }


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
            /// <summary>   All timing and distance details are kept in the state object. </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            private CaptureState? _captureState;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   The window content. </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            private UIElement? _windowContent;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Handle of the window. </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            private readonly IntPtr _windowHandle;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   The milliseconds after pointer pressed required before drag move. </summary>
            ///
            /// <value> The millisecond delay before drag move can begin. </value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            internal int MillisecondDelayBeforeDragMoveCanBegin { get; set; } = 50;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Gets or sets the distance in pixels change before drag move can begin. </summary>
            ///
            /// <value> The distance in pixels change before drag move can begin. </value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            internal int DistanceInPixelsChangeBeforeDragMoveCanBegin { get; set; } = 5;

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
            /// <param name="windowHandle"> Handle of the window. </param>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public DragMoveLogic(IntPtr windowHandle)
            {
                _windowHandle = windowHandle;
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Attach drag move handlers. </summary>
            ///
            /// <param name="windowContent">    The window content is a UIElement that is used to register
            ///                                 for pointer events.  This element should be a container that
            ///                                 covers as much of the Window as possible because it will provide
            ///                                 more dragging surface area to grab from. </param>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public void AttachDragMoveHandlers(UIElement windowContent)
            {
                if (_windowContent != null)
                    throw new InvalidOperationException($"{nameof(AttachDragMoveHandlers)} was called more than once");

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
                _windowContent = null;
            }

            #region Pointer Event Handlers
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Pointer pressed. </summary>
            ///
            /// <param name="sender">   Source of the event. </param>
            /// <param name="e">        Pointer routed event information. </param>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            private void PointerPressed(object sender, PointerRoutedEventArgs e)
            {
                //If we already began a drag move on another Pointer then there is nothing to do.  If you are
                // not aware, there can be several pointers all firing these events.  For example you have the
                // mouse, a pen or a touch device.
                if (_captureState != null) return;

                //Won't happen but just in case.
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
                // the AllowDragOnMaximizedWindow property for you to make your own decision on whether to
                // allow drag on maximized Windows.
                if (!AllowDragOnMaximizedWindow && _windowHandle.IsMaximized()) return;

                //First step in the process before drag begins.
                _captureState = CaptureState.StartCapture(_windowHandle, _windowContent, e, DistanceInPixelsChangeBeforeDragMoveCanBegin);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Pointer released. </summary>
            ///
            /// <param name="sender">   Source of the event. </param>
            /// <param name="e">        Pointer routed event information. </param>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            private void PointerReleased(object sender, PointerRoutedEventArgs e)
            {
                if (_captureState == null) return;

                //Make sure the pointer that is being released is the one we are using to drag
                if (_captureState.CapturedPointer.PointerId != e.Pointer.PointerId) return;

                _captureState.EndCapture();
                _captureState = null;
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Pointer moved. </summary>
            ///
            /// <param name="sender">   Source of the event. </param>
            /// <param name="e">        Pointer routed event information. </param>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            private void PointerMoved(object sender, PointerRoutedEventArgs e)
            {
                //If not down then do not process
                if (!e.Pointer.IsInContact) return;

                //If we didn't process a recent PointerPressed then leave
                if (_captureState == null) return;

                //If the pointer that moved isn't the one we captured, leave
                if (_captureState.CapturedPointer.PointerId != e.Pointer.PointerId) return;

                //If the time window has not been met then wait until it has been. This helps prevent
                // some accidental moves
                if (!_captureState.HasMetTimeThresholdToBeginDragMove)
                {
                    // This helps throttle movement from the initial PointerPressed event.  On touch screens,
                    //  people are usually not touching directly downward upon the screen.  It's often on a
                    //  slight angle and that slight angle can accidentally cause the DragMove event to fire
                    //  when they were not intending to do so.  This assures that enough time has passed
                    //  before the DragMove begins.
                    var diff = DateTime.Now.Subtract(_captureState.CapturedAt);
                    if (diff.TotalMilliseconds < MillisecondDelayBeforeDragMoveCanBegin)
                        return;

                    _captureState.HasMetTimeThresholdToBeginDragMove = true;
                }

                if(!_captureState.UpdateState(e))return;

                //Move the window.
                _windowHandle.MoveBy(_captureState._dX, _captureState._dY);
            }

            #endregion

            #region Private Class to Hold Mouse Capture State
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   A capture state. </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            private class CaptureState
            {
                ////////////////////////////////////////////////////////////////////////////////////////////////////
                /// <summary>   The captured content. </summary>
                ////////////////////////////////////////////////////////////////////////////////////////////////////

                private readonly UIElement _capturedContent;

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                /// <summary>   The handle. </summary>
                ////////////////////////////////////////////////////////////////////////////////////////////////////

                internal readonly IntPtr _wHandle;

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                /// <summary>   True if capture ended. </summary>
                ////////////////////////////////////////////////////////////////////////////////////////////////////

                private bool _captureEnded;

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                /// <summary>   The pixel move threshold. </summary>
                ////////////////////////////////////////////////////////////////////////////////////////////////////

                private readonly int _pixelMoveThreshold;

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                /// <summary>   Starts a capture. </summary>
                ///
                /// <param name="handle">               The handle. </param>
                /// <param name="capturedContent">      The captured content. </param>
                /// <param name="args">                 Pointer routed event information. </param>
                /// <param name="pixelMoveThreshold">   The pixel move threshold. </param>
                ///
                /// <returns>   A CaptureState? </returns>
                ////////////////////////////////////////////////////////////////////////////////////////////////////

                internal static CaptureState? StartCapture(
                    IntPtr handle, 
                    UIElement capturedContent, 
                    PointerRoutedEventArgs args, 
                    int pixelMoveThreshold)
                {
                    //If the content won't let us capture the pointer then we have to return null.
                    if (!capturedContent.CapturePointer(args.Pointer)) return null;

                    var state = new CaptureState(handle, capturedContent, args.Pointer, pixelMoveThreshold);

                    //Ignore the return value, we do not need to move the window yet because we just began the capture
                    _ = state.UpdateState(args);
                    return state;
                }

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                /// <summary>
                /// Initializes a new instance of the
                /// Oceanside.WinUI.Base.Behaviors.DragMoveBehavior.DragMoveLogic.CaptureState class.
                /// </summary>
                ///
                /// <param name="handle">               The handle. </param>
                /// <param name="capturedContent">      The captured content. </param>
                /// <param name="pointer">              The pointer. </param>
                /// <param name="pixelMoveThreshold">   The pixel move threshold. </param>
                ////////////////////////////////////////////////////////////////////////////////////////////////////

                private CaptureState(IntPtr handle, UIElement capturedContent, Pointer pointer, int pixelMoveThreshold)
                {
                    _pixelMoveThreshold = pixelMoveThreshold;
                    _wHandle = handle;
                    _capturedContent = capturedContent;
                    CapturedAt = DateTime.Now;
                    CapturedPointer = pointer;
                }

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                /// <summary>   Gets the Date/Time of the captured at. </summary>
                ///
                /// <value> The captured at. </value>
                ////////////////////////////////////////////////////////////////////////////////////////////////////

                internal DateTime CapturedAt { get; }

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                /// <summary>
                /// Gets or sets a value indicating whether this  has met time threshold to begin drag move.
                /// </summary>
                ///
                /// <value> True if this  has met time threshold to begin drag move, false if not. </value>
                ////////////////////////////////////////////////////////////////////////////////////////////////////

                internal bool HasMetTimeThresholdToBeginDragMove { get; set; }

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                /// <summary>   The identifier of captured pointer. </summary>
                ///
                /// <value> The captured pointer. </value>
                ////////////////////////////////////////////////////////////////////////////////////////////////////

                internal Pointer CapturedPointer { get; }

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                /// <summary>   The prior contact x coordinate. </summary>
                ////////////////////////////////////////////////////////////////////////////////////////////////////

                internal int? _lastMoveToX;

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                /// <summary>   The prior contact y coordinate. </summary>
                ////////////////////////////////////////////////////////////////////////////////////////////////////

                internal int? _lastMoveToY;

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                /// <summary>   Distance moved on X axis. </summary>
                ////////////////////////////////////////////////////////////////////////////////////////////////////

                internal int _dX;

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                /// <summary>   Distance moved on Y axis. </summary>
                ////////////////////////////////////////////////////////////////////////////////////////////////////

                internal int _dY;

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                /// <summary>   The distance between the last mouse down point and the current one. </summary>
                ////////////////////////////////////////////////////////////////////////////////////////////////////

                internal int _distance;

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                /// <summary>   Updates the current contact position. </summary>
                ///
                /// <param name="handle">           The handle. </param>
                /// <param name="capturedContent">  The captured content. </param>
                /// <param name="e">                Pointer routed event information. </param>
                /// <param name="captureState">     State of the capture. </param>
                ////////////////////////////////////////////////////////////////////////////////////////////////////

                internal bool UpdateState(PointerRoutedEventArgs e)
                {
                    if (_captureEnded) return false;

                    //We need to know how much padding and border and TitleBar area surrounds the window because 
                    // the pointer position we obtain is relative to the window.  There are many ways to do this
                    // but I found many of them do not work for all cases.  This seems to do the trick.
                    bool GetNonClientSizes(RECT windowRect, out int borderWidth, out int borderHeight)
                    {
                        var clientToScreenPoint = new POINT();
                        if (!ClientToScreen(_wHandle, ref clientToScreenPoint))
                        {
                            borderWidth = 0;
                            borderHeight = 0;
                            return false;
                        }

                        borderWidth = clientToScreenPoint.x - windowRect.left;
                        borderHeight = clientToScreenPoint.y - windowRect.top;
                        return true;
                    }

                    //Grab the current position and convert it to device units.  Note that at first I thought that
                    // e.GetCurrentPoint(null).RawPosition would provide this but I was wrong.
                    var p = e.GetCurrentPoint(null).Position;
                    var deviceX = _capturedContent.DipToDevice(p.X);
                    var deviceY = _capturedContent.DipToDevice(p.Y);

                    //Grab the entire windows rect and calculate the non-client sizes.
                    if (!GetWindowRect(_wHandle, out var windowRect)) return false; //If Failed, no move
                    if (!GetNonClientSizes(windowRect, out var nonClientWidth, out var nonClientHeight)) return false;

                    //Current X coordinate in device units is offset from the Window's space from the left edge of the
                    // screen to left edge of Window. Since the pointer position was given from the left of the client
                    // and not the window itself, we must also add in the non-client area width.
                    var currentScreenX = windowRect.left + deviceX + nonClientWidth;
                    //Same story for the Y axis.
                    var currentScreenY = windowRect.top + deviceY + nonClientHeight;

                    //If null then this is the first contact for this particular state instance
                    if (!(_lastMoveToX.HasValue && _lastMoveToY.HasValue))
                    {
                        //Init the starting point
                        _lastMoveToX = currentScreenX;
                        _lastMoveToY = currentScreenY;

                        //Dont move
                        return false;
                    }

                    //Determine how much the pointer changed
                    _dX = currentScreenX - _lastMoveToX.Value;
                    _dY = currentScreenY - _lastMoveToY.Value;

                    if (_dX == 0 && _dY == 0) return false;
                    //See if the distance between the last point we recorded and the current point have surpassed the
                    // distance threshold.  If not, wait until it has.
                    _distance = (int)Math.Sqrt(_dX * _dX + _dY * _dY);
                    if (_distance < _pixelMoveThreshold) return false;

                    _lastMoveToX = currentScreenX;
                    _lastMoveToY = currentScreenY;

                    //If we hit this point we have a change in X and/or a change in Y that surpasses a distance in
                    // pixels that requires us to move the Window to keep up with the pointer.
                    return true;
                }

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                /// <summary>   Ends a capture. </summary>
                ////////////////////////////////////////////////////////////////////////////////////////////////////

                internal void EndCapture()
                {
                    if (_captureEnded) return;
                    _captureEnded = true;
                    _capturedContent.ReleasePointerCapture(CapturedPointer);
                }
            }
            #endregion
        }
        #endregion //DragMoveLogic
    }
}
