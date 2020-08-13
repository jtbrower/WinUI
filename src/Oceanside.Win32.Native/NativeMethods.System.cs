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

namespace Oceanside.Win32.Native
{
    using System.Runtime.InteropServices;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <content>   A native methods. </content>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public static partial class NativeMethods
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   System parameters information. </summary>
        ///
        /// <param name="nAction">  The action. </param>
        /// <param name="nParam">   The parameter. </param>
        /// <param name="value">    [in,out] The value. </param>
        /// <param name="ignore">   The ignore. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [DllImport(User32, SetLastError = true, CharSet = CharSet.Auto, BestFitMapping = false)]
        private static extern bool SystemParametersInfo(int nAction, int nParam, ref int value, int ignore);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Retrieves the keyboard repeat-speed setting, which is a value in the range from 0
        /// (approximately 2.5 repetitions per second) through 31 (approximately 30 repetitions per
        /// second). The actual repeat rates are hardware-dependent and may vary from a linear scale by
        /// as much as 20%. The pvParam parameter must point to a DWORD variable that receives the
        /// setting. 
        /// 
        /// Note that this function was suffixed with NonCached to indicate a potentially expensive
        /// operation that should probably be cached by the caller.
        /// See https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-systemparametersinfoa
        /// </summary>
        ///
        /// <returns>   The keyboard speed as repetitions per second. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static int GetKeyboardSpeedNonCached()
        {
            const int SPI_GETKEYBOARDSPEED = 0xA;
            var speed = 0;
            if (!SystemParametersInfo(SPI_GETKEYBOARDSPEED, 0, ref speed, 0))
            {
                WriteGlobalErrorMsgIfSet();
                return -1;
            }
            // 0,...,31 correspond to 1000/2.5=400,...,1000/30 ms
            return speed < 0 || speed > 31 ? -1 : (31 - speed) * (400 - 1000 / 30) / 31 + 1000 / 30;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Retrieves the keyboard repeat-delay setting, which is a value in the range from 0
        /// (approximately 250 ms delay) through 3 (approximately 1 second delay). The actual delay
        /// associated with each value may vary depending on the hardware. The pvParam parameter must
        /// point to an integer variable that receives the setting.
        ///  See https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-systemparametersinfoa
        ///  
        /// Note that this function was suffixed with NonCached to indicate a potentially expensive
        /// operation that should probably be cached by the caller.
        /// </summary>
        ///
        /// <returns>   The keyboard delay. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static int GetKeyboardDelayNonCached()
        {
            const int SPI_GETKEYBOARDDELAY = 0x16;
            var delay = 0;
            if (!SystemParametersInfo(SPI_GETKEYBOARDDELAY, 0, ref delay, 0))
            {
                WriteGlobalErrorMsgIfSet();
                return -1;
            }

            // SPI_GETKEYBOARDDELAY 0,1,2,3 correspond to 250,500,750,1000ms
            return delay < 0 || delay > 3 ? -1 : (delay + 1) * 250;
        }
    }
}
