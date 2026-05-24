using DAL;
using System.Data;

namespace BUS
{
    public class NhaCungCapBUS
    {
        private readonly NhaCungCapDAL dal = new();
        public DataTable HienThi() => dal.HienThi();
        public DataTable TimKiem(string kw) => dal.TimKiem(kw);
        public string TaoMa() => dal.TaoMa();
        public void Them(string ma, string ten, string dc, string sdt, string email) => dal.Them(ma, ten, dc, sdt, email);
        public void CapNhat(string ma, string ten, string dc, string sdt, string email) => dal.CapNhat(ma, ten, dc, sdt, email);
        public void Xoa(string ma) => dal.Xoa(ma);
    }
}
