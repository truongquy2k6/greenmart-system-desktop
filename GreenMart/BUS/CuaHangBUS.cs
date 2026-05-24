using DAL;
using System.Data;

namespace BUS
{
    public class CuaHangBUS
    {
        private readonly CuaHangDAL dal = new();
        public DataTable HienThi() => dal.HienThi();
        public string TaoMa() => dal.TaoMa();
        public void Them(string ma, string ten, string dc, string sdt, string email) => dal.Them(ma, ten, dc, sdt, email);
        public void CapNhat(string ma, string ten, string dc, string sdt, string email) => dal.CapNhat(ma, ten, dc, sdt, email);
        public void Xoa(string ma) => dal.Xoa(ma);
    }
}
