//
// XMLHelper.cs: Helper class for XML files (based on Linq to XML).
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

using System.Xml.Linq;

namespace MPfm.Core
{
    /// <summary>
    /// Helper class for XML files (based on Linq to XML).
    /// </summary>
    public static class XMLHelper
    {
        /// <summary>
        /// Returns the value of an attribute and converts it to the format
        /// specified in the generic type.
        /// </summary>
        /// <typeparam name="T">Format to convert</typeparam>
        /// <param name="element">XElement containing the attribute</param>
        /// <param name="name">Attribute name</param>
        /// <returns>Attribute value</returns>
        public static T GetAttributeValueGeneric<T>(XElement element, string name) //where T : struct
        {
            // Create variables
            T result = default(T);

            // Get attribute value
            XAttribute attribute = element.Attribute(name);
            if (attribute != null)
            {
                // Convert value
                result = Conversion.TryParse<T>(attribute.Value);                
            }

            return result;
        }

        /// <summary>
        /// Returns the value of an attribute.
        /// </summary>        
        /// <param name="element">XElement containing the attribute</param>
        /// <param name="name">Attribute name</param>
        /// <returns>Attribute value</returns>
        public static string GetAttributeValue(XElement element, string name)
        {
            // Get attribute value
            XAttribute attribute = element.Attribute(name);
            if (attribute != null)
            {
                return attribute.Value;
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns a new attribute. If the value is null, the value is changed
        /// to string.Empty (XAttribute doesn't support null values!).
        /// </summary>
        /// <param name="name">Attribute name</param>
        /// <param name="value">Attribute value</param>
        /// <returns>Attribute object</returns>
        public static XAttribute NewAttribute(string name, object value)
        {
            // Create attribute
            XAttribute attribute = new XAttribute(name, string.Empty);

            // Check if value is null
            if (value != null)
            {
                // Set value
                attribute.SetValue(value);
            }

            return attribute;
        }
    }
}