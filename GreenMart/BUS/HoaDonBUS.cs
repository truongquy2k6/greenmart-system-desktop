using DAL;
using System;
using System.Data;

namespace BUS
{
    public class HoaDonBUS
    {
        private readonly HoaDonDAL dal = new();

        public string TaoMa() => dal.TaoMa();
        public DataTable LayChiTiet(string ma) => dal.LayChiTiet(ma);
        public DataTable TimKH(string sdt) => dal.TimKhachHang(sdt);
        public DataTable TimSP(string kw, string maCH) => dal.TimSanPhamPOS(kw, maCH);
        public void Huy(string ma) => dal.Huy(ma);
        public DataTable LayThongTinCuaHangTuHoaDon(string ma) => dal.LayThongTinCuaHangTuHoaDon(ma);

        public DataTable TimKiem(DateTime tu, DateTime den, string kw = "")
            => dal.TimKiem(tu, den, kw);

        public void ThemHoaDon(string ma, decimal tong, string maKH, string maNV, string? maKM, decimal giam, string pttt)
            => dal.ThemHoaDon(ma, tong, maKH, maNV, maKM, giam, pttt);

        public void ThemChiTiet(string maHD, string maSP, int sl, decimal gia)
            => dal.ThemChiTiet(maHD, maSP, sl, gia);
    }
}
