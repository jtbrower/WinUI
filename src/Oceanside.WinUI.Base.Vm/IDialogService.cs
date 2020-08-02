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

using System.Threading.Tasks;

namespace Oceanside.WinUI.Base.Vm
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Interface for dialog service. </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public interface IDialogService
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Confirm asynchronous. </summary>
        ///
        /// <param name="title">            The title. </param>
        /// <param name="message">          The message. </param>
        /// <param name="acceptButtonTxt">  (Optional) The accept button text. </param>
        /// <param name="closeButtonTxt">   (Optional) The close button text. </param>
        ///
        /// <returns>   An asynchronous result that yields the confirm. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        Task<bool> ConfirmAsync(string title, string message, string? acceptButtonTxt = null, string? closeButtonTxt = null);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Warning asynchronous. </summary>
        ///
        /// <param name="title">    The title. </param>
        /// <param name="message">  The message. </param>
        ///
        /// <returns>   An asynchronous result. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        Task WarnAsync(string title, string message);
    }
}