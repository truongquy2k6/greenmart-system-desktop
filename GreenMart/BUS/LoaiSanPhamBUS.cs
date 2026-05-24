using DAL;
using System.Data;

namespace BUS
{
    public class LoaiSanPhamBUS
    {
        private readonly LoaiSanPhamDAL dal = new();
        public DataTable HienThi(int pageNumber = 1, int pageSize = 10000) => dal.HienThi(pageNumber, pageSize);
        public DataTable TimKiem(string kw, int pageNumber = 1, int pageSize = 10000) => dal.TimKiem(kw, pageNumber, pageSize);
        public string TaoMa() => dal.TaoMa();
        public void Them(string ma, string ten) => dal.Them(ma, ten);
        public void CapNhat(string ma, string ten) => dal.CapNhat(ma, ten);
        public void Xoa(string ma) => dal.Xoa(ma);
    }
}
