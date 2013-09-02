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
using System.Data.Common;

namespace MPfm.Library.Database.Interfaces
{
    /// <summary>
    /// Interface for the SQLiteGateway class.
    /// </summary>
    public interface ISQLiteGateway
    {
        DbConnection GenerateConnection();
        Dictionary<string, string> GetMap<T>();
        string FormatSQLValue(object value);
        int Execute(string sql);
        object ExecuteScalar(string sql);
        void CompactDatabase();
        IEnumerable<object> SelectList(string sql);
        List<Tuple<object, object>> SelectTuple(string sql);
        T SelectOne<T>(string sql) where T : new();
        List<T> Select<T>(string sql) where T : new();
        int Update<T>(T obj, string tableName, string whereFieldName, object whereValue);
        int Update<T>(T obj, string tableName, Dictionary<string, object> where);
        int Insert<T>(T obj, string tableName);
        void Delete(string tableName, string idFieldName, Guid id);
        void Delete(string tableName);
        void Delete(string tableName, string where);
    }
}
