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

namespace Oceanside.WinUI.Base.Vm
{
    using System;
    using System.Threading;
    using System.Windows.Input;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A delegate command. </summary>
    ///
    /// <seealso cref="WinUI.Vm.DelegateCmd{System.Object}"/>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class DelegateCmd : DelegateCmd<object>
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes a new instance of the WinUI.Vm.DelegateCmd class. </summary>
        ///
        /// <param name="executeMethod">    The execute method. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public DelegateCmd(Action executeMethod)
            : base(o => executeMethod())
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes a new instance of the WinUI.Vm.DelegateCmd class. </summary>
        ///
        /// <param name="executeMethod">    The execute method. </param>
        /// <param name="canExecuteMethod"> The can execute method. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public DelegateCmd(Action executeMethod, Func<bool> canExecuteMethod)
            : base(o => executeMethod(), o => canExecuteMethod())
        {
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A delegate command. </summary>
    ///
    /// <typeparam name="T">    Generic type parameter. </typeparam>
    ///
    /// <seealso cref="WinUI.Vm.PropChangeBase"/>
    /// <seealso cref="System.Windows.Input.ICommand"/>
    /// <seealso cref="WinUI.Vm.IRaiseCanExecuteChanged"/>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class DelegateCmd<T> : PropChangeBase, ICommand, IRaiseCanExecuteChanged
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The can execute method. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private readonly Func<T, bool>? _canExecuteMethod;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The execute method. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private readonly Action<T> _executeMethod;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Context for the synchronization. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private readonly SynchronizationContext _synchronizationContext;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   True if is executing, false if not. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool _isExecuting;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes a new instance of the WinUI.Vm.DelegateCmd{T} class. </summary>
        ///
        /// <param name="executeMethod">    The execute method. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public DelegateCmd(Action<T> executeMethod)
            : this(executeMethod, x => true)
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes a new instance of the WinUI.Vm.DelegateCmd{T} class. </summary>
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
        ///                                             null. </exception>
        ///
        /// <param name="executeMethod">    The execute method. </param>
        /// <param name="canExecuteMethod"> The can execute method. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public DelegateCmd(Action<T> executeMethod, Func<T, bool>? canExecuteMethod)
        {
            _synchronizationContext = SynchronizationContext.Current;
            _executeMethod = executeMethod ?? throw new ArgumentNullException(nameof(executeMethod), "Execute Method cannot be null");
            _canExecuteMethod = canExecuteMethod;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public event EventHandler? CanExecuteChanged;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        ///
        /// <param name="parameter">    Data used by the command.  If the command does not require data
        ///                             to be passed, this object can be set to <see langword="null" />. </param>
        ///
        /// <returns>
        /// <see langword="true" /> if this command can be executed; otherwise, <see langword="false" />.
        /// </returns>
        ///
        /// <seealso cref="ICommand.CanExecute(object)"/>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        bool ICommand.CanExecute(object parameter)
        {
#pragma warning disable CS8604 // Possible null reference argument.
            return !_isExecuting && CanExecute(parameter is T variable ? variable : default);
#pragma warning restore CS8604 // Possible null reference argument.
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Defines the method to be called when the command is invoked. </summary>
        ///
        /// <param name="parameter">    Data used by the command.  If the command does not require data
        ///                             to be passed, this object can be set to <see langword="null" />. </param>
        ///
        /// <seealso cref="ICommand.Execute(object)"/>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        void ICommand.Execute(object parameter)
        {
            _isExecuting = true;
            try
            {
                PostOrInvokeCanExecuteChanged();
                Execute((T)parameter);
            }
            catch (Exception e)
            {
                ExternalOnExceptionCallBack.TryNotifyOfException(e);
            }
            finally
            {
                _isExecuting = false;
                PostOrInvokeCanExecuteChanged();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Posts the or invoke can execute changed. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void PostOrInvokeCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler == null) return;
            if (_synchronizationContext != null && _synchronizationContext != SynchronizationContext.Current)
            {
                _synchronizationContext.Post(o => handler.Invoke(this, EventArgs.Empty), null);
            }
            else
            {
                handler.Invoke(this, EventArgs.Empty);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Determine if we can execute. </summary>
        ///
        /// <param name="parameter">    The parameter. </param>
        ///
        /// <returns>   True if we can execute, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool CanExecute(T parameter)
        {
            return _canExecuteMethod == null || _canExecuteMethod(parameter);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the given parameter. </summary>
        ///
        /// <param name="parameter">    The parameter. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Execute(T parameter)
        {
            _executeMethod(parameter);
        }
    }
}