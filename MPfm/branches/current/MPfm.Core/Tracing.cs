//
// Tracing.cs: Tracing routines
//
// Copyright © 2011-2012 Yanick Castonguay
//
// This file is part of MPfm.
//
// MPfm is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// MPfm is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with MPfm. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MPfm.Core
{
    /// <summary>
    /// This class manages basic tracing for files.
    /// </summary>
    public static class Tracing
    {
        /// <summary>
        /// Logs the message to configured trace listeners.
        /// </summary>
        /// <param name="message">Message to log</param>
        public static void Log(string message)
        {
            Trace.WriteLine("[" + DateTime.Now.ToString() + "] " + message);
            Trace.Flush();
        }

        /// <summary>
        /// Logs an exception and its inner exception if available to configured trace listeners.
        /// </summary>
        /// <param name="ex">Exception</param>
        public static void Log(Exception ex)
        {
            Trace.WriteLine("Error occured: " + ex.Message + "\n" + ex.StackTrace);

            if (ex.InnerException != null)
            {
                Trace.WriteLine("Inner exception: " + ex.InnerException.Message + "\n" + ex.InnerException.StackTrace);
            }
        }

        /// <summary>
        /// Logs the message to configured trace listeners (without time stamp)
        /// </summary>
        /// <param name="message"></param>
        public static void LogWithoutTimeStamp(string message)
        {
            Trace.WriteLine(message);
            Trace.Flush();
        }

    }
}
