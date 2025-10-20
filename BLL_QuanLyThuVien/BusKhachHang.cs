using System.Collections.Generic;
using DAL_QuanLyBanBanh;
using DTO_QuanLyBanBanh;

namespace BLL_QuanLyBanBanh
{
    public class BusKhachHang
    {
        private readonly DALKhachHang dal = new DALKhachHang();

        public List<KhachHang> GetAll() => dal.SelectAll();

        public string Add(KhachHang kh) => dal.Insert(kh);

        public string Update(KhachHang kh) => dal.Update(kh);

        public string Delete(string maKH) => dal.Delete(maKH);

        public List<KhachHang> Search(string keyword) => dal.Search(keyword);
    }
}
