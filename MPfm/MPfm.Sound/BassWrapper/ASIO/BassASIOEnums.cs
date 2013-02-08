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

namespace MPfm.Sound.BassWrapper.ASIO
{
    public enum BASSASIOActive
    {
        BASS_ASIO_ACTIVE_DISABLED,
        BASS_ASIO_ACTIVE_ENABLED,
        BASS_ASIO_ACTIVE_PAUSED
    }

    public enum BASSASIOFormat
    {
        BASS_ASIO_FORMAT_UNKNOWN = -1,
        BASS_ASIO_FORMAT_16BIT = 16,
        BASS_ASIO_FORMAT_24BIT,
        BASS_ASIO_FORMAT_32BIT,
        BASS_ASIO_FORMAT_FLOAT
    }

    public enum BassAsioHandlerSyncType
    {
        SourceStalled,
        SourceResumed
    }

    [Flags]
    public enum BASSASIOInit
    {
        BASS_ASIO_DEFAULT = 0,
        BASS_ASIO_THREAD = 1
    }

    public enum BASSASIONotify
    {
        BASS_ASIO_NOTIFY_RATE = 1,
        BASS_ASIO_NOTIFY_RESET
    }

    [Flags]
    public enum BASSASIOReset
    {
        BASS_ASIO_RESET_ENABLE = 1,
        BASS_ASIO_RESET_JOIN = 2,
        BASS_ASIO_RESET_PAUSE = 4,
        BASS_ASIO_RESET_FORMAT = 8,
        BASS_ASIO_RESET_RATE = 16,
        BASS_ASIO_RESET_VOLUME = 32
    }
}
