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

using Sessions.Core;
using Sessions.Core.Attributes;
#if !IOS && !ANDROID && !MACOSX && !LINUX && !WINDOWSSTORE && !WINDOWS_PHONE
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
using MPfm.Library.Database.Interfaces;

namespace MPfm.Library.Database
{
    /// <summary>
    /// The SQLiteGateway class is a data adapter class which makes it easier to select, insert, update and delete
    /// data from the database.
    ///
    /// Notes: System.Data.SQLite doesn't like:    
    /// - SingleOrDefault -- replaced with FirstOrDefault.
    /// - compare database varchar to Guid.ToString() -- need to cast guid into string before using value in LIN
    /// </summary>
    public class SQLiteGateway : ISQLiteGateway
    {
        private DbProviderFactory factory = null;
        private SQLiteConnection connection = null;        

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
            Tracing.Log("SQLiteGateway init -- Initializing database factory (" + databaseFilePath + ")...");
						
#if (!MACOSX && !LINUX && !IOS && !ANDROID)			
        	factory = DbProviderFactories.GetFactory("System.Data.SQLite");
#elif (MACOSX || LINUX)	
			factory = DbProviderFactories.GetFactory("Mono.Data.SQLite");			
#endif
			
            this.databaseFilePath = databaseFilePath;
        }

        /// <summary>
        /// Creates a new database file.
        /// </summary>
        public static void CreateDatabaseFile(string databaseFilePath)
        {
            SQLiteConnection.CreateFile(databaseFilePath);
        }

        /// <summary>
        /// Generates a DbConnection based on the current database file path.
        /// </summary>
        /// <returns>DbConnection</returns>
        public DbConnection GenerateConnection()
        {
            DbConnection connection = factory.CreateConnection();
            connection.ConnectionString = "Data Source=" + databaseFilePath;                

            return connection;
        }

        ///// <summary>
        ///// Returns the list of properties which have different database field names.
        ///// </summary>
        ///// <typeparam name="T">Object to scan (generic)</typeparam>
        ///// <returns>List of DatabaseFieldMap</returns>
        //public List<DatabaseFieldMap> GetMap<T>()
        //{
        //    // Create map by scanning properties
        //    List<DatabaseFieldMap> maps = new List<DatabaseFieldMap>();
        //    PropertyInfo[] propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        //    foreach (PropertyInfo propertyInfo in propertyInfos)
        //    {
        //        // Scan attributes
        //        object[] attributes = propertyInfo.GetCustomAttributes(true);
        //        foreach (object attribute in attributes)
        //        {
        //            // Try to cast into attribute map
        //            DatabaseFieldAttribute attrMap = attribute as DatabaseFieldAttribute;
        //            if (attrMap != null)
        //                maps.Add(new DatabaseFieldMap(propertyInfo.Name, attrMap.DatabaseFieldName, attrMap.SaveToDatabase));
        //        }
        //    }

        //    return maps;
        //}

        /// <summary>
        /// Returns the list of properties which have different database field names.
        /// </summary>
        /// <typeparam name="T">Object to scan (generic)</typeparam>
        /// <returns>List of DatabaseFieldMap</returns>
        public List<DatabaseFieldMap> GetMap<T>()
        {
            var maps = new List<DatabaseFieldMap>();
            PropertyInfo[] propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                // Scan attributes
                object[] attributes = propertyInfo.GetCustomAttributes(true);
                foreach (object attribute in attributes)
                {
                    // Try to cast into attribute map
                    var attrMap = attribute as DatabaseFieldAttribute;
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
            DbConnection connection = null;            
            DbCommand command = null;            

            try
            {
                // Create and open connection
                connection = GenerateConnection();
                connection.Open();

                // Create and execute command
                command = factory.CreateCommand();
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
                command.Dispose();
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
            DbConnection connection = null;
            DbCommand command = null;

            try
            {
                // Create and open connection
                connection = GenerateConnection();
                connection.Open();

                // Create and execute command
                command = factory.CreateCommand();
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
                command.Dispose();
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
            DbConnection connection = null;
            DbDataReader reader = null;
            DbCommand command = null;
            var list = new List<object>();

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
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
				
                command.Dispose();
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
                command = factory.CreateCommand();
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
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
				
                command.Dispose();
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
            var list = Select<T>(sql);
            if (list != null && list.Count > 0)
                return list[0];
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
            DbConnection connection = null;
            DbDataReader reader = null;
            DbCommand command = null;
            var list = new List<T>();
            var maps = GetMap<T>();
            var propertyInfos = new List<Tuple<PropertyInfo, Delegate, Delegate>>();

            try
            {
                // Create and open connection
                connection = GenerateConnection();
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

                for (int a = 0; a < fields.Count; a++)
                {
                    var map = maps.FirstOrDefault(x => x.FieldName == fields[a]);
                    string propertyName = map != null ? map.PropertyName : fields[a];
                    var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
                    if (property != null)
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
                                catch (Exception ex)
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
                                if (convertDelegate != null)
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
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }

                command.Dispose();
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
            var dict = new Dictionary<string, object>();
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
            DbConnection connection = null;            
            DbCommand command = null;
            var maps = GetMap<T>();
            var sql = new StringBuilder();

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

                    // Add an AND keyword if this isn't the last item
                    if (a < where.Count - 1)
                    {
                        sql.Append(" AND ");
                    }
                    sql.Append("\n");
                }

                connection = GenerateConnection();
                connection.Open();

                command = factory.CreateCommand();
                command.CommandText = sql.ToString();
                command.Connection = connection;
                int count = command.ExecuteNonQuery();
                return count;
            }
            catch
            {
                throw;
            }
            finally
            {
                command.Dispose();
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

            try
            {
                connection = GenerateConnection();
                connection.Open();
                int count = Insert(obj, tableName, connection);
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
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        private int Insert<T>(T obj, string tableName, DbConnection connection)
        {
            DbCommand command = null;
            var maps = GetMap<T>();
            var sql = new StringBuilder();

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

                command = factory.CreateCommand();
                command.CommandText = sql.ToString();
                command.Connection = connection;
                int count = command.ExecuteNonQuery();
                return count;
            }
            catch
            {
                throw;
            }
            finally
            {
                command.Dispose();
            }
        }

        public int Insert<T>(IEnumerable<T> objs, string tableName)
        {
            int count = 0;
            DbConnection connection = null;
            connection = GenerateConnection();
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                foreach (var obj in objs)
                    count += Insert(obj, tableName, connection);
                transaction.Commit();
            }
            return count;
        }

        /// <summary>
        /// Deletes an item from the database.
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="idFieldName">Id field name</param>
        /// <param name="id">DTO id</param>
        public void Delete(string tableName, string idFieldName, Guid id)
        {
            Execute(string.Format("DELETE FROM {0} WHERE {1} = '{2}'", tableName, idFieldName, id));
        }

        /// <summary>
        /// Deletes all rows from a database table.
        /// </summary>
        /// <param name="tableName">Database table name</param>
        public void Delete(string tableName)
        {
            Execute(string.Format("DELETE FROM {0}", tableName));
        }

        /// <summary>
        /// Deletes all rows from a database table using the specified Where clause.
        /// </summary>
        /// <param name="tableName">Database table name</param>
        /// <param name="where">Where clause</param>
        public void Delete(string tableName, string where)
        {
            Execute(string.Format("DELETE FROM {0} WHERE {1}", tableName, where));
        }
    }
}
#endif
