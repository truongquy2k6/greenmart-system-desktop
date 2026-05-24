using DAL;
using System.Data;

namespace BUS
{
    // --- QUẢN LÝ NHÂN VIÊN ---
    public class NhanVienBUS
    {
        private readonly NhanVienDAL dal = new();

        public DataTable DangNhap(string u, string p) => dal.DangNhap(u, p);
        public DataTable HienThi() => dal.HienThi();
        public string TaoMa() => dal.TaoMaTuDong();
        public void Xoa(string ma) => dal.Xoa(ma);

        public void Them(string ma, string ten, string cv, DateTime ns, string gt, string sdt, string dc, string tdn, string mk, string maCH, string tt)
            => dal.Them(ma, ten, cv, ns, gt, sdt, dc, tdn, mk, maCH, tt);

        public void CapNhat(string ma, string ten, string cv, DateTime ns, string gt, string sdt, string dc, string tdn, string mk, string maCH, string tt)
            => dal.CapNhat(ma, ten, cv, ns, gt, sdt, dc, tdn, mk, maCH, tt);
    }

    // --- QUẢN LÝ SẢN PHẨM ---
    public class SanPhamBUS
    {
        private readonly SanPhamDAL dal = new();

        public DataTable HienThi() => dal.HienThi();
        public DataTable TimKiem(string kw) => dal.TimKiem(kw);
        public string TaoMa() => dal.TaoMa();
        public void Xoa(string ma) => dal.Xoa(ma);

        public void Them(string ma, string ten, decimal gia, string dvt, string anh, string mt, string ncc, string loai, string trangThai)
            => dal.Them(ma, ten, gia, dvt, anh, mt, ncc, loai, trangThai);

        public void CapNhat(string ma, string ten, decimal gia, string dvt, string anh, string mt, string ncc, string loai, string trangThai)
            => dal.CapNhat(ma, ten, gia, dvt, anh, mt, ncc, loai, trangThai);
    }

    // --- QUẢN LÝ HÓA ĐƠN ---
    public class HoaDonBUS
    {
        private readonly HoaDonDAL dal = new();

        public string TaoMa() => dal.TaoMa();
        public DataTable LayChiTiet(string ma) => dal.LayChiTiet(ma);
        public DataTable TimKH(string sdt) => dal.TimKhachHang(sdt);
        public DataTable TimSP(string kw, string maCH) => dal.TimSanPhamPOS(kw, maCH);
        public void Huy(string ma) => dal.Huy(ma);

        public DataTable TimKiem(DateTime tu, DateTime den, string kw = "")
            => dal.TimKiem(tu, den, kw);

        public void ThemHoaDon(string ma, decimal tong, string maKH, string maNV, string? maKM, decimal giam, string pttt)
            => dal.ThemHoaDon(ma, tong, maKH, maNV, maKM, giam, pttt);

        public void ThemChiTiet(string maHD, string maSP, int sl, decimal gia)
            => dal.ThemChiTiet(maHD, maSP, sl, gia);
    }

    // --- CÁC DANH MỤC KHÁC (Gộp gọn cho dễ nhìn) ---
    public class LoaiSanPhamBUS
    {
        private readonly LoaiSanPhamDAL dal = new();
        public DataTable HienThi() => dal.HienThi();
        public DataTable TimKiem(string kw) => dal.TimKiem(kw);
        public string TaoMa() => dal.TaoMa();
        public void Them(string ma, string ten) => dal.Them(ma, ten);
        public void CapNhat(string ma, string ten) => dal.CapNhat(ma, ten);
        public void Xoa(string ma) => dal.Xoa(ma);
    }

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

    public class KhachHangBUS
    {
        private readonly KhachHangDAL dal = new();
        public DataTable HienThi() => dal.HienThi();
        public string TaoMa() => dal.TaoMa();
        public void Them(string ma, string ten, string sdt, string dc, string email, int diem, string tt) => dal.Them(ma, ten, sdt, dc, email, diem, tt);
        public void CapNhat(string ma, string ten, string sdt, string dc, string email, int diem, string tt) => dal.CapNhat(ma, ten, sdt, dc, email, diem, tt);
        public void Xoa(string ma) => dal.Xoa(ma);
    }

    public class CuaHangBUS
    {
        private readonly CuaHangDAL dal = new();
        public DataTable HienThi() => dal.HienThi();
        public string TaoMa() => dal.TaoMa();
        public void Them(string ma, string ten, string dc, string sdt, string email) => dal.Them(ma, ten, dc, sdt, email);
        public void CapNhat(string ma, string ten, string dc, string sdt, string email) => dal.CapNhat(ma, ten, dc, sdt, email);
        public void Xoa(string ma) => dal.Xoa(ma);
    }

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

    public class KhuyenMaiBUS
    {
        private readonly KhuyenMaiDAL dal = new();
        public DataTable HienThi() => dal.HienThi();
        public string TaoMa() => dal.TaoMa();
        public void Them(string ma, string ten, string mt, string loai, decimal gt, DateTime bd, DateTime kt, decimal dk, string tt)
            => dal.Them(ma, ten, mt, loai, gt, bd, kt, dk, tt);
        public void CapNhat(string ma, string ten, string mt, string loai, decimal gt, DateTime bd, DateTime kt, decimal dk, string tt)
            => dal.CapNhat(ma, ten, mt, loai, gt, bd, kt, dk, tt);
        public void Xoa(string ma) => dal.Xoa(ma);
    }

    public class ThongKeBUS
    {
        private readonly ThongKeDAL dal = new();
        public DataTable LayTongQuan() => dal.LayTongQuan();
        public DataTable LayBieuDo() => dal.LayBieuDo();
        public DataTable LayTopSP() => dal.LayTopSP();
        public DataTable LayHDGanDay() => dal.LayHDGanDay();
        
        // New methods
        public DataTable LayTopKhachHang() => DatabaseHelper.ExecuteQuery("sp_LayTopKhachHang");
        public DataTable LayPhanBoLoaiSP() => DatabaseHelper.ExecuteQuery("sp_LayPhanBoLoaiSP");
        public DataTable LayDanhSachSapHet() => DatabaseHelper.ExecuteQuery("sp_LayDanhSachSapHet");
    }

    public class LichSuBUS
    {
        private readonly LichSuDAL dal = new();
        public DataTable LayTruyCap() => dal.LayTruyCap();
        public DataTable LayChinhSua(string? bang = null) => dal.LayChinhSua(bang);
        public void GhiNhanTruyCap(string maNV, string ip, string thietBi) => dal.GhiNhanTruyCap(maNV, ip, thietBi);
        public void GhiNhanDangXuat(string maNV) => dal.GhiNhanDangXuat(maNV);
        public void GhiNhanChinhSua(string maNV, string bang, string hanhDong, string maBanGhi, string cu, string moi) => dal.GhiNhanChinhSua(maNV, bang, hanhDong, maBanGhi, cu, moi);
    }
}