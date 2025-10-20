using System;
using System.Collections.Generic;
using DTO_QuanLyBanBanh;
using DAL_QuanLyBanBanh;

namespace BLL_QuanLyBanBanh
{
    public class BusTheLoaiBanh
    {
        private readonly DALTheLoaiBanh dal = new DALTheLoaiBanh();

        public List<TheLoaiBanh> GetAllTheLoaiBanh() => dal.SelectAll();

        public string AddTheLoaiBanh(TheLoaiBanh loai)
        {
            if (string.IsNullOrWhiteSpace(loai.TenLoai))
                return "Tên loại bánh không được để trống.";

            if (dal.GetTheLoaiBanhByTen(loai.TenLoai) != null)
                return "Tên loại bánh đã tồn tại.";

            loai.NgayTao ??= DateTime.Now;
            return dal.InsertTheLoaiBanh(loai);
        }

        public string UpdateTheLoaiBanh(TheLoaiBanh loai)
        {
            if (string.IsNullOrWhiteSpace(loai.TenLoai))
                return "Tên loại bánh không được để trống.";

            var existing = dal.GetTheLoaiBanhByTen(loai.TenLoai);
            if (existing != null && existing.MaLoai != loai.MaLoai)
                return "Tên loại bánh đã tồn tại.";

            return dal.UpdateTheLoaiBanh(loai);
        }

        public string DeleteTheLoaiBanh(string maLoai) => dal.DeleteTheLoaiBanh(maLoai);

        public List<TheLoaiBanh> SearchTheLoaiBanh(string keyword)
            => string.IsNullOrWhiteSpace(keyword) ? GetAllTheLoaiBanh() : dal.SearchTheLoaiBanh(keyword);

        public string TaoMaLoaiBanhTuDong()
        {
            var list = GetAllTheLoaiBanh();
            int max = 0;
            foreach (var item in list)
            {
                if (item.MaLoai?.StartsWith("L") == true &&
                    int.TryParse(item.MaLoai.Substring(1), out int so) && so > max)
                    max = so;
            }
            return "L" + (max + 1).ToString("D3");
        }
    }
}
