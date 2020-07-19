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
    using Microsoft.UI.Xaml;
    using Microsoft.Extensions.DependencyInjection;
    using WinUI.CustomControls;
    using WinUI.Vm;
    using System;
    using System.Diagnostics;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A program. </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public static class Program
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The service provider. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        internal static ServiceProvider? ServiceProvider { get; private set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The application. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
#pragma warning disable IDE0052 // Remove unread private members
        private static App _application;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the services. </summary>
        ///
        /// <returns>   A ServiceProvider. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static ServiceProvider InitializeServices()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<IDialogService, DialogService>();
            serviceCollection.AddSingleton<WpfWindow>();
            serviceCollection.AddSingleton<IWpfWindow>(s => s.GetRequiredService<WpfWindow>());
            serviceCollection.AddSingleton<SettingsPage>();
            serviceCollection.AddSingleton<SettingsPageVm>();
            return serviceCollection.BuildServiceProvider();
        }
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
        static void Main(string[] args)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            WinRT.ComWrappersSupport.InitializeComWrappers();
            Application.Start(ApplicationInitializationCallback);
            ServiceProvider?.Dispose();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the services and creates the application instance. </summary>
        ///
        /// <param name="p">    A variable-length parameters list containing p. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        static void ApplicationInitializationCallback(ApplicationInitializationCallbackParams p)
        {
            try
            {
                ServiceProvider = InitializeServices();

                //Yes, this is actually how this callback works.  You really don't even need to
                // save the application instance, you just instantiate it.  I am saving it here
                // out of clarity.  Declaring an object in space just feels different.
                _application = new App(ServiceProvider);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
    }
}
