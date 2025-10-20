using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL_QuanLyBanBanh;
using DTO_QuanLyBanBanh;

namespace BLL_QuanLyBanBanh
{
    public class BusNhanVien
    {
        private readonly DALNhanVien dal = new DALNhanVien();

        public List<NhanVien> GetAll() => dal.SelectAll();
        public string Add(NhanVien nv) => dal.Insert(nv);
        public string Update(NhanVien nv) => dal.Update(nv);
        public string Delete(string maNV) => dal.Delete(maNV);
        public List<NhanVien> Search(string keyword) =>
            string.IsNullOrWhiteSpace(keyword) ? GetAll() : dal.Search(keyword);
    }
}

