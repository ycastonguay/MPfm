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

namespace MPfm.Sound.BassWrapper.Wasapi
{
	[Flags]
	public enum BASSWASAPIDeviceInfo
	{
		BASS_DEVICE_UNKNOWN = 0,
		BASS_DEVICE_ENABLED = 1,
		BASS_DEVICE_DEFAULT = 2,
		BASS_DEVICE_INIT = 4,
		BASS_DEVICE_LOOPBACK = 8,
		BASS_DEVICE_INPUT = 16
	}

	[Flags]
	public enum BASSWASAPIDeviceType
	{
		BASS_WASAPI_TYPE_NETWORKDEVICE = 0,
		BASS_WASAPI_TYPE_SPEAKERS = 1,
		BASS_WASAPI_TYPE_LINELEVEL = 2,
		BASS_WASAPI_TYPE_HEADPHONES = 3,
		BASS_WASAPI_TYPE_MICROPHONE = 4,
		BASS_WASAPI_TYPE_HEADSET = 5,
		BASS_WASAPI_TYPE_HANDSET = 6,
		BASS_WASAPI_TYPE_DIGITAL = 7,
		BASS_WASAPI_TYPE_SPDIF = 8,
		BASS_WASAPI_TYPE_HDMI = 9,
		BASS_WASAPI_TYPE_UNKNOWN = 10
	}

	public enum BASSWASAPIFormat
	{
		BASS_WASAPI_FORMAT_UNKNOWN = -1,
		BASS_WASAPI_FORMAT_FLOAT,
		BASS_WASAPI_FORMAT_8BIT,
		BASS_WASAPI_FORMAT_16BIT,
		BASS_WASAPI_FORMAT_24BIT,
		BASS_WASAPI_FORMAT_32BIT
	}

	public enum BassWasapiHandlerSyncType
	{
		SourceStalled,
		SourceResumed
	}

	[Flags]
	public enum BASSWASAPIInit
	{
		BASS_WASAPI_SHARED = 0,
		BASS_WASAPI_EXCLUSIVE = 1,
		BASS_WASAPI_AUTOFORMAT = 2,
		BASS_WASAPI_BUFFER = 4
	}

	public enum BASSWASAPINotify
	{
		BASS_WASAPI_NOTIFY_CHANGE,
		BASS_WASAPI_NOTIFY_DEFOUTPUT,
		BASS_WASAPI_NOTIFY_DEFINPUT
	}
}
