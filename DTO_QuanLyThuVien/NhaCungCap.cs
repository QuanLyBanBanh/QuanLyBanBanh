using System;

namespace DTO_QuanLyBanBanh
{
    public class NhaCungCap
    {
        public string MaNCC { get; set; }
        public string TenNCC { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }
        public string DiaChi { get; set; }
        public string TrangThai { get; set; } // "Hoạt động" / "Tạm ngưng"
        public DateTime? NgayTao { get; set; }
    }
}
