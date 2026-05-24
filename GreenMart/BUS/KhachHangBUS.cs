using DAL;
using System.Data;

namespace BUS
{
    public class KhachHangBUS
    {
        private readonly KhachHangDAL dal = new();
        public DataTable HienThi() => dal.HienThi();
        public string TaoMa() => dal.TaoMa();
        public void Them(string ma, string ten, string sdt, string dc, string email, int diem, string tt) => dal.Them(ma, ten, sdt, dc, email, diem, tt);
        public void CapNhat(string ma, string ten, string sdt, string dc, string email, int diem, string tt) => dal.CapNhat(ma, ten, sdt, dc, email, diem, tt);
        public void Xoa(string ma) => dal.Xoa(ma);
    }
}
