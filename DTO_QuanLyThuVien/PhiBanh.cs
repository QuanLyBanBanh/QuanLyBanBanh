using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO_QuanLyBanBanh
{
    public class PhiBanh
    {
        public string MaPhi { get; set; }       
        public string MaBanh { get; set; }      
        public decimal PhiBan { get; set; }     
        public DateTime? NgayTao { get; set; }  
    }
}
