using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace DAL
{
    // ==========================================
    // LỚP XỬ LÝ DỮ LIỆU THỐNG KÊ
    // ==========================================
    public class ThongKeDAL
    {
        /// <summary>
        /// Lấy dữ liệu tổng quan cho trang Dashboard (Doanh thu, số đơn, số khách, v.v.)
        /// </summary>
        public DataTable LayTongQuan()
        {
            return DatabaseHelper.ExecuteQuery("sp_LayThongKeTongQuan");
        }

        /// <summary>
        /// Lấy dữ liệu biểu đồ doanh thu theo thời gian
        /// </summary>
        public DataTable LayBieuDo()
        {
            return DatabaseHelper.ExecuteQuery("sp_LayBieuDoDoanhThu");
        }

        /// <summary>
        /// Lấy danh sách Top sản phẩm bán chạy nhất
        /// </summary>
        public DataTable LayTopSP()
        {
            return DatabaseHelper.ExecuteQuery("sp_LayTopSanPham");
        }

        /// <summary>
        /// Lấy danh sách các hóa đơn vừa được lập gần đây
        /// </summary>
        public DataTable LayHDGanDay()
        {
            return DatabaseHelper.ExecuteQuery("sp_LayHoaDonGanDay");
        }
    }

    // ==========================================
    // LỚP XỬ LÝ DỮ LIỆU LỊCH SỬ HỆ THỐNG
    // ==========================================
    public class LichSuDAL
    {
        /// <summary>
        /// Lấy danh sách lịch sử truy cập (Đăng nhập/Đăng xuất)
        /// </summary>
        public DataTable LayTruyCap()
        {
            return DatabaseHelper.ExecuteQuery("sp_LayLichSuTruyCap");
        }

        /// <summary>
        /// Lấy danh sách lịch sử chỉnh sửa dữ liệu (Thêm, Sửa, Xóa)
        /// </summary>
        /// <param name="bang">Tên bảng cần lọc (truyền null nếu muốn xem tất cả)</param>
        public DataTable LayChinhSua(string? bang = null)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new("@TenBang", (object?)bang ?? DBNull.Value)
            };

            return DatabaseHelper.ExecuteQuery("sp_LayLichSuChinhSua", parameters);
        }

        /// <summary>
        /// Ghi nhận thông tin đăng nhập của nhân viên vào bảng LichSuTruyCap
        /// </summary>
        public void GhiNhanTruyCap(string maNV, string ip, string thietBi)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new("@MaNV", maNV),
                new("@DiaChiIP", ip),
                new("@ThietBi", thietBi)
            };

            DatabaseHelper.ExecuteNonQuery("sp_GhiNhanTruyCap", parameters);
        }

        /// <summary>
        /// Ghi nhận thời gian đăng xuất của nhân viên (cập nhật bản ghi chưa có ThoiGianDangXuat)
        /// </summary>
        public void GhiNhanDangXuat(string maNV)
        {
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@MaNV", maNV)
            };

            DatabaseHelper.ExecuteNonQuery("sp_GhiNhanDangXuat", parameters);
        }

        /// <summary>
        /// Ghi nhận lại các thay đổi dữ liệu khi người dùng Thêm/Sửa/Xóa
        /// </summary>
        public void GhiNhanChinhSua(string maNV, string bang, string hanhDong, string maBanGhi, string cu, string moi)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new("@MaNV", maNV),
                new("@TenBang", bang),
                new("@HanhDong", hanhDong),
                new("@MaBanGhi", maBanGhi),
                new("@NoiDungCu", cu),
                new("@NoiDungMoi", moi)
            };

            DatabaseHelper.ExecuteNonQuery("sp_GhiNhanChinhSua", parameters);
        }
    }
}