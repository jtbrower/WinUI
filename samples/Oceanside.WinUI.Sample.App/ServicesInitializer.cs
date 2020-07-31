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

namespace WinUI.Vm
{
    using Microsoft.Extensions.DependencyInjection;
    using WinUI.CustomControls;
    using WinUI.DemoApp;
    using WinUI.DemoApp.View;
    using WinUI.DemoApp.Vm;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   The services initializer. </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    internal static class ServicesInitializer
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   An IServiceCollection extension method that adds the services. </summary>
        ///
        /// <param name="serviceCollection">    The serviceCollection to act on. </param>
        ///
        /// <returns>   An IServiceCollection. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        internal static IServiceCollection AddServices(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddScoped<IDialogService, DialogService>();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   An IServiceCollection extension method that adds the views. </summary>
        ///
        /// <param name="serviceCollection">    The serviceCollection to act on. </param>
        ///
        /// <returns>   An IServiceCollection. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        internal static IServiceCollection AddViews(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddSingleton<ExtWindow>()
            .AddSingleton<IExtWindow>(s => s.GetRequiredService<ExtWindow>())
            .AddSingleton<IPlatform>(s => s.GetRequiredService<ExtWindow>())
            .AddSingleton<App>()
            .AddScoped<MainPage>();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   An IServiceCollection extension method that adds a view models. </summary>
        ///
        /// <param name="serviceCollection">    The serviceCollection to act on. </param>
        ///
        /// <returns>   An IServiceCollection. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        internal static IServiceCollection AddViewModels(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddScoped(s => new TitleBarVm
            {
                Title = "WinUI Desktop Demo",

            })
            .AddSingleton<WindowVm>()
            .AddScoped<MainPageVm>();
        }
    }
}
