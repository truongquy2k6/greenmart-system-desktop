using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace DAL
{
    /// <summary>
    /// Lớp xử lý dữ liệu (DAL) cho Hóa Đơn và Chi Tiết Hóa Đơn
    /// </summary>
    public class HoaDonDAL
    {
        /// <summary>
        /// Tìm kiếm hóa đơn theo khoảng thời gian và từ khóa
        /// </summary>
        public DataTable TimKiem(DateTime tu, DateTime den, string kw = "")
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new("@TuNgay", tu),
                new("@DenNgay", den),
                new("@TuKhoa", kw)
            };

            return DatabaseHelper.ExecuteQuery("sp_TimKiemHoaDon", parameters);
        }

        /// <summary>
        /// Lấy danh sách chi tiết của một hóa đơn cụ thể
        /// </summary>
        public DataTable LayChiTiet(string maHD)
        {
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@MaHD", maHD)
            };

            return DatabaseHelper.ExecuteQuery("sp_LayChiTietHoaDon", parameters);
        }

        /// <summary>
        /// Thêm mới một hóa đơn vào hệ thống
        /// </summary>
        public void ThemHoaDon(string ma, decimal tong, string? maKH, string maNV, string? maKM, decimal giam, string pttt)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new("@MaHD", ma),
                new("@TongTien", tong),
                // Xử lý null: Nếu maKH hoặc maKM là null, truyền DBNull.Value xuống DB
                new("@MaKH", (object?)maKH ?? DBNull.Value),
                new("@MaNV", maNV),
                new("@MaKM", (object?)maKM ?? DBNull.Value),
                new("@GiamGia", giam),
                new("@PhuongThucThanhToan", pttt)
            };

            DatabaseHelper.ExecuteNonQuery("sp_ThemHoaDon", parameters);
        }

        /// <summary>
        /// Thêm một sản phẩm vào chi tiết hóa đơn
        /// </summary>
        public void ThemChiTiet(string maHD, string maSP, int sl, decimal gia)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new("@MaHD", maHD),
                new("@MaSP", maSP),
                new("@SoLuong", sl),
                new("@DonGia", gia)
            };

            DatabaseHelper.ExecuteNonQuery("sp_ThemChiTietHD", parameters);
        }

        /// <summary>
        /// Hủy hóa đơn (gọi Stored Procedure để xử lý hoàn kho và đổi trạng thái)
        /// </summary>
        public void Huy(string ma)
        {
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@MaHD", ma)
            };

            DatabaseHelper.ExecuteNonQuery("sp_HuyHoaDon", parameters);
        }

        /// <summary>
        /// Tìm nhanh khách hàng theo số điện thoại (Dành cho màn hình POS)
        /// </summary>
        public DataTable TimKhachHang(string sdt)
        {
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@SDT", sdt)
            };

            return DatabaseHelper.ExecuteQuery("sp_TimKhachHangTheoSDT", parameters);
        }

        /// <summary>
        /// Tìm kiếm sản phẩm tại màn hình bán hàng (chỉ lấy hàng có tồn kho tại cửa hàng)
        /// </summary>
        public DataTable TimSanPhamPOS(string kw, string maCH)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new("@TuKhoa", kw),
                new("@MaCH", maCH)
            };

            return DatabaseHelper.ExecuteQuery("sp_TimKiemSanPham", parameters);
        }

        /// <summary>
        /// Tự động sinh mã hóa đơn mới (VD: HD001, HD002)
        /// </summary>
        public string TaoMa()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery("sp_TuDongTaoMaHD");

            if (dt.Rows.Count == 0)
            {
                return "HD001";
            }

            // Lấy mã cuối cùng trong DB (VD: "HD005")
            string lastId = dt.Rows[0][0].ToString()!.Trim();

            // Cắt 2 ký tự đầu ("HD"), chuyển phần còn lại thành số và cộng thêm 1
            int nextNumber = int.Parse(lastId.Substring(2)) + 1;

            // Format số mới thành chuỗi 3 chữ số (VD: 6 -> "006") và ghép với tiền tố
            return "HD" + nextNumber.ToString("D3");
        }
    }
}