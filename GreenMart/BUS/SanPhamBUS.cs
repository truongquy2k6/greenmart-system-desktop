using DAL;
using System.Data;

namespace BUS
{
    public class SanPhamBUS
    {
        private readonly SanPhamDAL dal = new();

        public DataTable HienThi(int pageNumber = 1, int pageSize = 10000) => dal.HienThi(pageNumber, pageSize);
        public DataTable TimKiem(string kw) => dal.TimKiem(kw);
        public DataTable LocNangCao(string tuKhoa, string maLoai, string maNCC, string donViTinh, string trangThai, int pageNumber, int pageSize) => dal.LocNangCao(tuKhoa, maLoai, maNCC, donViTinh, trangThai, pageNumber, pageSize);
        public string TaoMa() => dal.TaoMa();
        public void Xoa(string ma) => dal.Xoa(ma);

        public void Them(string ma, string ten, decimal gia, string dvt, string anh, string mt, string ncc, string loai, string trangThai)
            => dal.Them(ma, ten, gia, dvt, anh, mt, ncc, loai, trangThai);

        public void CapNhat(string ma, string ten, decimal gia, string dvt, string anh, string mt, string ncc, string loai, string trangThai)
            => dal.CapNhat(ma, ten, gia, dvt, anh, mt, ncc, loai, trangThai);
    }
}
