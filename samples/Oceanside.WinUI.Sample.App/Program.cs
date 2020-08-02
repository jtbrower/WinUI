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
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Diagnostics;

#if !DEBUG
    using System.Runtime.ExceptionServices;
#endif

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A program. </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public static class Program
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// If you prefer to use a static ServiceLocator then this is what you want to use.  Otherwise,
        /// its best to try and get as much as you can from automatic constructor injection.
        /// </summary>
        ///
        /// <value> The service provider. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        internal static ServiceProvider? ServiceProvider { get; private set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the current. </summary>
        ///
        /// <value> The current. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        internal static App? Current { get; private set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Main entry-point for this application. </summary>
        ///
        /// <param name="args"> An array of command-line argument strings. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
#pragma warning disable IDE0060 // Remove unused parameter
        private static void Main(string[] args)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            //Note that I still do not know if I need this line.  I adopted it when I was 
            // trying to find out how to start a WinUI desktop application from a Main method like this.
            // I found that Scott Jones from Microsoft had this call in his example.  So far I have not 
            // needed it, but I am leaving it here as a reminder of random stuff to try when I have some
            // problem that has sucked my brain clean of any solutions.  
            //WinRT.ComWrappersSupport.InitializeComWrappers();

            //In WPF it is easier to know when the application is exiting.  This is a solid way to
            // be notified for any required cleanup.
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            //If you are new to WinUI, coming from WPF like me, you will probably be a bit confused how this 
            // initializer works.  Merely declaring an instance of an Application is all that is required.
            // I could have placed all of this logic in "Main" itself, so be aware that you can just use
            // a simple lambda if you wish.
            Microsoft.UI.Xaml.Application.Start(ApplicationInitializationCallback);

            //Assure all services that implement IDisposable get a chance to run their dispose logic.
            ServiceProvider?.Dispose();

            //Not really needed, but I tend to do things like this out of good habit, plus some 
            // testing scenarios can cause abnormal control flow.
            ServiceProvider = null;
            Current = null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Current domain process exit. </summary>
        ///
        /// <param name="sender">   The sender. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static void CurrentDomain_ProcessExit(object? sender, EventArgs e)
        {
            //Additional way to know when the application is exiting.
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the services and creates the application instance. </summary>
        ///
        /// <param name="p">    A variable-length parameters list containing p. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static void ApplicationInitializationCallback(Microsoft.UI.Xaml.ApplicationInitializationCallbackParams p)
        {
            try
            {
                #region Logging explanation for the Developer
                //This deserves explanation.  Although I generally try to avoid static variables, I also like
                // to avoid as much cross-assembly coupling as possible.  When exceptions occur inside of 
                // an assembly that is not the main executable, I sometimes avoid coupling the assembly to 
                // a specific logging interface because it can bleed throughout the entire program.  Everyone
                // has their favorite logger and I like to give them the easiest method of integrating their
                // own choice.  I do that by providing a single point of entry for the entire assembly where
                // they can handle exceptions and logging how they wish.  If you do not set this Action, the
                // default one will be used.  The default logs to the console, calls Debugger.Break if the 
                // debugger is attached and then rethrows the original exception.
                // 
                // If you plan to use this, you should check to see if it is actually called.  I like to
                // provide this class but don't always need to use it.
                // 
                // Uncomment this line and replace it with your own choice or just leave it at its default.
                //ExternalOnExceptionCallBack.OnExceptionAction = e=> { Debug.WriteLine(e);};
                #endregion

                //Add services and build the provider.
                ServiceProvider = new ServiceCollection()
                    .AddServices()
                    .AddViewModels()
                    .AddViews()
                    .BuildServiceProvider();

                //Yes, this is actually how this callback works.  You really don't even need to
                // save the application instance, you just instantiate it.  I am saving it here
                // out of clarity.  Declaring an object in space just feels different.
                Current = ServiceProvider.GetRequiredService<App>();
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine(e);

                //Never leave a Debugger.Break in a release build because it isn't compiled out 
                // of the code like a Debug.WriteLine call is.  Unless the framework has been
                // changed, this will cause additional exceptions and obfuscate the real problem.
                if (Debugger.IsAttached) Debugger.Break();
#else
                //Try to rethrow the original exception
                ExceptionDispatchInfo.Capture(e.InnerException ?? e).Throw();
#endif
            }
        }
    }
}
