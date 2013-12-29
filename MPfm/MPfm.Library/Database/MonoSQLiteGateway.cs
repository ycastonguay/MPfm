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

#if IOS || ANDROID || MACOSX || LINUX
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using MPfm.Core;
using MPfm.Core.Attributes;
using MPfm.Core.Extensions;
using MPfm.Library.Database.Interfaces;
using Mono.Data.Sqlite;

namespace MPfm.Library.Database
{
    /// <summary>
    /// The MonoSQLiteGateway class is a data adapter class which makes it easier to select, insert, update and delete
    /// data from the database. It is based on Mono.Data.Sqlite rather than System.Data.SQLite. This gateway must be used
    /// on Android devices.
    ///
    /// Notes: Mono.Data.SQLite doesn't like:    
    /// - SingleOrDefault -- replaced with FirstOrDefault.
    /// - compare database varchar to Guid.ToString() -- need to cast guid into string before using value in LIN
    /// </summary>
    public class MonoSQLiteGateway : ISQLiteGateway
    {
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
        /// Default constructor for the MonoSQLiteGateway class.
        /// </summary>
        /// <param name="databaseFilePath">Database file path</param>
        public MonoSQLiteGateway(string databaseFilePath)
        {
            this.databaseFilePath = databaseFilePath;
        }

        /// <summary>
        /// Creates a new database file.
        /// </summary>
        public static void CreateDatabaseFile(string databaseFilePath)
        {
            try
            {
                // Create new database file
                SqliteConnection.CreateFile(databaseFilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Generates a DbConnection based on the current database file path.
        /// </summary>
        /// <returns>DbConnection</returns>
        public DbConnection GenerateConnection()
        {
            // Open connection
            SqliteConnection connection = new SqliteConnection();
            connection.ConnectionString = "Data Source=" + databaseFilePath;                

            return connection;
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
                     value.GetType().IsEnum)
            {
                // Replace single quotes by two quotes
                return "'" + value.ToString().Replace("'", "''") + "'";
            }
            else if (value.GetType().FullName.ToUpper() == "SYSTEM.FLOAT" ||
                     value.GetType().FullName.ToUpper() == "SYSTEM.SINGLE" ||
                     value.GetType().FullName.ToUpper() == "SYSTEM.DOUBLE" ||
                     value.GetType().FullName.ToUpper() == "SYSTEM.DECIMAL")
            {
                // Make sure a dot is used for decimals
                return value.ToString().Replace(",", ".");
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
            // Declare variables
            DbConnection connection = null;            
            DbCommand command = null;            

            try
            {
                // Create and open connection
                connection = GenerateConnection();
                connection.Open();

                // Create and execute command
                command = connection.CreateCommand();
                command.CommandText = sql;
                command.Connection = connection;
                int rows = command.ExecuteNonQuery();

                return rows;
            }
            catch
            {
                throw;
            }
            finally
            {
				// Dispose command
                command.Dispose();
				
                // Close and clean up connection
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes a scalar SQL query and returns the query value.
        /// </summary>
        /// <param name="sql">SQL query</param>
        /// <returns>Scalar query value</returns>
        public object ExecuteScalar(string sql)
        {
            // Declare variables
            DbConnection connection = null;
            DbCommand command = null;

            try
            {
                // Create and open connection
                connection = GenerateConnection();
                connection.Open();

                // Create and execute command
                command = connection.CreateCommand();
                command.CommandText = sql;
                command.Connection = connection;
                object obj = command.ExecuteScalar();

                return obj;
            }
            catch
            {
                throw;
            }
            finally
            {
				// Dispose command
                command.Dispose();
				
                // Close and clean up connection
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    connection.Dispose();
                }
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
            // Declare variables
            DbConnection connection = null;
            DbDataReader reader = null;
            DbCommand command = null;
            List<object> list = new List<object>();

            try
            {
                // Create and open connection
                connection = GenerateConnection();
                connection.Open();

                // Create command
                command = connection.CreateCommand();
                command.CommandText = sql;
                command.Connection = connection;

                // Create and execute reader
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    // Add key/value to dictionary
                    object field = reader.GetValue(0);
                    list.Add(field);
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
                {
                    reader.Close();
                    reader.Dispose();
                }
				
				// Dispose command
                command.Dispose();

                // Close and clean up connection
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Selects a list of tuples of two objects.
        /// </summary>
        /// <param name="sql">Query to execute (must have only two fields in the select statement)</param>
        /// <returns>List of tuple</returns>
        public List<Tuple<object, object>> SelectTuple(string sql)
        {
            // Declare variables
            DbConnection connection = null;
            DbDataReader reader = null;
            DbCommand command = null;            
            List<Tuple<object, object>> listTuple = new List<Tuple<object, object>>();

            try
            {
                // Create and open connection
                connection = GenerateConnection();
                connection.Open();

                // Create command
                command = connection.CreateCommand();
                command.CommandText = sql;
                command.Connection = connection;

                // Create and execute reader
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    // Add key/value to dictionary
                    object field1 = reader.GetValue(0);
                    object field2 = reader.GetValue(1);                    
                    listTuple.Add(new Tuple<object, object>(field1, field2));
                }

                return listTuple;
            }
            catch
            {
                throw;
            }
            finally
            {
                // Clean up reader and connection
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
				
				// Dispose command
                command.Dispose();

                // Close and clean up connection
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
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
			SqliteConnection connection = null;
			SqliteDataReader reader = null;
			SqliteCommand command = null;
            List<T> list = new List<T>();
            List<DatabaseFieldMap> maps = GetMap<T>();
			List<Tuple<PropertyInfo, Delegate, Delegate>> propertyInfos = new List<Tuple<PropertyInfo, Delegate, Delegate>>();

            try
            {                
                // Create and open connection
				//connection = GenerateConnection();
				connection = new SqliteConnection();
				connection.ConnectionString = "Data Source=" + databaseFilePath;       
                connection.Open();

                // Create command
                command = connection.CreateCommand();
                command.CommandText = sql;
                command.Connection = connection;

				// Create and execute reader
				reader = command.ExecuteReader();

				// Prepare map; order propertyInfo list to speed up conversion
				var fields = new List<string>();
				for (int a = 0; a < reader.FieldCount; a++)
					fields.Add(reader.GetName(a));

				for(int a = 0; a < fields.Count; a++)
				{
					var map = maps.FirstOrDefault(x => x.FieldName == fields[a]);
					string propertyName = map != null ? map.PropertyName : fields[a];
					var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
					if(property != null)
					{
						Delegate convertDelegate = null;
						if (property.PropertyType != reader.GetFieldType(a))
						{
							var fieldType = reader.GetFieldType(a);
							MethodInfo castMethod = typeof(Convert).GetMethod("To" + property.PropertyType.Name, new Type[] { fieldType });
							if (castMethod != null)
							{
								try
								{
									// Create delegate to convert type
									Type funcType = typeof(Func<,>).MakeGenericType(new Type[2] { fieldType, property.PropertyType });
									var del = Delegate.CreateDelegate(funcType, castMethod);
									var item = new Tuple<Type, Type, Delegate>(fieldType, property.PropertyType, del);
									convertDelegate = del;
								}
								catch(Exception ex)
								{
									Console.WriteLine("SQLiteGateway - Select - Construct delegate error: {0}", ex);
								}
							}
						}

						var delSetter = Delegate.CreateDelegate(typeof(Action<object, object, object[]>), property, "SetValue");
						propertyInfos.Add(new Tuple<PropertyInfo, Delegate, Delegate>(property, delSetter, convertDelegate));
					}
					else
					{
						Console.WriteLine("[!!!] SqliteGateway - Failed to recognize property {0}", propertyName);
						propertyInfos.Add(new Tuple<PropertyInfo, Delegate, Delegate>(null, null, null));
					}
				}

                while (reader.Read())
                {
                    // Create object and fill data
                    T data = new T();
                    for (int a = 0; a < reader.FieldCount; a++)
                    {
                        Type fieldType = reader.GetFieldType(a);
                        object fieldValue = reader.GetValue(a);

						PropertyInfo info = propertyInfos[a].Item1;
						var delSet = propertyInfos[a].Item2;
						if (info != null)
                        {
                            if (fieldValue is System.DBNull)
							{
                                fieldValue = null;                                
							}
							else if (info.PropertyType.IsEnum)
                            {
								fieldValue = Enum.Parse(info.PropertyType, fieldValue.ToString());
                            }         
							else if (info.PropertyType == typeof(Guid))
							{
								fieldValue = new Guid(fieldValue.ToString());                                    
							}
                            else if (info.PropertyType.FullName != fieldType.FullName)
                            {
								var convertDelegate = propertyInfos[a].Item3;
								if(convertDelegate != null)
									fieldValue = convertDelegate.DynamicInvoke(new object[1] { fieldValue });
                            }

							// This is actually faster than dynamic invoke
							info.SetValue(data, fieldValue, null);
//							if(delSet != null)
//								delSet.DynamicInvoke(new object[3] { data, fieldValue, null });
						}
                    }

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
                {
                    reader.Close();
                    reader.Dispose();
                }
				
				// Dispose command
                command.Dispose();

                // Close and clean up connection
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
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
            // Declare variables
            DbConnection connection = null;            
            DbCommand command = null;
            List<DatabaseFieldMap> maps = GetMap<T>();
            StringBuilder sql = new StringBuilder();

            try
            {
                // Generate query
                sql.AppendLine("UPDATE [" + tableName + "] SET ");

                // Scan through properties
                PropertyInfo[] propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                bool addedOneItem = false;
                for (int a = 0; a < propertyInfos.Length; a++)
                {
                    PropertyInfo propertyInfo = propertyInfos[a];
                    if (propertyInfo.GetSetMethod() != null)
                    {
                        // Check mapping
                        bool saveToDatabase = true;
                        string fieldName = propertyInfo.Name;
                        var map = maps.FirstOrDefault(x => x.PropertyName == propertyInfo.Name);
                        if (map != null)
                        {
                            fieldName = map.FieldName;
                            saveToDatabase = map.SaveToDatabase;
                        }

                        if (saveToDatabase)
                        {
                            // Add comma if an item was added previously
                            if (!addedOneItem)
                                addedOneItem = true;
                            else
                                sql.Append(", ");

                            // Add database field name
                            sql.Append("[" + fieldName + "]=");
                            object value = propertyInfo.GetValue(obj, null);
                            sql.Append(FormatSQLValue(value));
                            sql.Append("\n");
                        }
                    }
                }

                // Generate where clause
                sql.AppendLine(" WHERE ");
                addedOneItem = false;
                for(int a = 0; a < where.Count; a++)
                {
                    // Add comma if an item was added previously
                    if(!addedOneItem)
                        addedOneItem = true;
                    else
                        sql.Append(", ");

                    KeyValuePair<string, object> keyValue = where.ElementAt(a);
                    sql.AppendLine("[" + keyValue.Key + "]=");
                    sql.Append(FormatSQLValue(keyValue.Value));
                    sql.Append("\n");
                }

                // Create and open connection
                connection = GenerateConnection();
                connection.Open();

                // Create command
                command = connection.CreateCommand();
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
				// Dispose command
                command.Dispose();
				
                // Close and clean up connection
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
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
            DbConnection connection = null;
            DbCommand command = null;
            List<DatabaseFieldMap> maps = GetMap<T>();
            StringBuilder sql = new StringBuilder();

            try
            {
                // Generate query
                sql.AppendLine("INSERT INTO [" + tableName + "] (");

                // Scan through properties to set column names
                PropertyInfo[] propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                bool addedOneItem = false;
                for (int a = 0; a < propertyInfos.Length; a++)
                {
                    PropertyInfo propertyInfo = propertyInfos[a];
                    //Console.WriteLine("Insert - Fields - Item {0} name: {1}", a, propertyInfo.Name);
                    if (propertyInfo.GetSetMethod() != null)
                    {
                        // Check mapping
                        bool saveToDatabase = true;
                        string fieldName = propertyInfo.Name;
                        var map = maps.FirstOrDefault(x => x.PropertyName == propertyInfo.Name);
                        if (map != null)
                        {
                            fieldName = map.FieldName;
                            saveToDatabase = map.SaveToDatabase;
                        }

                        if (saveToDatabase)
                        {
                            // Add comma if an item was added previously
                            if (!addedOneItem)
                                addedOneItem = true;
                            else
                                sql.Append(", ");

                            sql.Append("[" + fieldName + "]");
                            sql.Append("\n");
                        }
                    }
                }
                sql.AppendLine(") VALUES (");

                // Scan through properties and set values
                addedOneItem = false;
                for (int a = 0; a < propertyInfos.Length; a++)
                {
                    PropertyInfo propertyInfo = propertyInfos[a];
                    //Console.WriteLine("Insert - Values - Item {0} name: {1}", a, propertyInfo.Name);
                    if (propertyInfo.GetSetMethod() != null)
                    {
                        // Check mapping
                        bool saveToDatabase = true;
                        string fieldName = propertyInfo.Name;
                        var map = maps.FirstOrDefault(x => x.PropertyName == propertyInfo.Name);
                        if (map != null)
                            saveToDatabase = map.SaveToDatabase;

                        if (saveToDatabase)
                        {
                            // Add comma if an item was added previously
                            if (!addedOneItem)
                                addedOneItem = true;
                            else
                                sql.Append(", ");

                            // Get value and determine how to add field value
                            object value = propertyInfo.GetValue(obj, null);
                            sql.Append(FormatSQLValue(value));
                            sql.Append("\n");
                        }
                    }
                }
                sql.AppendLine(") ");
                //Console.WriteLine(sql);

                // Create and open connection
                connection = GenerateConnection();
                connection.Open();

                // Create command
                command = connection.CreateCommand();
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
				// Dispose command
                command.Dispose();

                // Close and clean up connection
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
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
