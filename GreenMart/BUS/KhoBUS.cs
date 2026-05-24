using DAL;
using System.Data;

namespace BUS
{
    public class KhoBUS
    {
        private readonly KhoDAL dal = new();
        public DataTable HienThi() => dal.HienThi();
        public DataTable ChiTiet(string ma) => dal.ChiTiet(ma);
        public DataTable HienThiTatCaChiTiet() => dal.HienThiTatCaChiTiet();
        public string TaoMa() => dal.TaoMa();
        public void Them(string ma, string ten, string dc, string sdt, string maCH) => dal.Them(ma, ten, dc, sdt, maCH);
        public void CapNhat(string ma, string ten, string dc, string sdt, string maCH) => dal.CapNhat(ma, ten, dc, sdt, maCH);
        public void Xoa(string ma) => dal.Xoa(ma);
        public void ThemChiTiet(string maKho, string maSP, int sl) => dal.ThemChiTiet(maKho, maSP, sl);
        public void CapNhatChiTiet(string maKho, string maSP, int sl) => dal.CapNhatChiTiet(maKho, maSP, sl);
        public void XoaChiTiet(string maKho, string maSP) => dal.XoaChiTiet(maKho, maSP);
    }
}
