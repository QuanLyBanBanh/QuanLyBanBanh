using DTO_QuanLyBanBanh;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using DAL_QuanLyBanBanh;

namespace DAL_QuanLyBanBanh
{
    public class DALPhiBanh
    {
        private PhiBanh MapReaderToPhiBanh(SqlDataReader reader)
        {
            return new PhiBanh
            {
                MaPhi = reader["MaPhi"].ToString(),
                MaBanh = reader["MaBanh"].ToString(),
                PhiBan = reader["PhiBan"] != DBNull.Value ? Convert.ToDecimal(reader["PhiBan"]) : 0,
                NgayTao = reader["NgayTao"] != DBNull.Value ? Convert.ToDateTime(reader["NgayTao"]) : (DateTime?)null
            };
        }

        private List<PhiBanh> SelectBySql(string sql, List<object> args)
        {
            List<PhiBanh> list = new List<PhiBanh>();
            using (SqlDataReader reader = DBUtil.Query(sql, args))
            {
                while (reader.Read())
                    list.Add(MapReaderToPhiBanh(reader));
            }
            return list;
        }

        public List<PhiBanh> SelectAll()
        {
            string sql = "SELECT * FROM PhiBanh";
            return SelectBySql(sql, new List<object>());
        }

        public string InsertPhiBanh(PhiBanh pb)
        {
            string sql = @"INSERT INTO PhiBanh (MaPhi, MaBanh, PhiBan, NgayTao)
                           VALUES (@0, @1, @2, @3)";
            List<object> parameters = new List<object>
            {
                pb.MaPhi,
                pb.MaBanh,
                pb.PhiBan,
                pb.NgayTao ?? (object)DBNull.Value
            };

            try
            {
                DBUtil.Update(sql, parameters);
                return "";
            }
            catch (Exception ex)
            {
                return "Lỗi khi thêm: " + ex.Message;
            }
        }

        public string UpdatePhiBanh(PhiBanh pb)
        {
            string sql = @"UPDATE PhiBanh
                           SET MaBanh = @0,
                               PhiBan = @1,
                               NgayTao = @2
                           WHERE MaPhi = @3";
            List<object> parameters = new List<object>
            {
                pb.MaBanh,
                pb.PhiBan,
                pb.NgayTao ?? (object)DBNull.Value,
                pb.MaPhi
            };

            try
            {
                int rows = DBUtil.Update(sql, parameters);
                return rows == 0 ? "Không tìm thấy bản ghi để cập nhật." : "";
            }
            catch (Exception ex)
            {
                return "Lỗi khi cập nhật: " + ex.Message;
            }
        }

        public string DeletePhiBanh(string maPhi)
        {
            string sql = "DELETE FROM PhiBanh WHERE MaPhi = @0";
            try
            {
                DBUtil.Update(sql, new List<object> { maPhi });
                return "";
            }
            catch (Exception ex)
            {
                return "Lỗi khi xóa: " + ex.Message;
            }
        }

        public List<PhiBanh> SearchPhiBanh(string keyword)
        {
            string sql = @"SELECT * FROM PhiBanh
                           WHERE MaPhi LIKE @0 OR MaBanh LIKE @0";
            List<object> parameters = new List<object> { "%" + keyword + "%" };
            return SelectBySql(sql, parameters);
        }
    }
}
