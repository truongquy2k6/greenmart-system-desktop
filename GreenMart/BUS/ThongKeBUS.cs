using DAL;
using System.Data;

namespace BUS
{
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
}
