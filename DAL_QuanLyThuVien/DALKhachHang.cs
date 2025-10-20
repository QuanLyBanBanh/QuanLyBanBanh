using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO_QuanLyBanBanh;
using Microsoft.Data.SqlClient;

namespace DAL_QuanLyBanBanh
{
    public class DALKhachHang
    {
        // ======= HÀM MAP DATAREADER -> OBJECT =======
        private KhachHang Map(SqlDataReader r)
        {
            return new KhachHang
            {
                MaKH = r["MaKH"]?.ToString(),
                TenKH = r["TenKH"]?.ToString(),
                SDT = r["SDT"]?.ToString(),
                Email = r["Email"]?.ToString(),
                DiaChi = r["DiaChi"]?.ToString(),
                NgayTao = r["NgayTao"] != DBNull.Value ? Convert.ToDateTime(r["NgayTao"]) : (DateTime?)null
            };
        }

        // ======= HÀM SELECT CHUNG =======
        private List<KhachHang> SelectBySql(string sql, List<object> args)
        {
            List<KhachHang> list = new List<KhachHang>();
            using (SqlDataReader r = DBUtil.Query(sql, args))
            {
                while (r.Read())
                    list.Add(Map(r));
            }
            return list;
        }

        // ======= LẤY TẤT CẢ KHÁCH HÀNG =======
        public List<KhachHang> SelectAll() =>
            SelectBySql("SELECT * FROM KhachHang", new List<object>());

        // ======= THÊM KHÁCH HÀNG =======
        public string Insert(KhachHang kh)
        {
            string sql = @"INSERT INTO KhachHang(MaKH, TenKH, SDT, Email, DiaChi, NgayTao)
                           VALUES(@0,@1,@2,@3,@4,@5)";
            try
            {
                DBUtil.Update(sql, new List<object>
                {
                    kh.MaKH,
                    kh.TenKH,
                    kh.SDT,
                    kh.Email ?? (object)DBNull.Value,
                    kh.DiaChi ?? (object)DBNull.Value,
                    kh.NgayTao ?? (object)DBNull.Value
                });
                return "";
            }
            catch (Exception ex)
            {
                return "Lỗi thêm KH: " + ex.Message;
            }
        }

        // ======= CẬP NHẬT KHÁCH HÀNG =======
        public string Update(KhachHang kh)
        {
            string sql = @"UPDATE KhachHang 
                           SET TenKH=@0, SDT=@1, Email=@2, DiaChi=@3, NgayTao=@4 
                           WHERE MaKH=@5";
            try
            {
                int rows = DBUtil.Update(sql, new List<object>
                {
                    kh.TenKH,
                    kh.SDT,
                    kh.Email ?? (object)DBNull.Value,
                    kh.DiaChi ?? (object)DBNull.Value,
                    kh.NgayTao ?? (object)DBNull.Value,
                    kh.MaKH
                });
                return rows == 0 ? "Không tìm thấy KH để cập nhật." : "";
            }
            catch (Exception ex)
            {
                return "Lỗi cập nhật KH: " + ex.Message;
            }
        }

        // ======= XÓA KHÁCH HÀNG =======
        public string Delete(string maKH)
        {
            try
            {
                DBUtil.Update("DELETE FROM KhachHang WHERE MaKH=@0", new List<object> { maKH });
                return "";
            }
            catch (Exception ex)
            {
                return "Lỗi xóa KH: " + ex.Message;
            }
        }

        // ======= TÌM KIẾM KHÁCH HÀNG =======
        public List<KhachHang> Search(string keyword)
        {
            string sql = @"SELECT * FROM KhachHang 
                           WHERE MaKH LIKE @0 OR TenKH LIKE @0 OR SDT LIKE @0 OR Email LIKE @0 OR DiaChi LIKE @0";
            return SelectBySql(sql, new List<object> { "%" + keyword + "%" });
        }
    }
}
