//
// SQLiteGateway.cs: Data adapter class for SQLite.
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
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Linq;
using System.Data.Objects;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using MPfm.Core;
using MPfm.Library.Data;

namespace MPfm.Library
{
    /// <summary>
    /// The SQLiteGateway class is a data adapter class which makes it easier to select, insert, update and delete
    /// data from the database.
    /// </summary>
    public class SQLiteGateway
    {
        // Private variables
        private string m_databaseFilePath = string.Empty;
        private DbProviderFactory m_factory = null;
        private DbConnection m_connection = null;

        /// <summary>
        /// Default constructor for the SQLiteGateway class.
        /// </summary>
        /// <param name="databaseFilePath">Database file path</param>
        public SQLiteGateway(string databaseFilePath)
        {
            // Initialize factory
            m_factory = DbProviderFactories.GetFactory("System.Data.SQLite");
            m_databaseFilePath = databaseFilePath;
        }

        /// <summary>
        /// Opens the database connection. 
        /// Raises an exception if the connection cannot be opened.
        /// </summary>
        protected void OpenConnection()
        {
            // Check if the connection is still open
            if (m_connection != null && m_connection.State != ConnectionState.Closed)
            {
                // Throw exception
                throw new Exception("Cannot open database connection; the connection is already opened!");
            }

            try
            {
                // Open connection
                m_connection = m_factory.CreateConnection();
                m_connection.ConnectionString = "Data Source=" + m_databaseFilePath;
                m_connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Closes the database connection. 
        /// Raises an exception if the connection cannot be closed.
        /// </summary>
        protected void CloseConnection()
        {
            // Check if the connection is still open
            if (m_connection == null || m_connection.State == ConnectionState.Closed)
            {
                // Throw exception
                throw new Exception("Cannot close database connection; the connection isn't opened!");
            }

            try
            {
                // Close connection
                m_connection.Close();
                m_connection.Dispose();
                m_connection = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
            DbCommand command = m_factory.CreateCommand();
            command.CommandText = sql;
            command.Connection = m_connection;

            // Create adapter
            DbDataAdapter adapter = m_factory.CreateDataAdapter();
            adapter.SelectCommand = command;

            // Fill table
            DataTable table = new DataTable();
            adapter.Fill(table);            

            // Close connection
            CloseConnection();

            return table;
        }
    }
}
