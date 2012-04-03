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
            factory = DbProviderFactories.GetFactory("System.Data.SQLite");
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
            // Open connection
            OpenConnection();

            // Create command
            DbCommand command = factory.CreateCommand();
            command.CommandText = sql;
            command.Connection = connection;

            // Create adapter
            DbDataAdapter adapter = factory.CreateDataAdapter();
            adapter.SelectCommand = command;

            // Fill table
            DataTable table = new DataTable();
            adapter.Fill(table);            

            // Close connection
            CloseConnection();

            return table;
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
