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

#if WINDOWSSTORE || WINDOWS_PHONE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MPfm.Core;
using MPfm.Core.Attributes;
using MPfm.Core.Extensions;
using MPfm.Library.Database.Interfaces;
using SQLite;

namespace MPfm.Library.Database
{
    /// <summary>
    /// The SQLiteGateway class is a data adapter class which makes it easier to select, insert, update and delete
    /// data from the database.
    /// </summary>
    public class WinRTSQLiteGateway : ISQLiteGateway
    {
        private SQLiteConnection _connection = null;

        private string _databaseFilePath;
        /// <summary>
        /// Database file path.
        /// </summary>
        public string DatabaseFilePath
        {
            get
            {
                return _databaseFilePath;
            }
        }

        /// <summary>
        /// Default constructor for the SQLiteGateway class.
        /// </summary>
        /// <param name="databaseFilePath">Database file path</param>
        public WinRTSQLiteGateway(string databaseFilePath)
        {
            _databaseFilePath = databaseFilePath;
        }

        /// <summary>
        /// Creates a new database file.
        /// </summary>
        public static void CreateDatabaseFile(string databaseFilePath)
        {     
            // Just create a connection, if the file doesn't exist, it will create the file.
        }

        /// <summary>
        /// Generates a connection based on the current database file path.
        /// </summary>
        /// <returns>Database connection</returns>
        public SQLiteConnection GenerateConnection()
        {
            return new SQLiteConnection(_databaseFilePath, true);
        }

        /// <summary>
        /// Returns the list of properties which have different database field names.
        /// </summary>
        /// <typeparam name="T">Object to scan (generic)</typeparam>
        /// <returns>List of DatabaseFieldMap</returns>
        public List<DatabaseFieldMap> GetMap<T>()
        {
            // Create map by scanning properties
            List<DatabaseFieldMap> maps = new List<DatabaseFieldMap>();
            var propertyInfos = typeof(T).GetTypeInfo().DeclaredProperties;
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                // Scan attributes
                var attributes = propertyInfo.GetCustomAttributes();
                foreach (var attribute in attributes)
                {
                    // Try to cast into attribute map
                    DatabaseFieldAttribute attrMap = attribute as DatabaseFieldAttribute;
                    if (attrMap != null)
                        maps.Add(new DatabaseFieldMap(propertyInfo.Name, attrMap.DatabaseFieldName, attrMap.SaveToDatabase));
                }
            }

            return maps;
        }

        /// <summary>
        /// Formats a value for a SQL command.
        /// If the value type is String or Guid, quotes will be added to the value.
        /// If the value type is DBNull, a null value will be added (without quotes).
        /// </summary>
        /// <param name="value">Value to format</param>
        /// <returns>Formatted value</returns>
        public string FormatSQLValue(object value)
        {           
            // Check value type
            if (value == null)
            {
                return "null";
            }
            else if (value.GetType().FullName.ToUpper() == "SYSTEM.DATETIME")
            {
                return "'" + value.ToString() + "'";
            }
            else if (value.GetType().FullName.ToUpper() == "SYSTEM.BOOLEAN")
            {
                int newValue = ((bool)value) ? 1 : 0;
                return newValue.ToString();
            }
            else if (value.GetType().FullName.ToUpper() == "SYSTEM.STRING" ||
                     value.GetType().FullName.ToUpper() == "SYSTEM.GUID" ||
                     value.GetType().GetTypeInfo().IsEnum)
            {
                // Replace single quotes by two quotes
                return "'" + value.ToString().Replace("'", "''") + "'";
            }
            else
            {
                return value.ToString();
            }
        }

        /// <summary>
        /// Executes a SQL query and returns the number of rows affected.
        /// </summary>
        /// <param name="sql">SQL query</param>
        /// <returns>Number of rows affected</returns>
        public int Execute(string sql)
        {
            SQLiteConnection connection = null;            

            try
            {                
                // Create and open connection
                connection = GenerateConnection();
                int rows = connection.Execute(sql, new object[0]);
                return rows;
            }
            catch
            {
                throw;
            }
            finally
            {                
                connection.Close();
                connection.Dispose();
            }
        }

        /// <summary>
        /// Executes a scalar SQL query and returns the query value.
        /// </summary>
        /// <param name="sql">SQL query</param>
        /// <returns>Scalar query value</returns>
        public object ExecuteScalar(string sql)
        {
            SQLiteConnection connection = null;

            try
            {
                connection = GenerateConnection();
                object obj = connection.ExecuteScalar<object>(sql, new object[0]);
                return obj;
            }
            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /// <summary>
        /// Compacts the database.
        /// </summary>
        public void CompactDatabase()
        {
            // Compact database
            Execute("VACUUM;");
        }

        /// <summary>
        /// Selects a list of objects.
        /// </summary>
        /// <param name="sql">Query to execute (must have only one field in the select statement)</param>
        /// <returns>List of objects</returns>
        public IEnumerable<object> SelectList(string sql)
        {
            SQLiteConnection connection = null;
            SQLiteCommand command = null;
            List<object> list = new List<object>();

            try
            {
                //connection = GenerateConnection();                
                //connection.Get<object>()
                //connection.CreateCommand()
                //connection.Open();

                //// Create command
                //command = factory.CreateCommand();
                //command.CommandText = sql;
                //command.Connection = connection;

                //// Create and execute reader
                //reader = command.ExecuteReader();
                //while (reader.Read())
                //{
                //    // Add key/value to dictionary
                //    object field = reader.GetValue(0);
                //    list.Add(field);
                //}

                return list;
            }
            catch
            {
                throw;
            }
            finally
            {
                //// Clean up reader and _connection
                //if (reader != null)
                //{
                //    reader.Close();
                //    reader.Dispose();
                //}
				
                //// Dispose command
                //command.Dispose();

                //// Close and clean up _connection
                //if (connection.State == ConnectionState.Open)
                //{
                //    connection.Close();
                //    connection.Dispose();
                //}
            }
        }

        /// <summary>
        /// Selects a list of tuples of two objects.
        /// </summary>
        /// <param name="sql">Query to execute (must have only two fields in the select statement)</param>
        /// <returns>List of tuple</returns>
        public List<Tuple<object, object>> SelectTuple(string sql)
        {
            SQLiteConnection connection = null;
            //DbDataReader reader = null; // No reader for WInRT... argh!            
            SQLiteCommand command = null;
            List<Tuple<object, object>> listTuple = new List<Tuple<object, object>>();

            return listTuple;

            //try
            //{
            //    // Create and open _connection
            //    connection = GenerateConnection();
            //    connection.Open();

            //    // Create command
            //    command = factory.CreateCommand();
            //    command.CommandText = sql;
            //    command.Connection = connection;

            //    // Create and execute reader
            //    reader = command.ExecuteReader();
            //    while (reader.Read())
            //    {
            //        // Add key/value to dictionary
            //        object field1 = reader.GetValue(0);
            //        object field2 = reader.GetValue(1);                    
            //        listTuple.Add(new Tuple<object, object>(field1, field2));
            //    }

            //    return listTuple;
            //}
            //catch
            //{
            //    throw;
            //}
            //finally
            //{
            //    // Clean up reader and _connection
            //    if (reader != null)
            //    {
            //        reader.Close();
            //        reader.Dispose();
            //    }

            //    // Dispose command
            //    command.Dispose();

            //    // Close and clean up _connection
            //    if (connection.State == ConnectionState.Open)
            //    {
            //        connection.Close();
            //        connection.Dispose();
            //    }
            //}
        }

        /// <summary>
        /// Executes a select query and returns the first item of a list of objects as specified in 
        /// the generics type.
        /// </summary>
        /// <typeparam name="T">Object tye to fill</typeparam>
        /// <param name="sql">Query to execute</param>
        /// <returns>Object</returns>
        public T SelectOne<T>(string sql) where T : new()
        {
            // Select first from list
            List<T> list = Select<T>(sql);
            if (list != null && list.Count > 0)
            {
                return list[0];
            }
            return default(T);
        }

        /// <summary>
        /// Executes a select query and returns a list of objects as specified in the generics type.
        /// </summary>
        /// <typeparam name="T">Object tye to fill</typeparam>
        /// <param name="sql">Query to execute</param>
        /// <returns>List of objects</returns>
        public List<T> Select<T>(string sql) where T : new()
        {
            SQLiteConnection connection = null;
            SQLiteCommand command = null;
            List<T> list = new List<T>();
            var maps = GetMap<T>();

            try
            {
                //var mapping = new TableMapping(T);
                connection = GenerateConnection();                
                command = connection.CreateCommand(sql, new object[0]);
                list = command.ExecuteQuery<T>();                

                return list;
            }
            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }

            return list;
        }

        /// <summary>
        /// Updates an object into the database.
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="obj">Object to update</param>
        /// <param name="tableName">Database table name</param>
        /// <param name="whereFieldName">Where clause field name (followed by equals to)</param>
        /// <param name="whereValue">Where clause field value</param>
        /// <returns>Number of rows affected</returns>
        public int Update<T>(T obj, string tableName, string whereFieldName, object whereValue)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add(whereFieldName, whereValue);
            return Update<T>(obj, tableName, dict);
        }

        /// <summary>
        /// Updates an object into the database.
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="obj">Object to update</param>
        /// <param name="tableName">Database table name</param>
        /// <param name="where">Dictionary containing the where clause (equals to)</param>
        /// <returns>Number of rows affected</returns>
        public int Update<T>(T obj, string tableName, Dictionary<string, object> where)
        {            
            SQLiteConnection connection = null;            
            SQLiteCommand command = null;
            var maps = GetMap<T>();
            StringBuilder sql = new StringBuilder();

            return 0;

            //try
            //{
            //    // Generate query
            //    sql.AppendLine("UPDATE [" + tableName + "] SET ");

            //    // Scan through properties
            //    PropertyInfo[] propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            //    bool addedOneItem = false;
            //    for (int a = 0; a < propertyInfos.Length; a++)
            //    {
            //        PropertyInfo propertyInfo = propertyInfos[a];
            //        if (propertyInfo.GetSetMethod() != null)
            //        {
            //            string fieldName = propertyInfo.Name;                    
            //            if (dictMap.ContainsValue(propertyInfo.Name))
            //                fieldName = dictMap.FindKeyByValue<string, string>(propertyInfo.Name); 

            //            // Add comma if an item was added previously
            //            if(!addedOneItem)
            //                addedOneItem = true;
            //            else
            //                sql.Append(", ");

            //            // Add database field name
            //            sql.Append("[" + fieldName + "]=");
            //            object value = propertyInfo.GetValue(obj, null);
            //            sql.Append(FormatSQLValue(value));
            //            sql.Append("\n");
            //        }
            //    }

            //    // Generate where clause
            //    sql.AppendLine(" WHERE ");
            //    addedOneItem = false;
            //    for(int a = 0; a < where.Count; a++)
            //    {
            //        // Add comma if an item was added previously
            //        if(!addedOneItem)
            //            addedOneItem = true;
            //        else
            //            sql.Append(", ");

            //        KeyValuePair<string, object> keyValue = where.ElementAt(a);
            //        sql.AppendLine("[" + keyValue.Key + "]=");
            //        sql.Append(FormatSQLValue(keyValue.Value));

            //        // Add an AND keyword if this isn't the last item
            //        if (a < where.Count - 1)
            //        {
            //            sql.Append(" AND ");
            //        }
            //        sql.Append("\n");
            //    }

            //    // Create and open _connection
            //    connection = GenerateConnection();
            //    connection.Open();

            //    // Create command
            //    command = factory.CreateCommand();
            //    command.CommandText = sql.ToString();
            //    command.Connection = connection;

            //    // Execute command
            //    int count = command.ExecuteNonQuery();
            //    return count;
            //}
            //catch
            //{
            //    throw;
            //}
            //finally
            //{
            //    // Dispose command
            //    command.Dispose();

            //    // Close and clean up _connection
            //    if (connection.State == ConnectionState.Open)
            //    {
            //        connection.Close();
            //        connection.Dispose();
            //    }
            //}
        }

        /// <summary>
        /// Inserts an object into the database.
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="obj">Object to insert</param>
        /// <param name="tableName">Database table name</param>        
        /// <returns>Number of rows affected</returns>
        public int Insert<T>(T obj, string tableName)
        {
            SQLiteConnection connection = null;
            SQLiteCommand command = null;
            var map = GetMap<T>();
            StringBuilder sql = new StringBuilder();

            return 0;

            //try
            //{
            //    // Generate query
            //    sql.AppendLine("INSERT INTO [" + tableName + "] (");

            //    // Scan through properties to set column names
            //    PropertyInfo[] propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            //    bool addedOneItem = false;
            //    for (int a = 0; a < propertyInfos.Length; a++)
            //    {
            //        PropertyInfo propertyInfo = propertyInfos[a];
            //        if (propertyInfo.GetSetMethod() != null)
            //        {
            //            string fieldName = propertyInfo.Name;
            //            if (dictMap.ContainsValue(propertyInfo.Name))
            //                fieldName = dictMap.FindKeyByValue<string, string>(propertyInfo.Name);

            //            // Add comma if an item was added previously
            //            if(!addedOneItem)
            //                addedOneItem = true;
            //            else
            //                sql.Append(", ");

            //            sql.Append("[" + fieldName + "]");
            //            sql.Append("\n");
            //        }
            //    }
            //    sql.AppendLine(") VALUES (");

            //    // Scan through properties and set values
            //    addedOneItem = false;
            //    for (int a = 0; a < propertyInfos.Length; a++)
            //    {
            //        PropertyInfo propertyInfo = propertyInfos[a];
            //        if (propertyInfo.GetSetMethod() != null)
            //        {
            //            string fieldName = propertyInfo.Name;
            //            if (dictMap.ContainsValue(propertyInfo.Name))
            //                fieldName = dictMap.FindKeyByValue<string, string>(propertyInfo.Name);

            //            // Add comma if an item was added previously
            //            if(!addedOneItem)
            //                addedOneItem = true;
            //            else
            //                sql.Append(", ");

            //            // Get value and determine how to add field value
            //            object value = propertyInfo.GetValue(obj, null);
            //            sql.Append(FormatSQLValue(value));                       
            //            sql.Append("\n");
            //        }
            //    }
            //    sql.AppendLine(") ");

            //    // Create and open _connection
            //    connection = GenerateConnection();
            //    connection.Open();

            //    // Create command
            //    command = factory.CreateCommand();
            //    command.CommandText = sql.ToString();
            //    command.Connection = connection;

            //    // Execute command
            //    int count = command.ExecuteNonQuery();
            //    return count;
            //}
            //catch
            //{
            //    throw;
            //}
            //finally
            //{
            //    // Dispose command
            //    command.Dispose();

            //    // Close and clean up _connection
            //    if (connection.State == ConnectionState.Open)
            //    {
            //        connection.Close();
            //        connection.Dispose();
            //    }
            //}
        }

        /// <summary>
        /// Deletes an item from the database.
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="idFieldName">Id field name</param>
        /// <param name="id">DTO id</param>
        public void Delete(string tableName, string idFieldName, Guid id)
        {
            Execute("DELETE FROM " + tableName + " WHERE " + idFieldName + " = '" + id.ToString() + "'");
        }

        /// <summary>
        /// Deletes all rows from a database table.
        /// </summary>
        /// <param name="tableName">Database table name</param>
        public void Delete(string tableName)
        {
            Execute("DELETE FROM " + tableName);
        }

        /// <summary>
        /// Deletes all rows from a database table using the specified Where clause.
        /// </summary>
        /// <param name="tableName">Database table name</param>
        /// <param name="where">Where clause</param>
        public void Delete(string tableName, string where)
        {
            Execute("DELETE FROM " + tableName + " WHERE " + where);
        }
    }
}
#endif
