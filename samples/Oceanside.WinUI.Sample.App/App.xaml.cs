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

namespace Oceanside.WinUI.Sample.App
{
    using Oceanside.WinUI.Base.Vm;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.UI.Threading;
    using Microsoft.UI.Xaml;
    using System;
    using Oceanside.WinUI.Sample.App.Vm;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <content>   An application. </content>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public partial class App
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The service provider. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private readonly IServiceProvider _serviceProvider;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes a new instance of the Oceanside.WinUI.Sample.App.App class. </summary>
        ///
        /// <param name="paramters">    A variable-length parameters list containing parameters. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public App(IServiceProvider serviceProvider)
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
            var window = _serviceProvider.GetRequiredService<IExtWindow>();

            //Set the initial page to display by grabbing the View Model which will cause
            // the DataTemplate system to display the content with a template that we
            // provided through the App.xaml resources.
            var windowVm = _serviceProvider.GetRequiredService<WindowVm>();
            windowVm.ClientsContent = _serviceProvider.GetRequiredService<MainPageVm>();
            window.Vm = windowVm;

            //Start transparent so we have a chance to resize before its visible.  This does not
            // fully satisfy my desire to show a Window with a specific size without the need 
            // for WinUI to show it at a random size first.  More work will come on this, but this
            // is a rough half-baked work-around until I can get to this area.
            window.SetTransparency(100);

            window.Activate();

            //This will have a jarring effect because I have not had time to figure out how to set the size before
            // the window is activated for the first time.
            window.Resize(600, 400);

            //Make the window fully visible by removing the transparency.
            window.SetTransparency(0);
        }
    }
}
