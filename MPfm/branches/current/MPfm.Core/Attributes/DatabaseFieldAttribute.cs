//
// DatabaseFieldAttribute.cs: This attribute is used to map database fields and 
//                            object properties.
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
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace MPfm.Core
{
    /// <summary>
    /// This attribute is used to map database fields and object properties.
    /// </summary>
    public class DatabaseFieldAttribute : System.Attribute
    {
        /// <summary>
        /// Private value for the DatabaseFieldName property.
        /// </summary>
        private string databaseFieldName = string.Empty;
        /// <summary>
        /// Database field name.
        /// </summary>
        public string DatabaseFieldName
        {
            get
            {
                return databaseFieldName;
            }
        }

        /// <summary>
        /// Private value for the SaveToDatabase property.
        /// </summary>
        private bool saveToDatabase = true;
        /// <summary>
        /// Defines if the property should be saved into the database.
        /// By default, the value is true.
        /// </summary>
        public bool SaveToDatabase
        {
            get
            {
                return saveToDatabase;
            }
        }

        /// <summary>
        /// Default constructor for the DatabaseFieldNameAttribute class.
        /// </summary>
        /// <param name="saveToDatabase">Save property value to database</param>        
        public DatabaseFieldAttribute(bool saveToDatabase)
        {
            this.saveToDatabase = saveToDatabase;
        }

        /// <summary>
        /// Default constructor for the DatabaseFieldNameAttribute class.
        /// </summary>
        /// <param name="saveToDatabase">Save property value to database</param>
        /// <param name="databaseFieldName">Database field name</param>
        public DatabaseFieldAttribute(bool saveToDatabase, string databaseFieldName)
        {
            this.saveToDatabase = saveToDatabase;
            this.databaseFieldName = databaseFieldName;
        }
    }
}
