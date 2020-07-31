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
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
#if DEBUG
    using System.Linq;
    using System.Reflection;
#endif

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A notify property changed base. </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PropChangeBase : INotifyPropertyChanged, INotifyPropertyChanging
    {
#if DEBUG

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   List of names of the runtime properties. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private readonly List<string> _runtimePropertyNames;
#endif

#if DEBUG

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Default constructor. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public PropChangeBase()
        {
            _runtimePropertyNames = GetType().GetRuntimeProperties().Select(x => x.Name).ToList();
        }
#endif

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Occurs when a property value changes. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public event PropertyChangedEventHandler? PropertyChanged;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Occurs when a property value is changing. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public event PropertyChangingEventHandler? PropertyChanging;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Occurs when Static Property Changed. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static event EventHandler<PropertyChangedEventArgs>? StaticPropertyChanged;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event queue for all listeners interested in StaticPropertyChanging events. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static event EventHandler<PropertyChangingEventArgs>? StaticPropertyChanging;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the property changed action. </summary>
        /// <param name="propertyName"> (Optional) . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
#if DEBUG

            //Only verify if not null since null means all properties.
            if (Debugger.IsAttached)
            {
                VerifyPropertyName(propertyName);
            }
#endif

            var handler = PropertyChanged;
            if (handler == null) return;
            //Lock to prevent garbage collection before we invoke
            lock (handler)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Notifies all properties changed as well as changing. Use only in special cases
        ///             as this will cause a storm of events.</summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void NotifyAllPropertiesChanged()
        {
            //Indicate that all properties are changing.
            OnPropertyChanging(null);

            //Indicated that all properties changed.
            OnPropertyChanged(null);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the property changing action. </summary>
        /// <param name="propertyName"> (Optional) Name of the property. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnPropertyChanging([CallerMemberName] string? propertyName = null)
        {
            var handler = PropertyChanging;
            if (handler == null) return;
            //Lock to prevent garbage collection before we invoke
            lock (handler)
            {
                var e = new PropertyChangingEventArgs(propertyName);
                handler(this, e);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the static property changed action. </summary>
        /// <param name="propertyName"> . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static void OnStaticPropertyChanged(string? propertyName)
        {
            var handler = StaticPropertyChanged;
            if (handler == null) return;
            //Lock to prevent garbage collection before we invoke
            lock (handler)
            {
                handler(null, new PropertyChangedEventArgs(propertyName));
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the static property changing action. </summary>
        /// <param name="propertyName"> Name of the property. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static void OnStaticPropertyChanging(string? propertyName)
        {
            var handler = StaticPropertyChanging;
            if (handler == null) return;
            //Lock to prevent garbage collection before we invoke
            lock (handler)
            {
                handler(null, new PropertyChangingEventArgs(propertyName));
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets static property. </summary>
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="field">        [in,out]. </param>
        /// <param name="value">        . </param>
        /// <param name="propertyName"> (Optional) </param>
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool SetStaticProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;

            OnStaticPropertyChanging(propertyName);
            field = value;
            OnStaticPropertyChanged(propertyName);
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets a property. </summary>
        ///
        /// <param name="field">        [in,out] The field. </param>
        /// <param name="value">        The value. </param>
        /// <param name="propertyName"> (Optional) Name of the property. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;

            //Notify of changes about to occur.
            OnPropertyChanging(propertyName);
            field = value;

            //Notify of actual changes
            OnPropertyChanged(propertyName);

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   (Only available in DEBUG builds) verify property name. </summary>
        /// <throws cref="ArgumentException">
        ///     Thrown when one or more arguments have unsupported or illegal values.
        /// </throws>
        /// <param name="propertyName"> . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        private void VerifyPropertyName(string? propertyName)
        {
            //Note that even though this function is marked with a conditional attribute
            // to compile only in debug, the definition of the variable we use to 
            // hold the property names cannot be marked with that attribute.  We still
            // keep the attribute on this function because it reduces the number of 
            // if DEBUGs we have to sprinkle around elsewhere.
#if DEBUG
            //We allow null or empty here because that will cause all properties to update.  
            if (string.IsNullOrEmpty(propertyName) || _runtimePropertyNames.Contains(propertyName)) return;
            var msg = "Property Name: " + "\"" + propertyName + "\"" + " is Invalid";
            throw new ArgumentException(msg);
#endif
        }
    }
}