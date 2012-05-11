//
// SQLiteGateway.cs: Data adapter class for SQLite.
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
using System.Data;
using System.Data.Common;
using System.Data.Linq;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
using MPfm.Core;

namespace MPfm.Library
{
    /// <summary>
    /// The SQLiteGateway class is a data adapter class which makes it easier to select, insert, update and delete
    /// data from the database.
    ///
    /// Notes: System.Data.SQLite doesn't like:    
    /// - SingleOrDefault -- replaced with FirstOrDefault.
    /// - compare database varchar to Guid.ToString() -- need to cast guid into string before using value in LIN
    /// </summary>
    public class SQLiteGateway
    {
        // Private variables        
        private DbProviderFactory factory = null;
        private DbConnection connection = null;

        /// <summary>
        /// Private value for the DatabaseFilePath property.
        /// </summary>
        private string databaseFilePath = string.Empty;
        /// <summary>
        /// Database file path.
        /// </summary>
        public string DatabaseFilePath
        {
            get
            {
                return databaseFilePath;
            }
        }

        /// <summary>
        /// Default constructor for the SQLiteGateway class.
        /// </summary>
        /// <param name="databaseFilePath">Database file path</param>
        public SQLiteGateway(string databaseFilePath)
        {
            // Initialize factory
            Tracing.Log("SQLiteGateway init -- Initializing database factory (" + databaseFilePath + ")...");
						
#if (!MACOSX && !LINUX)			
        	factory = DbProviderFactories.GetFactory("System.Data.SQLite");
#else			
			factory = DbProviderFactories.GetFactory("Mono.Data.SQLite");			
#endif
			
            this.databaseFilePath = databaseFilePath;
        }

        /// <summary>
        /// Creates a new database file.
        /// </summary>
        public static void CreateDatabaseFile(string databaseFilePath)
        {
            // Create new database file
            SQLiteConnection.CreateFile(databaseFilePath);            
        }

        /// <summary>
        /// Generates a DbConnection based on the current database file path.
        /// </summary>
        /// <returns>DbConnection</returns>
        protected DbConnection GenerateConnection()
        {
            // Open connection
            DbConnection connection = factory.CreateConnection();
            connection.ConnectionString = "Data Source=" + databaseFilePath;                

            return connection;
        }

        /// <summary>
        /// Returns the list of properties which have different database field names.
        /// </summary>
        /// <typeparam name="T">Object to scan (generic)</typeparam>
        /// <returns>Dictionary of DatabaseFieldName/PropertyName</returns>
        protected Dictionary<string, string> GetMap<T>()
        {
            // Create map by scanning properties
            Dictionary<string, string> dictMap = new Dictionary<string, string>();
            PropertyInfo[] propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                // Scan attributes
                object[] attributes = propertyInfo.GetCustomAttributes(true);
                foreach (object attribute in attributes)
                {
                    // Try to cast into attribute map
                    DatabaseFieldAttribute attrMap = attribute as DatabaseFieldAttribute;
                    if (attrMap != null)
                    {
                        // Add item to dictionary
                        dictMap.Add(attrMap.DatabaseFieldName, propertyInfo.Name);
                    }
                }
            }

            return dictMap;
        }

        /// <summary>
        /// Formats a value for a SQL command.
        /// If the value type is String or Guid, quotes will be added to the value.
        /// If the value type is DBNull, a null value will be added (without quotes).
        /// </summary>
        /// <param name="value">Value to format</param>
        /// <returns>Formatted value</returns>
        protected string FormatSQLValue(object value)
        {           
            // Check value type
            if (value == null)
            {
                return "null";
            }
            else if (value.GetType().FullName.ToUpper() == "SYSTEM.STRING" ||
                     value.GetType().FullName.ToUpper() == "SYSTEM.GUID" ||
                     value.GetType().IsEnum)
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
        /// Opens the database connection. 
        /// Raises an exception if the connection cannot be opened.
        /// </summary>
        protected void OpenConnection()
        {
            // Check if the connection is still open
            if (connection != null && connection.State != ConnectionState.Closed)
            {
                // Throw exception
                //throw new Exception("Cannot open database connection; the connection is already opened!");
                return;
            }

            try
            {
                // Open connection
                connection = factory.CreateConnection();
                connection.ConnectionString = "Data Source=" + databaseFilePath;
                connection.Open();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Closes the database connection. 
        /// Raises an exception if the connection cannot be closed.
        /// </summary>
        protected void CloseConnection()
        {
            // Check if the connection is still open
            if (connection == null || connection.State == ConnectionState.Closed)
            {
                // Throw exception
                //throw new Exception("Cannot close database connection; the connection isn't opened!");
                return;
            }

            try
            {
                // Close connection
                connection.Close();
                connection.Dispose();
                connection = null;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Executes a SQL query and returns the number of rows affected.
        /// </summary>
        /// <param name="sql">SQL query</param>
        /// <returns>Number of rows affected</returns>
        protected int Execute(string sql)
        {
            // Open connection
            OpenConnection();

            // Create command
            DbCommand command = factory.CreateCommand();
            command.CommandText = sql;
            command.Connection = connection;
            int rows = command.ExecuteNonQuery();

            // Close connection
            CloseConnection();

            return rows;
        }

        /// <summary>
        /// Executes a scalar SQL query and returns the query value.
        /// </summary>
        /// <param name="sql">SQL query</param>
        /// <returns>Scalar query value</returns>
        protected object ExecuteScalar(string sql)
        {
            // Open connection
            OpenConnection();

            // Create command
            DbCommand command = factory.CreateCommand();
            command.CommandText = sql;
            command.Connection = connection;
            object obj = command.ExecuteScalar();

            // Close connection
            CloseConnection();

            return obj;
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
        /// Executes a select query and returns a DataTable object.
        /// </summary>
        /// <param name="sql">Query to execute</param>
        /// <returns>DataTable with data</returns>
        protected DataTable Select(string sql)
        {            
            DbDataAdapter adapter = null;
            DbCommand command = null;
            try
            {
                // Open connection
                OpenConnection();

                // Create command
                command = factory.CreateCommand();
                command.CommandText = sql;
                command.Connection = connection;

                // Fill table
                DataTable table = new DataTable();                
                FillDataTable(command, table);

                // Close connection
                CloseConnection();

                return table;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (adapter != null)
                    adapter.Dispose();
            }
        }

        /// <summary>
        /// Executes a select query and returns a list of objects as specified in the generics type.
        /// </summary>
        /// <typeparam name="T">Object tye to fill</typeparam>
        /// <param name="sql">Query to execute</param>
        /// <returns>List of objects</returns>
        protected List<T> Select<T>(string sql) where T : new()
        {
            // Declare variables
            DbConnection connection = null;
            DbDataReader reader = null;            
            DbCommand command = null;
            List<T> list = new List<T>();
            Dictionary<string, string> dictMap = GetMap<T>();

            try
            {                
                // Create and open connection
                connection = GenerateConnection();
                connection.Open();

                // Create command
                command = factory.CreateCommand();
                command.CommandText = sql;
                command.Connection = connection;

                // Create and execute reader
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    // Create object and fill data
                    T data = new T();

                    // Cycle through columns
                    for (int a = 0; a < reader.FieldCount; a++)
                    {
                        // Get column info
                        string fieldName = reader.GetName(a);
                        Type fieldType = reader.GetFieldType(a);
                        object fieldValue = reader.GetValue(a);

                        // Check for map
                        string propertyName = fieldName;                        
                        if(dictMap.ContainsKey(fieldName))
                        {
                            propertyName = dictMap[fieldName];
                        }

                        // Get property info and fill column if valid
                        PropertyInfo info = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
                        if (info != null)
                        {
                            try
                            {                         
                                // Set value to null
                                if (fieldValue is System.DBNull)
                                    fieldValue = null;                                

                                // Check if the type is an enum                                    
                                if (info.PropertyType.IsEnum)
                                {
                                    // Try to cast dynamically
                                    MethodInfo castMethod = typeof(Conversion).GetMethod("GetEnumValue").MakeGenericMethod(info.PropertyType);
                                    fieldValue = castMethod.Invoke(null, new object[] { fieldValue.ToString() });
                                }                                
                                else if (info.PropertyType.FullName.ToUpper() == "SYSTEM.GUID")
                                {
                                    // Guid aren't supported in SQLite, so they are stored as strings.
                                    fieldValue = new Guid(fieldValue.ToString());                                    
                                }
                                else if (info.PropertyType.FullName != fieldType.FullName)
                                {
                                    // Call a convert method in the Convert static class, if available
                                    MethodInfo castMethod = typeof(Convert).GetMethod("To" + info.PropertyType.Name, new Type[] { fieldType });
                                    if (castMethod != null)
                                    {
                                        fieldValue = castMethod.Invoke(null, new object[] { fieldValue });                                        
                                    }
                                }

                                // Set property value
                                info.SetValue(data, fieldValue, null);
                            }
                            catch
                            {
                                throw;
                            }
                        }
                    }

                    // Add item to list
                    list.Add(data);
                }
                
                return list;
            }
            catch
            {
                throw;
            }
            finally
            {
                // Clean up reader and connection
                if (reader != null)
                    reader.Close();

                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        /// <summary>
        /// Updates an object into the database.
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="obj">Object to update</param>
        /// <param name="tableName">Database table name</param>
        /// <param name="where">Dictionary containing the where clause (equals to)</param>
        /// <returns>Number of rows affected</returns>
        protected int Update<T>(T obj, string tableName, Dictionary<string, object> where)
        {            
            // Declare variables
            DbConnection connection = null;            
            DbCommand command = null;
            Dictionary<string, string> dictMap = GetMap<T>();
            StringBuilder sql = new StringBuilder();

            try
            {
                // Generate query
                sql.AppendLine("UPDATE [" + tableName + "] SET ");

                // Scan through properties
                PropertyInfo[] propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                for (int a = 0; a < propertyInfos.Length; a++)
                {
                    // Get property info
                    PropertyInfo propertyInfo = propertyInfos[a];

                    // Make sure the property has a setter
                    if (propertyInfo.GetSetMethod() != null)
                    {
                        // Check for map
                        string fieldName = propertyInfo.Name;                    
                        if (dictMap.ContainsValue(propertyInfo.Name))
                        {
                            fieldName = dictMap.FindKeyByValue<string, string>(propertyInfo.Name); 
                        }

                        // Add database field name
                        sql.Append("[" + fieldName + "]=");

                        // Get value and determine how to add field value
                        object value = propertyInfo.GetValue(obj, null);
                        sql.Append(FormatSQLValue(value));

                        // Add a comma if this isn't the last item
                        if (a < propertyInfos.Length - 1)
                        {
                            sql.Append(", ");
                        }
                        sql.Append("\n");
                    }
                }

                // Generate where clause
                sql.AppendLine(" WHERE ");
                for(int a = 0; a < where.Count; a++)
                {
                    KeyValuePair<string, object> keyValue = where.ElementAt(a);
                    sql.AppendLine("[" + keyValue.Key + "]=");
                    sql.Append(FormatSQLValue(keyValue.Value));

                    // Add an AND keyword if this isn't the last item
                    if (a < where.Count - 1)
                    {
                        sql.Append(" AND ");
                    }
                    sql.Append("\n");
                }

                // Create and open connection
                connection = GenerateConnection();
                connection.Open();

                // Create command
                command = factory.CreateCommand();
                command.CommandText = sql.ToString();
                command.Connection = connection;

                // Execute command
                int count = command.ExecuteNonQuery();
                return count;
            }
            catch
            {
                throw;
            }
            finally
            {
                // Close and clean up connection
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        /// <summary>
        /// Fills a DataTable object from a DbCommand. This method is a workaround to a SQLite bug in Mono:
        /// https://bugzilla.xamarin.com/show_bug.cgi?id=2128
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <param name="dt">DataTable object to fill</param>
        private static void FillDataTable(DbCommand command, DataTable dt)
        {
            var reader = command.ExecuteReader();
            var len = reader.FieldCount;
            var values = new object[len];

            // Create the DataTable columns
            for (int i = 0; i < len; i++)
                dt.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
                       
            // Add data rows
            dt.BeginLoadData();
            while (reader.Read())
            {
                // Add values
                for (int i = 0; i < len; i++)
                    values[i] = reader[i];

                // Add row
                dt.Rows.Add(values);
            }
            dt.EndLoadData();

            // Dispose
            reader.Close();
            reader.Dispose();
        }

        /// <summary>
        /// Updates a DataTable into the database (useful for insert/update/delete).
        /// </summary>
        /// <param name="table">DataTable to update</param>
        /// <param name="sql">Base query to select item to update/insert/delete</param>
        protected void UpdateDataTable(DataTable table, string sql)
        {
            // Open connection
            OpenConnection();

            // Create command
            DbCommand command = factory.CreateCommand();
            command.CommandText = sql;
            command.Connection = connection;

            // Create adapter
            DbDataAdapter adapter = factory.CreateDataAdapter();
            adapter.SelectCommand = command;

            // Create command builder
            DbCommandBuilder builder = factory.CreateCommandBuilder();
            builder.DataAdapter = adapter;

            // Get the insert, update and delete commands
            adapter.InsertCommand = builder.GetInsertCommand();
            adapter.UpdateCommand = builder.GetUpdateCommand();
            adapter.DeleteCommand = builder.GetDeleteCommand();

            adapter.Update(table);

            // Close connection
            CloseConnection();
        }

        /// <summary>
        /// Updates a DataTable into the database using a transaction (useful for insert/update/delete).
        /// </summary>
        /// <param name="table">DataTable to update</param>
        /// <param name="sql">Base query to select item to update/insert/delete</param>
        protected void UpdateDataTableTransaction(DataTable table, string sql)
        {
            // Open connection
            OpenConnection();

            // Create transaction
            DbTransaction transaction = connection.BeginTransaction();

            // Create command
            DbCommand command = factory.CreateCommand();
            command.CommandText = sql;
            command.Connection = connection;            

            // Create adapter
            DbDataAdapter adapter = factory.CreateDataAdapter();
            adapter.SelectCommand = command;

            // Create command builder
            DbCommandBuilder builder = factory.CreateCommandBuilder();
            builder.DataAdapter = adapter;

            // Get the insert, update and delete commands
            adapter.InsertCommand = builder.GetInsertCommand();
            adapter.UpdateCommand = builder.GetUpdateCommand();
            adapter.DeleteCommand = builder.GetDeleteCommand();

            adapter.Update(table);

            transaction.Commit();

            // Dispose stuff            
            adapter.Dispose();
            builder.Dispose();
            command.Dispose();
            transaction.Dispose();            

            // Close connection
            CloseConnection();
        }

        /// <summary>
        /// Inserts an item into the database.
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="idFieldName">Id field name</param>
        /// <param name="dto">Object</param>
        protected void Insert(string tableName, string idFieldName, object dto)
        {
            // Get empty result set
            string baseQuery = "SELECT * FROM " + tableName;
            DataTable table = Select(baseQuery + " WHERE " + idFieldName + " = ''");

            // Add new row to data table
            DataRow newRow = table.NewRow();
            table.Rows.Add(newRow);
            ConvertLibrary.ToRow(ref newRow, dto);

            // Insert new row into database
            UpdateDataTable(table, baseQuery);
        }

        /// <summary>
        /// Updates an item in the database.
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="idFieldName">Id field name</param>
        /// <param name="id">DTO id</param>
        /// <param name="dto">DTO</param>
        protected void Update(string tableName, string idFieldName, Guid id, object dto)
        {
            // Get item to update
            string baseQuery = "SELECT * FROM " + tableName;
            DataTable table = Select(baseQuery + " WHERE " + idFieldName + " = '" + id.ToString() + "'");

            // Check if the row was found
            if (table.Rows.Count == 0)
            {
                throw new Exception("Could not find the item to update (TableName: " + tableName + " | Id: " + id.ToString() + ")");
            }

            // Update row in DataTable
            DataRow row = table.Rows[0];
            ConvertLibrary.ToRow(ref row, dto);

            // Update row into database
            UpdateDataTable(table, baseQuery);
        }

        /// <summary>
        /// Deletes an item from the database.
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="idFieldName">Id field name</param>
        /// <param name="id">DTO id</param>
        protected void Delete(string tableName, string idFieldName, Guid id)
        {
            Execute("DELETE FROM " + tableName + " WHERE " + idFieldName + " = '" + id.ToString() + "'");

            //// Get item to delete
            //string baseQuery = "SELECT * FROM " + tableName;
            //DataTable table = Select(baseQuery + " WHERE " + idFieldName + " = '" + id.ToString() + "'");

            //// Check if the row was found
            //if (table.Rows.Count == 0)
            //{
            //    throw new Exception("Could not find the item to delete (TableName: " + tableName + " | Id: " + id.ToString() + ")");
            //}

            //// Delete row in DataTable
            //table.Rows[0].Delete();

            //// Update row into database
            //UpdateDataTable(table, baseQuery);
        }

        /// <summary>
        /// Deletes all rows from a database table.
        /// </summary>
        /// <param name="tableName">Database table name</param>
        protected void Delete(string tableName)
        {
            Execute("DELETE FROM " + tableName);
        }

        /// <summary>
        /// Deletes all rows from a database table using the specified Where clause.
        /// </summary>
        /// <param name="tableName">Database table name</param>
        /// <param name="where">Where clause</param>
        protected void Delete(string tableName, string where)
        {
            Execute("DELETE FROM " + tableName + " WHERE " + where);
        }
    }
}
