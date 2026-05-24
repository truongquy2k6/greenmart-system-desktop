using DAL;
using System.Data;

namespace BUS
{
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
