using System;
using System.Collections.Generic;
using DTO_QuanLyBanBanh;
using DAL_QuanLyBanBanh;

namespace BLL_QuanLyBanBanh
{
    public class BusNhaCungCap
    {
        private readonly DALNhaCungCap dal = new DALNhaCungCap();

        public List<NhaCungCap> GetAllNhaCungCap()
        {
            return dal.SelectAll();
        }

        public string AddNhaCungCap(NhaCungCap ncc)
        {
            return dal.InsertNhaCungCap(ncc);
        }

        public string UpdateNhaCungCap(NhaCungCap ncc)
        {
            return dal.UpdateNhaCungCap(ncc);
        }

        public string DeleteNhaCungCap(string maNCC)
        {
            return dal.DeleteNhaCungCap(maNCC);
        }

        public List<NhaCungCap> SearchNhaCungCap(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return GetAllNhaCungCap();
            return dal.SearchNhaCungCap(keyword);
        }
    }
}
