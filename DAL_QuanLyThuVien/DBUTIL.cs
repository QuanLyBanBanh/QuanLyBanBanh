using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace DAL_QuanLyBanBanh
{
    public class DBUtil
    {
        public static string connString = @"Data Source=HAHAHA\SQLEXPRESS;Initial Catalog=QuanLyBanBanh;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";

        public static SqlCommand GetCommand(string sql, List<Object> args, CommandType cmdType)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.CommandType = cmdType;
            for (int i = 0; i < args.Count; i++)
            {
                cmd.Parameters.AddWithValue($"@{i}", args[i]);
            }
            return cmd;
        }

        public static int Update(string sql, List<Object> args, CommandType cmdType = CommandType.Text)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                using (SqlCommand cmd = new SqlCommand(sql, conn, transaction))
                {
                    cmd.CommandType = cmdType;
                    for (int i = 0; i < args.Count; i++)
                    {
                        cmd.Parameters.AddWithValue($"@{i}", args[i] ?? DBNull.Value);
                    }

                    try
                    {
                        int rowsAffected = cmd.ExecuteNonQuery();
                        transaction.Commit();
                        return rowsAffected;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }


        public static SqlDataReader Query(string sql, List<object> args, CommandType cmdType = CommandType.Text)
        {
            try
            {
                SqlCommand cmd = GetCommand(sql, args, cmdType);
                cmd.Connection.Open();
                return cmd.ExecuteReader();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static T Value<T>(string sql, List<object> args, CommandType cmdType = CommandType.Text) where T : new()
        {
            try
            {
                SqlCommand cmd = GetCommand(sql, args, cmdType);
                cmd.Connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    T result = new T();
                    Type type = typeof(T);
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string columnName = reader.GetName(i);
                        PropertyInfo propertyInfo = type.GetProperty(columnName);

                        if (propertyInfo != null && propertyInfo.CanWrite)
                        {
                            object value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            if (value != null)
                                propertyInfo.SetValue(result, Convert.ChangeType(value, propertyInfo.PropertyType));
                        }
                    }
                    return result;
                }

                return default;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static T ExecuteScalar<T>(string sql, List<object> args, CommandType cmdType = CommandType.Text)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = cmdType;
                for (int i = 0; i < args.Count; i++)
                {
                    cmd.Parameters.AddWithValue($"@{i}", args[i] ?? DBNull.Value);
                }

                conn.Open();
                object result = cmd.ExecuteScalar();
                if (result == null || result == DBNull.Value)
                    return default(T);

                return (T)Convert.ChangeType(result, typeof(T));
            }
        }
        public static object ExecuteScalar(string sql, List<object> parameters)
        {
            using SqlConnection conn = new SqlConnection(connString);
            using SqlCommand cmd = new SqlCommand(sql, conn);

            // Thêm tham số vào command
            for (int i = 0; i < parameters.Count; i++)
            {
                cmd.Parameters.AddWithValue($"@{i}", parameters[i] ?? DBNull.Value);
            }

            conn.Open();
            return cmd.ExecuteScalar();
        }
        public static DataTable GetDataTable(string sql, List<object> args, CommandType cmdType = CommandType.Text)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand cmd = GetCommand(sql, args, cmdType))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }
        public static DataTable ConvertToDataTable<T>(List<T> items)
        {
            DataTable table = new DataTable(typeof(T).Name);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in props)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (T item in items)
            {
                var values = new object[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }
                table.Rows.Add(values);
            }

            return table;
        }

    }
}
