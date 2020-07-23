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
namespace WinUI.Native
{
    using Microsoft.UI.Xaml;
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using WinRT;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <content>   Logic required to obtain the Window Handle. </content>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public static partial class NativeMethods
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   A Window extension method that gets a handle. </summary>
        ///
        /// <param name="window">   The window to act on. </param>
        ///
        /// <returns>   The handle. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static IntPtr GetHandle(this Window window)
        {
            WindowWrapper windowWrapper = WindowWrapper.FromAbi(window.ThisPtr);
            return windowWrapper.WindowHandle;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets current procedure main window handle. </summary>
        ///
        /// <returns>   The current procedure main window handle. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static IntPtr GetCurrentProcMainWindowHandle()
        {
            using var process = Process.GetCurrentProcess();
            return process.MainWindowHandle;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Interface for window native. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("EECDBF0E-BAE9-4CB6-A68E-9598E1CB57BB")]
        private interface IWindowNative
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Gets the handle of the window. </summary>
            ///
            /// <value> The window handle. </value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            IntPtr WindowHandle { get; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// See https://github.com/microsoft/microsoft-ui-xaml/issues/2564#issuecomment-638420195 for
        /// information regarding the source of the WindowWrapper class and the MIT License where the
        /// code was located https://github.com/microsoft/WinUI-3-Demos/blob/master/LICENSE.
        /// </summary>
        ///
        /// <seealso cref="WinUI.Native.NativeMethods.IWindowNative"/>
        /// <seealso cref="Oceanside.Views.WinUI.WindowInterop.IWindowNative"/>
        /// <seealso cref="Oceanside.Views.WinUI.Interop.IWindowNative"/>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [Guid("EECDBF0E-BAE9-4CB6-A68E-9598E1CB57BB")]
        private class WindowWrapper : IWindowNative
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   A vftbl. </summary>
            ///
            /// <seealso cref="WinUI.Native.NativeMethods.IWindowNative"/>
            /// <seealso cref="Oceanside.Views.WinUI.Interop.IWindowNative"/>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            [Guid("EECDBF0E-BAE9-4CB6-A68E-9598E1CB57BB")]
            public struct Vftbl
            {
                ////////////////////////////////////////////////////////////////////////////////////////////////////
                /// <summary>   Gets window handle 0. </summary>
                ///
                /// <param name="thisPtr">      this pointer. </param>
                /// <param name="windowHandle"> [out] Handle of the window. </param>
                ///
                /// <returns>   An int. </returns>
                ////////////////////////////////////////////////////////////////////////////////////////////////////

                public delegate int _get_WindowHandle_0(IntPtr thisPtr, out IntPtr windowHandle);

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                /// <summary>   Zero-based index of the unknown vftbl. </summary>
                ////////////////////////////////////////////////////////////////////////////////////////////////////

                internal WinRT.Interop.IUnknownVftbl IUnknownVftbl;

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                /// <summary>   The get window handle 0. </summary>
                ////////////////////////////////////////////////////////////////////////////////////////////////////

                public _get_WindowHandle_0 get_WindowHandle_0;

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                /// <summary>   The abi to projection vftable. </summary>
                ////////////////////////////////////////////////////////////////////////////////////////////////////

                public static readonly Vftbl AbiToProjectionVftable;

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                /// <summary>   The abi to projection vftable pointer. </summary>
                ////////////////////////////////////////////////////////////////////////////////////////////////////

                public static readonly IntPtr AbiToProjectionVftablePtr;

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                /// <summary>
                /// Initializes static members of the Oceanside.Views.WinUI.WindowInterop.WindowWrapper class.
                /// </summary>
                ////////////////////////////////////////////////////////////////////////////////////////////////////

                static Vftbl()
                {
                    AbiToProjectionVftable = new Vftbl
                    {
                        IUnknownVftbl = WinRT.Interop.IUnknownVftbl.AbiToProjectionVftbl,
                        get_WindowHandle_0 = Do_Abi_get_WindowHandle_0
                    };
                    AbiToProjectionVftablePtr = Marshal.AllocHGlobal(Marshal.SizeOf<Vftbl>());
                    Marshal.StructureToPtr(AbiToProjectionVftable, AbiToProjectionVftablePtr, false);
                }

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                /// <summary>   Executes the abi get window handle 0 operation. </summary>
                ///
                /// <param name="thisPtr">      this pointer. </param>
                /// <param name="windowHandle"> [out] Handle of the window. </param>
                ///
                /// <returns>   An int. </returns>
                ////////////////////////////////////////////////////////////////////////////////////////////////////

                private static int Do_Abi_get_WindowHandle_0(IntPtr thisPtr, out IntPtr windowHandle)
                {
                    windowHandle = default;
                    try
                    {
                        windowHandle = ComWrappersSupport.FindObject<IWindowNative>(thisPtr).WindowHandle;
                    }
                    catch (Exception ex)
                    {
                        return Marshal.GetHRForException(ex);
                    }
                    return 0;
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Initializes this  from the given abi. </summary>
            ///
            /// <param name="thisPtr">  this pointer. </param>
            ///
            /// <returns>   An ObjectReference{Vftbl} </returns>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            internal static ObjectReference<Vftbl> FromAbi(IntPtr thisPtr)
            {
                return ObjectReference<Vftbl>.FromAbi(thisPtr);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Implicit cast that converts the given IObjectReference to a WindowWrapper. </summary>
            ///
            /// <param name="obj">  The object. </param>
            ///
            /// <returns>   The result of the operation. </returns>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static implicit operator WindowWrapper(IObjectReference obj)
            {
#pragma warning disable CS8603 // Possible null reference return.
                return (obj != null) ? new WindowWrapper(obj) : null;
#pragma warning restore CS8603 // Possible null reference return.
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   The object. </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            protected readonly ObjectReference<Vftbl> _obj;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Gets the object reference. </summary>
            ///
            /// <value> The object reference. </value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public IObjectReference ObjRef => _obj;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Gets this pointer. </summary>
            ///
            /// <value> this pointer. </value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public IntPtr ThisPtr => _obj.ThisPtr;

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Converts this  to an interface. </summary>
            ///
            /// <typeparam name="I">    Generic type parameter. </typeparam>
            ///
            /// <returns>   An ObjectReference{I} </returns>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public ObjectReference<I> AsInterface<I>()
            {
                return _obj.As<I>();
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Gets as. </summary>
            ///
            /// <typeparam name="A">    Type of a. </typeparam>
            ///
            /// <returns>   A. </returns>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public A As<A>()
            {
                return _obj.AsType<A>();
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Initializes a new instance of the Oceanside.Views.WinUI.WindowInterop.WindowWrapper class.
            /// </summary>
            ///
            /// <param name="obj">  The object. </param>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public WindowWrapper(IObjectReference obj) : this(obj.As<Vftbl>()) { }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Initializes a new instance of the Oceanside.Views.WinUI.WindowInterop.WindowWrapper class.
            /// </summary>
            ///
            /// <param name="obj">  The object. </param>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            internal WindowWrapper(ObjectReference<Vftbl> obj)
            {
                _obj = obj;
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Gets the handle of the window. </summary>
            ///
            /// <value> The window handle. </value>
            ///
            /// <seealso cref="Oceanside.Views.WinUI.Interop.IWindowNative.WindowHandle"/>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public IntPtr WindowHandle
            {
                get
                {
                    Marshal.ThrowExceptionForHR(_obj.Vftbl.get_WindowHandle_0(ThisPtr, out var windowHandle));

                    return windowHandle;
                }
            }
        }
    }
}
