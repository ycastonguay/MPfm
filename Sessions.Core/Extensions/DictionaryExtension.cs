// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;

namespace Sessions.Core.Extensions
{
    /// <summary>
    /// Extensions for the Dictionary class.
    /// </summary>
    public static class DictionaryExtension
    {
        /// <summary>
        /// Finds a key by its value. Returns the first result if multiple results are found.        
        /// </summary>
        /// <typeparam name="TKey">Key (generic)</typeparam>
        /// <typeparam name="TValue">Value (generic)</typeparam>
        /// <param name="dictionary">Current dictionary instance</param>
        /// <param name="value">Value</param>
        /// <returns>Key</returns>
        public static TKey FindKeyByValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue value)
        {
            // Check if dictionary is valid
            if (dictionary == null)
                throw new ArgumentNullException("The directory parameter value is null.");

            // Loop through key/value pairs and try to find the key
            foreach (KeyValuePair<TKey, TValue> pair in dictionary)
                if (value.Equals(pair.Value)) return pair.Key;

            return default(TKey);
        }
    }
}
