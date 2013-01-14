//
// OS.cs: Operating system detection class and enum.
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

namespace MPfm.Core
{
	/// <summary>
	/// The ImageManipulation class contains static functions for manipulating images.
	/// </summary>
	public static class OS
	{
		/// <summary>
		/// Returns the operating system type.
		/// </summary>
		/// <returns>Operating system type (Windows, Linux or Mac OS X)</returns>
		public static OSType Type
		{
			get
			{
			// Check platform
			OSType osType = OSType.Windows;
			switch(Environment.OSVersion.Platform)
			{
				case PlatformID.Win32NT:
				case PlatformID.Win32S:
				case PlatformID.Win32Windows:
				case PlatformID.WinCE:
					osType = OSType.Windows;
					break;
				case PlatformID.Unix:
					// Sometimes Mac OS X is detected as Unix.
					// The version returned by Linux is actually the kernel version, so it is not higher than 3 at the moment.
					// Mac OS X returns a higher number (Unix 11.3 when I tested under Mac OS X 10.7.3), so we can detect the platform that way.
					// Source: http://lists.ximian.com/pipermail/mono-osx/2007-December/001094.html
					if(Environment.OSVersion.Version.Major > 8)
					{
						osType = OSType.MacOSX;
					}
					else
					{
						osType = OSType.Linux;
					}
					break;
				case PlatformID.MacOSX:
					osType = OSType.MacOSX;
					break;
			}
			return osType;
			}
		}
	}

	/// <summary>
	/// Defines the operating system (Windows, Linux or Mac OS X).
	/// </summary>
	public enum OSType
	{
		/// <summary>
		/// Microsoft Windows (any version).
		/// </summary>
		Windows = 0,
		/// <summary>
		/// Linux (any distribution).
		/// </summary>
		Linux = 1,
		/// <summary>
		/// Mac OS X (any verison).
		/// </summary>
		MacOSX = 2
	}
}
