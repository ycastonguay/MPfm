// Copyright © 2011-2013 Yanick Castonguay
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
using System.Security;

namespace MPfm.Sound.BassWrapper.Mix
{
	[SuppressUnmanagedCodeSecurity]
	public sealed class BassMix
	{
		public const int BASSMIXVERSION = 516;
		[DllImport(BassWrapperGlobals.DllImportValue_BassMix, CharSet = CharSet.Auto)]
		public static extern int BASS_Mixer_GetVersion();
		public static Version BASS_Mixer_GetVersion(int fieldcount)
		{
			if (fieldcount < 1)
			{
				fieldcount = 1;
			}
			if (fieldcount > 4)
			{
				fieldcount = 4;
			}
			int num = BassMix.BASS_Mixer_GetVersion();
			Version result = new Version(2, 4);
			switch (fieldcount)
			{
				case 1:
					result = new Version(num >> 24 & 255, 0);
					break;
				case 2:
					result = new Version(num >> 24 & 255, num >> 16 & 255);
					break;
				case 3:
					result = new Version(num >> 24 & 255, num >> 16 & 255, num >> 8 & 255);
					break;
				case 4:
					result = new Version(num >> 24 & 255, num >> 16 & 255, num >> 8 & 255, num & 255);
					break;
			}
			return result;
		}
		[DllImport(BassWrapperGlobals.DllImportValue_BassMix, CharSet = CharSet.Auto)]
		public static extern int BASS_Mixer_StreamCreate(int freq, int chans, BASSFlag flags);
		[DllImport(BassWrapperGlobals.DllImportValue_BassMix, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_Mixer_StreamAddChannel(int handle, int channel, BASSFlag flags);
		[DllImport(BassWrapperGlobals.DllImportValue_BassMix, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_Mixer_StreamAddChannelEx(int handle, int channel, BASSFlag flags, long start, long length);
		[DllImport(BassWrapperGlobals.DllImportValue_BassMix, CharSet = CharSet.Auto)]
		public static extern int BASS_Mixer_ChannelGetData(int handle, IntPtr buffer, int length);
		[DllImport(BassWrapperGlobals.DllImportValue_BassMix, CharSet = CharSet.Auto)]
		public static extern int BASS_Mixer_ChannelGetData(int handle, [In] [Out] float[] buffer, int length);
		[DllImport(BassWrapperGlobals.DllImportValue_BassMix, CharSet = CharSet.Auto)]
		public static extern int BASS_Mixer_ChannelGetData(int handle, [In] [Out] short[] buffer, int length);
		[DllImport(BassWrapperGlobals.DllImportValue_BassMix, CharSet = CharSet.Auto)]
		public static extern int BASS_Mixer_ChannelGetData(int handle, [In] [Out] int[] buffer, int length);
		[DllImport(BassWrapperGlobals.DllImportValue_BassMix, CharSet = CharSet.Auto)]
		public static extern int BASS_Mixer_ChannelGetData(int handle, [In] [Out] byte[] buffer, int length);
		[DllImport(BassWrapperGlobals.DllImportValue_BassMix, CharSet = CharSet.Auto)]
		public static extern BASSFlag BASS_Mixer_ChannelFlags(int handle, BASSFlag flags, BASSFlag mask);
		public static bool BASS_Mixer_ChannelPause(int handle)
		{
			return BassMix.BASS_Mixer_ChannelFlags(handle, BASSFlag.BASS_STREAM_PRESCAN, BASSFlag.BASS_STREAM_PRESCAN) > BASSFlag.BASS_DEFAULT;
		}
		public static bool BASS_Mixer_ChannelPlay(int handle)
		{
			return BassMix.BASS_Mixer_ChannelFlags(handle, BASSFlag.BASS_DEFAULT, BASSFlag.BASS_STREAM_PRESCAN) >= BASSFlag.BASS_DEFAULT;
		}
		public static BASSActive BASS_Mixer_ChannelIsActive(int handle)
		{
			BASSFlag bASSFlag = BassMix.BASS_Mixer_ChannelFlags(handle, BASSFlag.BASS_STREAM_PRESCAN, BASSFlag.BASS_DEFAULT);
			if (bASSFlag < BASSFlag.BASS_DEFAULT)
			{
				return BASSActive.BASS_ACTIVE_STOPPED;
			}
			if ((bASSFlag & BASSFlag.BASS_STREAM_PRESCAN) != BASSFlag.BASS_DEFAULT)
			{
				return BASSActive.BASS_ACTIVE_PAUSED;
			}
			return BASSActive.BASS_ACTIVE_PLAYING;
		}
		[DllImport(BassWrapperGlobals.DllImportValue_BassMix, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_Mixer_ChannelRemove(int handle);
		[DllImport(BassWrapperGlobals.DllImportValue_BassMix, CharSet = CharSet.Auto)]
		public static extern int BASS_Mixer_ChannelGetMixer(int handle);
		[DllImport(BassWrapperGlobals.DllImportValue_BassMix, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_Mixer_ChannelSetPosition(int handle, long pos, BASSMode mode);
		public static bool BASS_Mixer_ChannelSetPosition(int handle, long pos)
		{
			return BassMix.BASS_Mixer_ChannelSetPosition(handle, pos, BASSMode.BASS_POS_BYTES);
		}
		[DllImport(BassWrapperGlobals.DllImportValue_BassMix, CharSet = CharSet.Auto)]
		public static extern long BASS_Mixer_ChannelGetPosition(int handle, BASSMode mode);
		[DllImport(BassWrapperGlobals.DllImportValue_BassMix, CharSet = CharSet.Auto)]
		public static extern long BASS_Mixer_ChannelGetPositionEx(int handle, BASSMode mode, int delay);
		public static long BASS_Mixer_ChannelGetPosition(int handle)
		{
			return BassMix.BASS_Mixer_ChannelGetPosition(handle, BASSMode.BASS_POS_BYTES);
		}
		[DllImport(BassWrapperGlobals.DllImportValue_BassMix, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_Mixer_ChannelSetMatrix(int handle, float[,] matrix);
		[DllImport(BassWrapperGlobals.DllImportValue_BassMix, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_Mixer_ChannelGetMatrix(int handle, [In] [Out] float[,] matrix);
		[DllImport(BassWrapperGlobals.DllImportValue_BassMix, CharSet = CharSet.Auto)]
		public static extern long BASS_Mixer_ChannelGetEnvelopePos(int handle, BASSMIXEnvelope type, ref float value);
		[DllImport(BassWrapperGlobals.DllImportValue_BassMix, CharSet = CharSet.Auto)]
		public static extern long BASS_Mixer_ChannelGetEnvelopePos(int handle, BASSMIXEnvelope type, [MarshalAs(UnmanagedType.AsAny)] [In] [Out] object value);
		[DllImport(BassWrapperGlobals.DllImportValue_BassMix, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_Mixer_ChannelSetEnvelopePos(int handle, BASSMIXEnvelope type, long pos);
		[DllImport(BassWrapperGlobals.DllImportValue_BassMix, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_Mixer_ChannelSetEnvelope(int handle, BASSMIXEnvelope type, BASS_MIXER_NODE[] nodes, int count);
		public static bool BASS_Mixer_ChannelSetEnvelope(int handle, BASSMIXEnvelope type, BASS_MIXER_NODE[] nodes)
		{
			return BassMix.BASS_Mixer_ChannelSetEnvelope(handle, type, nodes, (nodes == null) ? 0 : nodes.Length);
		}
		[DllImport(BassWrapperGlobals.DllImportValue_BassMix, CharSet = CharSet.Auto)]
		public static extern int BASS_Mixer_ChannelSetSync(int handle, BASSSync type, long param, SYNCPROC proc, IntPtr user);
		[DllImport(BassWrapperGlobals.DllImportValue_BassMix, CharSet = CharSet.Auto)]
		private static extern int BASS_Mixer_ChannelSetSync(int A_0, BASSSync A_1, long A_2, SYNCPROCEX A_3, IntPtr A_4);
		public static int BASS_Mixer_ChannelSetSyncEx(int handle, BASSSync type, long param, SYNCPROCEX proc, IntPtr user)
		{
			type |= (BASSSync)16777216;
			return BassMix.BASS_Mixer_ChannelSetSync(handle, type, param, proc, user);
		}
		[DllImport(BassWrapperGlobals.DllImportValue_BassMix, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_Mixer_ChannelRemoveSync(int handle, int sync);
		[DllImport(BassWrapperGlobals.DllImportValue_BassMix, CharSet = CharSet.Auto)]
		public static extern int BASS_Split_StreamCreate(int channel, BASSFlag flags, int[] mapping);
		[DllImport(BassWrapperGlobals.DllImportValue_BassMix, CharSet = CharSet.Auto)]
		public static extern int BASS_Split_StreamGetSource(int handle);
		[DllImport(BassWrapperGlobals.DllImportValue_BassMix, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_Split_StreamReset(int handle);
		[DllImport(BassWrapperGlobals.DllImportValue_BassMix, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BASS_Split_StreamResetEx(int handle, int offset);
		[DllImport(BassWrapperGlobals.DllImportValue_BassMix, CharSet = CharSet.Auto)]
		private static extern int BASS_Split_StreamGetSplits(int A_0, [In] [Out] int[] A_1, int A_2);
		public static int[] BASS_Split_StreamGetSplits(int handle)
		{
			int num = BassMix.BASS_Split_StreamGetSplits(handle, null, 0);
			if (num < 0)
			{
				return null;
			}
			int[] array = new int[num];
			num = BassMix.BASS_Split_StreamGetSplits(handle, array, num);
			if (num < 0)
			{
				return null;
			}
			return array;
		}
		[DllImport(BassWrapperGlobals.DllImportValue_BassMix, CharSet = CharSet.Auto)]
		public static extern int BASS_Split_StreamGetAvailable(int handle);
	}
}
