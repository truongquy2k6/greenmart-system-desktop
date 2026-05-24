using DAL;
using System;
using System.Data;

namespace BUS
{
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
}
