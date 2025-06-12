using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DataAccessBlock
{
    public class DataAccess
    {
        private readonly string _connectionString;

        public DataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public class Parameter
        {
            public string Name;
            public DbType Type;
            public int Size;
            public ParameterDirection Direction;
            public string SourceColumn;
            public object Value;

            public Parameter(string name, DbType type, int size, ParameterDirection direction, string sourceColumn, object value)
            {
                Name = name;
                Type = type;
                Size = size;
                Direction = direction;
                SourceColumn = sourceColumn;
                Value = value;
            }
        }

        private void AddParameters(SqlCommand cmd, ArrayList parameters)
        {
            if (parameters == null) return;

            foreach (Parameter p in parameters)
            {
                SqlParameter param = new SqlParameter();
                param.ParameterName = p.Name;
                param.DbType = p.Type;
                param.Size = p.Size;
                param.Value = p.Value ?? DBNull.Value;
                param.Direction = p.Direction;
                if (!string.IsNullOrEmpty(p.SourceColumn))
                    param.SourceColumn = p.SourceColumn;

                cmd.Parameters.Add(param);
            }
        }


        public int ExecuteNonQuery(string sql, CommandType commandType, ArrayList parameters)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand(sql, conn)
            {
                CommandType = commandType
            };
            AddParameters(cmd, parameters);
            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        public object ExecuteScalar(string sql, CommandType commandType, ArrayList parameters)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand(sql, conn)
            {
                CommandType = commandType
            };
            AddParameters(cmd, parameters);
            conn.Open();
            return cmd.ExecuteScalar();
        }

        public List<T> GetList<T>(string sql, CommandType commandType, ArrayList parameters) where T : new()
        {
            List<T> list = new();
            using SqlConnection conn = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand(sql, conn)
            {
                CommandType = commandType
            };
            AddParameters(cmd, parameters);
            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            var props = typeof(T).GetProperties();
            while (reader.Read())
            {
                T obj = new();
                foreach (var prop in props)
                {
                    if (!reader.HasColumn(prop.Name) || reader[prop.Name] == DBNull.Value)
                        continue;

                    prop.SetValue(obj, Convert.ChangeType(reader[prop.Name], prop.PropertyType));
                }
                list.Add(obj);
            }
            return list;
        }
    }

    public static class SqlDataReaderExtensions
    {
        public static bool HasColumn(this SqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
                if (reader.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            return false;
        }
    }
}
