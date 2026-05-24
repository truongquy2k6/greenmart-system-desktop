using DAL;
using System.Data;
using System;

namespace BUS
{
    public class NhanVienBUS
    {
        private readonly NhanVienDAL dal = new();

        public DataTable DangNhap(string u, string p) => dal.DangNhap(u, PasswordHelper.HashPassword(p));
        public DataTable HienThi() => dal.HienThi();
        public string TaoMa() => dal.TaoMaTuDong();
        public void Xoa(string ma) => dal.Xoa(ma);

        public void Them(string ma, string ten, string cv, DateTime ns, string gt, string sdt, string dc, string tdn, string mk, string maCH, string tt)
            => dal.Them(ma, ten, cv, ns, gt, sdt, dc, tdn, mk, maCH, tt);

        public void CapNhat(string ma, string ten, string cv, DateTime ns, string gt, string sdt, string dc, string tdn, string mk, string maCH, string tt)
            => dal.CapNhat(ma, ten, cv, ns, gt, sdt, dc, tdn, mk, maCH, tt);
    }
}
