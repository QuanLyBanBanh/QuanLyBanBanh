using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO_QuanLyBanBanh;
using Microsoft.Data.SqlClient;

namespace DAL_QuanLyBanBanh
{
    public class DALNhanVien
    {
        private NhanVien Map(SqlDataReader r)
        {
            return new NhanVien
            {
                MaNV = r["MaNV"].ToString(),
                TenNV = r["TenNV"]?.ToString(),
                GioiTinh = r["GioiTinh"]?.ToString(),
                NgaySinh = r["NgaySinh"] != DBNull.Value ? Convert.ToDateTime(r["NgaySinh"]) : (DateTime?)null,
                SDT = r["SDT"]?.ToString(),
                ChucVu = r["ChucVu"]?.ToString(),
                HinhAnh = r["HinhAnh"]?.ToString(),
                TrangThai = r["TrangThai"]?.ToString(),
                MatKhau = r["MatKhau"]?.ToString(),
                NgayTao = r["NgayTao"] != DBNull.Value ? Convert.ToDateTime(r["NgayTao"]) : (DateTime?)null,
                Email = r["Email"]?.ToString() // ✅ thêm dòng này
            };
        }

        private List<NhanVien> SelectBySql(string sql, List<object> args)
        {
            List<NhanVien> list = new List<NhanVien>();
            using (SqlDataReader r = DBUtil.Query(sql, args))
            {
                while (r.Read())
                    list.Add(Map(r));
            }
            return list;
        }

        public List<NhanVien> SelectAll() =>
            SelectBySql("SELECT * FROM NhanVien", new List<object>());

        public string Insert(NhanVien nv)
        {
            string sql = @"INSERT INTO NhanVien(MaNV, TenNV, GioiTinh, NgaySinh, SDT, ChucVu, HinhAnh, TrangThai, MatKhau, NgayTao, Email)
                           VALUES(@0,@1,@2,@3,@4,@5,@6,@7,@8,@9,@10)";
            try
            {
                DBUtil.Update(sql, new List<object> {
                    nv.MaNV, nv.TenNV, nv.GioiTinh, nv.NgaySinh ?? (object)DBNull.Value,
                    nv.SDT, nv.ChucVu, nv.HinhAnh ?? (object)DBNull.Value,
                    nv.TrangThai, nv.MatKhau, nv.NgayTao ?? (object)DBNull.Value,
                    nv.Email ?? (object)DBNull.Value // ✅ thêm Email
                });
                return "";
            }
            catch (Exception ex) { return "Lỗi thêm NV: " + ex.Message; }
        }

        public string Update(NhanVien nv)
        {
            string sql = @"UPDATE NhanVien SET TenNV=@0, GioiTinh=@1, NgaySinh=@2, SDT=@3,
                           ChucVu=@4, HinhAnh=@5, TrangThai=@6, MatKhau=@7, NgayTao=@8, Email=@9 WHERE MaNV=@10";
            try
            {
                int rows = DBUtil.Update(sql, new List<object> {
                    nv.TenNV, nv.GioiTinh, nv.NgaySinh ?? (object)DBNull.Value, nv.SDT,
                    nv.ChucVu, nv.HinhAnh ?? (object)DBNull.Value, nv.TrangThai,
                    nv.MatKhau, nv.NgayTao ?? (object)DBNull.Value,
                    nv.Email ?? (object)DBNull.Value, // ✅ thêm Email
                    nv.MaNV
                });
                return rows == 0 ? "Không tìm thấy NV để cập nhật." : "";
            }
            catch (Exception ex) { return "Lỗi cập nhật: " + ex.Message; }
        }

        public string Delete(string maNV)
        {
            try
            {
                DBUtil.Update("DELETE FROM NhanVien WHERE MaNV=@0", new List<object> { maNV });
                return "";
            }
            catch (Exception ex) { return "Lỗi xóa: " + ex.Message; }
        }

        public List<NhanVien> Search(string keyword)
        {
            string sql = @"SELECT * FROM NhanVien WHERE MaNV LIKE @0 OR TenNV LIKE @0 OR SDT LIKE @0 OR ChucVu LIKE @0 OR Email LIKE @0"; // ✅ thêm Email vào tìm kiếm
            return SelectBySql(sql, new List<object> { "%" + keyword + "%" });
        }
    }
}
