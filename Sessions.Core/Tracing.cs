// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Diagnostics;
using System.Threading;

namespace Sessions.Core
{
    /// <summary>
    /// This class manages basic tracing for files.
    /// </summary>
    public static class Tracing
    {
        /// <summary>
        /// Logs a message to configured trace listeners.
        /// </summary>
        /// <param name="message">Message to log</param>
        public static void Log(string message)
        {
            LogInternal(message);
        }

        /// <summary>
        /// Logs a message with composite format string to configured trace listeners.
        /// </summary>
        /// <param name="message">Composite format string</param>
        /// <param name="args">Object that contains zero or more objects</param>
        public static void Log(string format, params object[] args)
        {
            LogInternal(string.Format(format, args));
        }

        /// <summary>
        /// Logs an exception and its inner exception if available to configured trace listeners.
        /// </summary>
        /// <param name="ex">Exception</param>
        public static void Log(Exception ex)
        {
            string message = string.Format("Error occured: {0}", ex);
            if (ex.InnerException != null)
                message += string.Format("\nInner exception: {0}", ex.InnerException);

            LogInternal(message);
        }

        /// <summary>
        /// Logs the message to configured trace listeners (with time stamp)
        /// </summary>
        /// <param name="message"></param>
        public static void LogWithTimeStamp(string message)
        {            
            LogInternal(string.Format("[{0}] {1}", DateTime.Now, message));
        }

        private static void LogInternal(string message)
        {
#if IOS || ANDROID
            Console.WriteLine(message);
#elif WINDOWSSTORE || PCL || WINDOWS_PHONE
            Debug.WriteLine(message);
#else            
            //Trace.WriteLine(string.Format("[{0}/{1}] {2}", (int)AppDomain.GetCurrentThreadId(), Thread.CurrentThread.ManagedThreadId, message));
            Trace.WriteLine(string.Format("[{0}] {1}", Thread.CurrentThread.ManagedThreadId, message));
            Trace.Flush();
#endif
            
        }

    }
}
