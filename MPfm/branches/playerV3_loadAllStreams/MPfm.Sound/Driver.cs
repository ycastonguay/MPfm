//
// Driver.cs: This class is a data transfer object (DTO) representing a driver.
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
using System.Collections.Generic;
using System.Text;

namespace MPfm.Sound
{
    /// <summary>
    /// This class wraps up the Driver class of the FMOD library.
    /// </summary>
    public class Driver
    {
        /// <summary>
        /// Constructor for the Driver class. Requires the id and name
        /// of the driver.
        /// </summary>
        /// <param name="id">Driver Identifier</param>
        /// <param name="name">Driver Name</param>
        public Driver(int id, string name)
        {
            m_id = id;
            m_name = name;
        }

        private int m_id;
        /// <summary>
        /// Identifier of the driver.
        /// </summary>
        public int Id
        {
            get
            {
                return m_id;
            }
        }

        private string m_name;
        /// <summary>
        /// Name of the driver.
        /// </summary>
        public string Name
        {
            get
            {
                return m_name;
            }
        }
    }
}
