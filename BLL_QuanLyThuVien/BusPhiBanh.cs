using System;
using System.Collections.Generic;
using DAL_QuanLyBanBanh;
using DTO_QuanLyBanBanh;

namespace BLL_QuanLyBanBanh
{
    public class BusPhiBanh
    {
        private readonly DALPhiBanh dal = new DALPhiBanh();

        public List<PhiBanh> GetAllPhiBanh()
        {
            return dal.SelectAll();
        }

        public string AddPhiBanh(PhiBanh pb)
        {
            return dal.InsertPhiBanh(pb);
        }

        public string UpdatePhiBanh(PhiBanh pb)
        {
            return dal.UpdatePhiBanh(pb);
        }

        public string DeletePhiBanh(string maPhi)
        {
            return dal.DeletePhiBanh(maPhi);
        }

        public List<PhiBanh> SearchPhiBanh(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return GetAllPhiBanh();
            return dal.SearchPhiBanh(keyword);
        }
    }
}
