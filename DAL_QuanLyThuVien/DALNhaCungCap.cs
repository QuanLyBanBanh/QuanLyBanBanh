using DTO_QuanLyBanBanh;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace DAL_QuanLyBanBanh
{
    public class DALNhaCungCap
    {
        // Ánh xạ dữ liệu từ SqlDataReader sang đối tượng NhaCungCap
        private NhaCungCap MapReaderToNhaCungCap(SqlDataReader reader)
        {
            return new NhaCungCap
            {
                MaNCC = reader["MaNCC"].ToString(),
                TenNCC = reader["TenNCC"]?.ToString(),
                SDT = reader["SDT"]?.ToString(),
                Email = reader["Email"]?.ToString(),
                DiaChi = reader["DiaChi"]?.ToString(),
                TrangThai = reader["TrangThai"]?.ToString(), // Chuỗi: "Hoạt động" / "Tạm ngưng"
                NgayTao = reader["NgayTao"] != DBNull.Value ? Convert.ToDateTime(reader["NgayTao"]) : (DateTime?)null
            };
        }

        // Thực hiện câu truy vấn SQL trả về danh sách nhà cung cấp
        private List<NhaCungCap> SelectBySql(string sql, List<object> args)
        {
            List<NhaCungCap> list = new List<NhaCungCap>();
            using (SqlDataReader reader = DBUtil.Query(sql, args))
            {
                while (reader.Read())
                {
                    list.Add(MapReaderToNhaCungCap(reader));
                }
            }
            return list;
        }

        // Lấy toàn bộ danh sách nhà cung cấp
        public List<NhaCungCap> SelectAll()
        {
            string sql = "SELECT * FROM NhaCungCap";
            return SelectBySql(sql, new List<object>());
        }

        // Thêm mới nhà cung cấp
        public string InsertNhaCungCap(NhaCungCap ncc)
        {
            string sql = @"INSERT INTO NhaCungCap (MaNCC, TenNCC, SDT, Email, DiaChi, TrangThai, NgayTao)
                           VALUES (@0, @1, @2, @3, @4, @5, @6)";
            List<object> parameters = new List<object>
            {
                ncc.MaNCC,
                ncc.TenNCC,
                ncc.SDT,
                ncc.Email,
                ncc.DiaChi,
                ncc.TrangThai ?? (object)DBNull.Value,
                ncc.NgayTao ?? (object)DBNull.Value
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

        // Cập nhật thông tin nhà cung cấp
        public string UpdateNhaCungCap(NhaCungCap ncc)
        {
            string sql = @"UPDATE NhaCungCap
                           SET TenNCC = @0,
                               SDT = @1,
                               Email = @2,
                               DiaChi = @3,
                               TrangThai = @4,
                               NgayTao = @5
                           WHERE MaNCC = @6";
            List<object> parameters = new List<object>
            {
                ncc.TenNCC,
                ncc.SDT,
                ncc.Email,
                ncc.DiaChi,
                ncc.TrangThai ?? (object)DBNull.Value,
                ncc.NgayTao ?? (object)DBNull.Value,
                ncc.MaNCC
            };

            try
            {
                int rows = DBUtil.Update(sql, parameters);
                return rows == 0 ? "Không tìm thấy Nhà Cung Cấp để cập nhật." : "";
            }
            catch (Exception ex)
            {
                return "Lỗi khi cập nhật: " + ex.Message;
            }
        }

        // Xóa nhà cung cấp theo mã
        public string DeleteNhaCungCap(string maNCC)
        {
            string sql = "DELETE FROM NhaCungCap WHERE MaNCC = @0";
            try
            {
                DBUtil.Update(sql, new List<object> { maNCC });
                return "";
            }
            catch (Exception ex)
            {
                return "Lỗi khi xóa: " + ex.Message;
            }
        }

        // Tìm kiếm nhà cung cấp
        public List<NhaCungCap> SearchNhaCungCap(string keyword)
        {
            string sql = @"SELECT * FROM NhaCungCap
                           WHERE MaNCC LIKE @0 
                              OR TenNCC LIKE @0 
                              OR SDT LIKE @0
                              OR Email LIKE @0 
                              OR DiaChi LIKE @0
                              OR TrangThai LIKE @0";
            List<object> parameters = new List<object> { "%" + keyword + "%" };
            return SelectBySql(sql, parameters);
        }
    }
}
