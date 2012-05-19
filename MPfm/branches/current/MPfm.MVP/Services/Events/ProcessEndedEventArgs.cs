//
// ProcessEndedEventArgs.cs: Process ended event arguments.
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
using System.IO;
using System.Reflection;

namespace MPfm.MVP
{
	/// <summary>
	/// Process ended event arguments.
	/// </summary>
	public class ProcessEndedEventArgs : EventArgs
	{
		/// <summary>
		/// Defines if the process was canceled or not.
		/// </summary>
		public bool Canceled { get; set; }
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MPfm.MVP.ProcessEndedEventArgs"/> class.
		/// </summary>
		/// <param name='canceled'>Defines if the process was canceled or not.</param>
		public ProcessEndedEventArgs(bool canceled)
		{
			this.Canceled = canceled;
		}
	}
}

