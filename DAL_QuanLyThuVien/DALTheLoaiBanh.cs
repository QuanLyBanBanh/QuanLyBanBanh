using DAL_QuanLyBanBanh;
using DTO_QuanLyBanBanh;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace DAL_QuanLyBanBanh
{
    public class DALTheLoaiBanh
    {
        private TheLoaiBanh MapReaderToTheLoaiBanh(SqlDataReader reader)
        {
            return new TheLoaiBanh
            {
                MaLoai = reader["MaLoai"].ToString(),
                TenLoai = reader["TenLoai"]?.ToString(),
                NgayTao = reader["NgayTao"] != DBNull.Value
                    ? Convert.ToDateTime(reader["NgayTao"])
                    : (DateTime?)null
            };
        }

        private List<TheLoaiBanh> SelectBySql(string sql, List<object> args)
        {
            List<TheLoaiBanh> list = new List<TheLoaiBanh>();
            using (SqlDataReader reader = DBUtil.Query(sql, args))
            {
                while (reader.Read())
                    list.Add(MapReaderToTheLoaiBanh(reader));
            }
            return list;
        }

        public List<TheLoaiBanh> SelectAll()
        {
            string sql = "SELECT MaLoai, TenLoai, NgayTao FROM TheLoaiBanh ORDER BY MaLoai";
            return SelectBySql(sql, new List<object>());
        }

        public string InsertTheLoaiBanh(TheLoaiBanh loaiBanh)
        {
            string sql = "INSERT INTO TheLoaiBanh (MaLoai, TenLoai, NgayTao) VALUES (@0, @1, @2)";
            try
            {
                DBUtil.Update(sql, new List<object>
                {
                    loaiBanh.MaLoai,
                    loaiBanh.TenLoai,
                    loaiBanh.NgayTao ?? DateTime.Now
                });
                return "";
            }
            catch (Exception ex)
            {
                return "Lỗi khi thêm loại bánh: " + ex.Message;
            }
        }

        public string UpdateTheLoaiBanh(TheLoaiBanh loaiBanh)
        {
            string sql = "UPDATE TheLoaiBanh SET TenLoai = @0 WHERE MaLoai = @1";
            try
            {
                int rows = DBUtil.Update(sql, new List<object> { loaiBanh.TenLoai, loaiBanh.MaLoai });
                return rows == 0 ? "Không tìm thấy loại bánh để cập nhật." : "";
            }
            catch (Exception ex)
            {
                return "Lỗi khi cập nhật loại bánh: " + ex.Message;
            }
        }

        public string DeleteTheLoaiBanh(string maLoai)
        {
            string sql = "DELETE FROM TheLoaiBanh WHERE MaLoai = @0";
            try
            {
                DBUtil.Update(sql, new List<object> { maLoai });
                return "";
            }
            catch (Exception ex)
            {
                return "Lỗi khi xóa loại bánh: " + ex.Message;
            }
        }

        public List<TheLoaiBanh> SearchTheLoaiBanh(string keyword)
        {
            string sql = @"SELECT * FROM TheLoaiBanh 
                           WHERE MaLoai LIKE @0 OR TenLoai LIKE @0";
            return SelectBySql(sql, new List<object> { "%" + keyword + "%" });
        }

        public TheLoaiBanh GetTheLoaiBanhByTen(string tenLoai)
        {
            string sql = "SELECT * FROM TheLoaiBanh WHERE TenLoai = @0";
            var list = SelectBySql(sql, new List<object> { tenLoai });
            return list.Count > 0 ? list[0] : null;
        }
    }
}
