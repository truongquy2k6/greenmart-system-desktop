using Microsoft.Data.SqlClient;
using System.Data;

namespace DAL
{
    /// <summary>
    /// Lớp xử lý dữ liệu (DAL) cho Kho và Chi Tiết Kho
    /// </summary>
    public class KhoDAL
    {
        /// <summary>
        /// Lấy danh sách tất cả các kho
        /// </summary>
        public DataTable HienThi()
        {
            return DatabaseHelper.ExecuteQuery("sp_HienThiKho");
        }

        /// <summary>
        /// Lấy chi tiết các sản phẩm đang có trong một kho cụ thể
        /// </summary>
        public DataTable ChiTiet(string maKho)
        {
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@MaKho", maKho)
            };

            return DatabaseHelper.ExecuteQuery("sp_HienThiChiTietKhoTheoMa", parameters);
        }

        /// <summary>
        /// Lấy toàn bộ chi tiết kho (tất cả kho) — dùng cho tab Chi tiết kho
        /// </summary>
        public DataTable HienThiTatCaChiTiet()
        {
            return DatabaseHelper.ExecuteQuery("sp_HienThiTatCaChiTietKho");
        }

        /// <summary>
        /// Thêm thông tin kho mới
        /// </summary>
        public void Them(string ma, string ten, string dc, string sdt, string maCH)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new("@MaKho", ma),
                new("@TenKho", ten),
                new("@DiaChi", dc),
                new("@SoDienThoai", sdt),
                new("@MaCH", maCH)
            };

            DatabaseHelper.ExecuteNonQuery("sp_ThemKho", parameters);
        }

        /// <summary>
        /// Cập nhật thông tin kho
        /// </summary>
        public void CapNhat(string ma, string ten, string dc, string sdt, string maCH)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new("@MaKho", ma),
                new("@TenKho", ten),
                new("@DiaChi", dc),
                new("@SoDienThoai", sdt),
                new("@MaCH", maCH)
            };

            DatabaseHelper.ExecuteNonQuery("sp_CapNhatKho", parameters);
        }

        /// <summary>
        /// Xóa kho (Lưu ý: Cần xử lý cẩn thận nếu kho đang chứa hàng)
        /// </summary>
        public void Xoa(string ma)
        {
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@MaKho", ma)
            };

            DatabaseHelper.ExecuteNonQuery("sp_XoaKho", parameters);
        }

        /// <summary>
        /// Thêm sản phẩm vào chi tiết kho
        /// </summary>
        public void ThemChiTiet(string maKho, string maSP, int sl)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new("@MaKho", maKho),
                new("@MaSP", maSP),
                new("@SoLuong", sl)
            };

            DatabaseHelper.ExecuteNonQuery("sp_ThemChiTietKho", parameters);
        }

        /// <summary>
        /// Cập nhật số lượng sản phẩm trong kho
        /// </summary>
        public void CapNhatChiTiet(string maKho, string maSP, int sl)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new("@MaKho", maKho),
                new("@MaSP", maSP),
                new("@SoLuong", sl)
            };

            DatabaseHelper.ExecuteNonQuery("sp_CapNhatChiTietKho", parameters);
        }

        /// <summary>
        /// Xóa một sản phẩm khỏi chi tiết kho
        /// </summary>
        public void XoaChiTiet(string maKho, string maSP)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new("@MaKho", maKho),
                new("@MaSP", maSP)
            };

            DatabaseHelper.ExecuteNonQuery("sp_XoaChiTietKho", parameters);
        }

        /// <summary>
        /// Tự động sinh mã kho mới (VD: K01, K02)
        /// </summary>
        public string TaoMa()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery("sp_TuDongTaoMaKho");

            if (dt.Rows.Count == 0)
            {
                return "K01";
            }

            // Lấy mã cuối cùng trong DB (VD: "K05")
            string lastId = dt.Rows[0][0].ToString()!.Trim();

            // Cắt 1 ký tự đầu ("K"), chuyển phần còn lại thành số và cộng thêm 1
            int nextNumber = int.Parse(lastId.Substring(1)) + 1;

            // Format số mới thành chuỗi 2 chữ số (VD: 6 -> "06") và ghép với tiền tố
            return "K" + nextNumber.ToString("D2");
        }
    }
}