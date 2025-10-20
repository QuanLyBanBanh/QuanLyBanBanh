using GUI_QuanLyBanBanh;
using GUI_QuanLyThuVien;
using System;
using System.Windows.Forms;

namespace Nhom2_QuanLyThuVien
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Cấu hình ứng dụng theo chuẩn mới (cần thiết cho .NET 6+)
            ApplicationConfiguration.Initialize();

            // Chạy trực tiếp form quản lý Nhà Cung Cấp
            Application.Run(new frmNhanVien());
        }
    }
}