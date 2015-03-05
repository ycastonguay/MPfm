//// Copyright © 2011-2013 Yanick Castonguay
////
//// This file is part of Sessions.
////
//// Sessions is free software: you can redistribute it and/or modify
//// it under the terms of the GNU General Public License as published by
//// the Free Software Foundation, either version 3 of the License, or
//// (at your option) any later version.
////
//// Sessions is distributed in the hope that it will be useful,
//// but WITHOUT ANY WARRANTY; without even the implied warranty of
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//// GNU General Public License for more details.
////
//// You should have received a copy of the GNU General Public License
//// along with Sessions. If not, see <http://www.gnu.org/licenses/>.
//
//#if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
//
//using Sessions.Sound.BassNetWrapper;
//using Un4seen.Bass;
//
//namespace Sessions.Player.Objects
//{
//    /// <summary>
//    /// Defines synchronization callback properties to be used with the Player class.
//    /// </summary>
//    public class PlayerSyncProc
//    {
//        /// <summary>
//        /// Private value for the Handle property.
//        /// </summary>
//        private int handle = 0;
//        /// <summary>
//        /// Defines the synchronization callback handle.
//        /// </summary>
//        public int Handle
//        {
//            get
//            {
//                return handle;
//            }
//            set
//            {
//                handle = value;
//            }
//        }
//
//        /// <summary>
//        /// Private value for the SyncProc property.
//        /// </summary>
//        private SYNCPROC syncProc = null;
//        /// <summary>
//        /// Defines the synchronization callback (BASS.NET object).
//        /// </summary>
//        public SYNCPROC SyncProc
//        {
//            get
//            {
//                return syncProc;
//            }
//            set
//            {
//                syncProc = value;
//            }
//        }
//
//        /// <summary>
//        /// Default constructor for the SyncProc class.
//        /// </summary>
//        public PlayerSyncProc()
//        {
//        }
//    }
//}
//
//#endif