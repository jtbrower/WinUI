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
    using Microsoft.UI.Threading;
    using Microsoft.UI.Xaml;
    using WinUI.Native;
    using Microsoft.Extensions.DependencyInjection;
    using WinUI.CustomControls;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <content>   An application. </content>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public partial class App : Application
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The service provider. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private ServiceProvider _serviceProvider;

        //Hang on to the reference
#pragma warning disable IDE0052 // Remove unread private members

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The drag move feature. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private DragMoveFeature? _dragMoveFeature;
#pragma warning restore IDE0052 // Remove unread private members

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes a new instance of the WinUI.DemoApp.App class. </summary>
        ///
        /// <param name="paramters">    A variable-length parameters list containing paramters. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public App(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            InitializeComponent();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Invoked when the application is launched. Override this method to perform application
        /// initialization and to display initial content in the associated Window.
        /// </summary>
        ///
        /// <param name="args"> Event data for the event. </param>
        ///
        /// <seealso cref="Microsoft.UI.Xaml.Application.OnLaunched(LaunchActivatedEventArgs)"/>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            //At the moment I do not know why or if this is required but I noticed it
            // in example code somewhere and decided to add it.  
            DispatcherQueueSyncContext.SetForCurrentThread();
            //I am not sure if calling the default is a noop.  The WinUI template did not
            // call the base but the same example code that I spoke about above did.
            base.OnLaunched(args);

            //Create a window and set the page.
            var window = _serviceProvider.GetRequiredService<MainWindow>();
            window.SetPage(_serviceProvider.GetRequiredService<SettingsPage>());

            //Merely requesting an instance of the drag feature is all it takes to attach it to the applications
            // main window.  It's probably more intuitive to have an initialize method.  Technically you wouldn't
            // need to hold onto this instance anymore because it attaches to the Window instance and is detached
            // when it is disposed.  Sinces it lasts the life of the application, the service container will dispose
            // of it when the application ends and the container is disposed.
            _dragMoveFeature = _serviceProvider.GetRequiredService<DragMoveFeature>();
            _dragMoveFeature.AttachDragMoveHandlers(window.Content);

            //Listen to close since we don't have the same application wide events that WPF offers for shutdown
            window.Closed += Window_Closed;
            window.Activate();

            //This will have a jarring effect because I have not had time to figure out how to set the size before
            // the window is activated for the first time.
            window.ResizeWindow(600, 500);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by Window for closed events. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="args">     Window event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            //This could be a bit wonkey because the window has now closed, yet we will be accessing that instance
            // to remove the event handlers.  We probably need to do that or nullify _dragMoveFeature because the
            // _dragMoveFeature could keep the Window instance alive.  This is just a bit overly anal because in this
            // sample application the app is about to close, but I just wanted newer developers to understand this
            // before they copy and paste this code to fit their own needs.
            _dragMoveFeature?.RemoveDragMoveHandlers();

            _serviceProvider?.Dispose();
        }
    }
}
