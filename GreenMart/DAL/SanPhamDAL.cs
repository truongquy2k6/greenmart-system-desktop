using Microsoft.Data.SqlClient;
using System.Data;

namespace DAL
{
    /// <summary>
    /// Lớp xử lý dữ liệu (DAL) cho Sản Phẩm
    /// </summary>
    public class SanPhamDAL
    {
        /// <summary>
        /// Lấy danh sách tất cả sản phẩm cùng với thông tin Nhà cung cấp và Loại sản phẩm
        /// </summary>
        public DataTable HienThi(int pageNumber = 1, int pageSize = 10000)
        {
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@PageNumber", pageNumber),
                new SqlParameter("@PageSize", pageSize)
            };
            return DatabaseHelper.ExecuteQuery("sp_HienThiSanPham", parameters);
        }

        /// <summary>
        /// Thêm một sản phẩm mới vào hệ thống
        /// </summary>
        public void Them(string ma, string ten, decimal gia, string dvt, string anh, string mt, string ncc, string loai, string trangThai)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new("@MaSP", ma),
                new("@TenSP", ten),
                new("@DonGia", gia),
                new("@DonViTinh", dvt),
                new("@HinhAnh", anh),
                new("@MoTa", mt),
                new("@MaNCC", ncc),
                new("@MaLoai", loai),
                new("@TrangThai", trangThai)
            };

            DatabaseHelper.ExecuteNonQuery("sp_ThemSanPham", parameters);
        }

        /// <summary>
        /// Cập nhật thông tin chi tiết của sản phẩm
        /// </summary>
        public void CapNhat(string ma, string ten, decimal gia, string dvt, string anh, string mt, string ncc, string loai, string trangThai)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new("@MaSP", ma),
                new("@TenSP", ten),
                new("@DonGia", gia),
                new("@DonViTinh", dvt),
                new("@HinhAnh", anh),
                new("@MoTa", mt),
                new("@MaNCC", ncc),
                new("@MaLoai", loai),
                new("@TrangThai", trangThai)
            };

            DatabaseHelper.ExecuteNonQuery("sp_CapNhatSanPham", parameters);
        }

        /// <summary>
        /// Xóa sản phẩm theo mã (Khuyến nghị: Nên xử lý xóa mềm/cập nhật trạng thái nếu SP đã từng được bán)
        /// </summary>
        public void Xoa(string ma)
        {
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@MaSP", ma)
            };

            DatabaseHelper.ExecuteNonQuery("sp_XoaSanPham", parameters);
        }

        public DataTable TimKiem(string kw)
        {
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@TuKhoa", kw)
            };

            return DatabaseHelper.ExecuteQuery("sp_TimKiemSanPham1", parameters);
        }

        /// <summary>
        /// Lọc sản phẩm nâng cao nhiều tiêu chí có phân trang
        /// </summary>
        public DataTable LocNangCao(string tuKhoa, string maLoai, string maNCC, string donViTinh, string trangThai, int pageNumber, int pageSize)
        {
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@TuKhoa", tuKhoa ?? ""),
                new SqlParameter("@MaLoai", maLoai ?? ""),
                new SqlParameter("@MaNCC", maNCC ?? ""),
                new SqlParameter("@DonViTinh", donViTinh ?? ""),
                new SqlParameter("@TrangThai", trangThai ?? ""),
                new SqlParameter("@PageNumber", pageNumber),
                new SqlParameter("@PageSize", pageSize)
            };

            return DatabaseHelper.ExecuteQuery("sp_LocNangCaoSanPham", parameters);
        }

        /// <summary>
        /// Tự động sinh mã sản phẩm mới (VD: SP01, SP02)
        /// </summary>
        public string TaoMa()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery("sp_TuDongTaoMaSanPham");

            if (dt.Rows.Count == 0)
            {
                return "SP01";
            }

            // Lấy mã cuối cùng trong DB (VD: "SP05")
            string lastId = dt.Rows[0][0].ToString()!.Trim();

            // Cắt 2 ký tự đầu ("SP"), chuyển phần còn lại thành số và cộng thêm 1
            int nextNumber = int.Parse(lastId.Substring(2)) + 1;

            // Format số mới thành chuỗi 2 chữ số (VD: 6 -> "06") và ghép với tiền tố
            return "SP" + nextNumber.ToString("D2");
        }
    }
}