//
// Timer.cs: High resolution timer.
//
// Copyright © 2011 Yanick Castonguay
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
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace MPfm.Sound
{
    /// <summary>
    /// This Timer class is a high resolution Timer.
    /// I don't remember where I took this code.
    /// </summary>
    public class Timer : IComponent
    {
        #region DllImport
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        [DllImport("winmm.dll")]
        private static extern int timeGetDevCaps(ref TimerCaps caps, int sizeOfTimerCaps);
        
        [DllImport("winmm.dll")]
        private static extern int timeSetEvent(int delay, int resolution, TimerProc proc, int user, int mode);
        
        [DllImport("winmm.dll")] 
        private static extern int timeKillEvent(int id);

        [StructLayout(LayoutKind.Sequential)]
        public struct TimerCaps
        {
            public int periodMin;
            public int periodMax;
        }
        
        private delegate void TimerProc(int id, int msg, int user, int param1, int param2);        
        private TimerProc timerProc;

        #endregion

        #region Properties

        private int m_resolution = 1;
        public int Resolution
        {
            get
            {
                return m_resolution;
            }
        }

        private int m_period = 0;
        public int Period
        {
            get
            {
                return m_period;
            }
            set
            {
                if (!m_running)
                {
                    m_period = value;
                }
            }
        }

        private bool m_running = false;
        public bool Running
        {
            get
            {
                return m_running;
            }
        }

        private TimerCaps m_timerCaps;
        public TimerCaps TimerCapabilities
        {
            get
            {
                return m_timerCaps;
            }
        }

        private int m_timerId;
        public int TimerId
        {
            get
            {
                return m_timerId;
            }
        }

        #endregion               

        // Constructor
        public Timer()
        {            
            timeGetDevCaps(ref m_timerCaps, Marshal.SizeOf(m_timerCaps));
            timerProc = new TimerProc(TimerProcCallback);
            m_period = TimerCapabilities.periodMin;
            m_resolution = 1;
            m_running = false;
        }

        private void TimerProcCallback(int id, int msg, int user, int param1, int param2)
        {
            OnTick(EventArgs.Empty);
        }

        #region Methods

        // Start the timer
        public void Start()
        {
            m_running = true;
            m_timerId = timeSetEvent(Period, Resolution, timerProc, 0, 1);
        }

        // Stop the timer
        public void Stop()
        {
            m_running = false;
            int result = timeKillEvent(m_timerId);
        }

        #endregion

        #region Event Handlers

        public event EventHandler Tick;
        private void OnTick(EventArgs e)
        {
            EventHandler handler = Tick;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region IComponent Members

        public event EventHandler Disposed;

        private ISite site = null;
        public ISite Site
        {
            get
            {
                return site;
            }
            set
            {
                site = value;
            }
        }

        public void Dispose()
        {
            //OnDisposed(EventArgs.Empty);
        }

        #endregion
    }
}

