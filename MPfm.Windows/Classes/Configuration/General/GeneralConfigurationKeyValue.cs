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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using MPfm.Core;

namespace MPfm
{
    /// <summary>
    /// Defines a key/value pair to be used in the MPfm general configuration section.
    /// </summary>
    public class GeneralConfigurationKeyValue
    {
        /// <summary>
        /// Key name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Key value.
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// Value type (boolean, integer, etc.).
        /// </summary>
        public Type ValueType { get; set; }

        /// <summary>
        /// Default constructor for the GeneralConfigurationKeyValue class.
        /// </summary>
        public GeneralConfigurationKeyValue()
        {
            // Set default values
            Name = string.Empty;
            Value = null;
            ValueType = typeof(Int32);
        }

        /// <summary>
        /// Gets the value of the current key using the specified generic type.
        /// </summary>
        /// <typeparam name="T">Return value type</typeparam>
        /// <returns>Value</returns>
        public T GetValue<T>()
        {
            // Set default value
            T value = default(T);

            // Convert value
            value = Conversion.TryParse<T>(Value.ToString());

            return value;
        }
    }
}
