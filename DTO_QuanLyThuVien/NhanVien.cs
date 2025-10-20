using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO_QuanLyBanBanh
{
    public class NhanVien
    {
        public string MaNV { get; set; }
        public string TenNV { get; set; }
        public string GioiTinh { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string SDT { get; set; }
        public string ChucVu { get; set; }
        public string HinhAnh { get; set; } // chỉ lưu tên file
        public string TrangThai { get; set; }
        public string MatKhau { get; set; }
        public DateTime? NgayTao { get; set; }

        public string Email { get; set; }

    }
}
