using Microsoft.Data.SqlClient;
using System.Data;

namespace DAL
{
    /// <summary>
    /// Lớp xử lý dữ liệu (DAL) cho Loại Sản Phẩm
    /// </summary>
    public class LoaiSanPhamDAL
    {
        /// <summary>
        /// Lấy danh sách tất cả các loại sản phẩm
        /// </summary>
        public DataTable HienThi(int pageNumber = 1, int pageSize = 10000)
        {
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@PageNumber", pageNumber),
                new SqlParameter("@PageSize", pageSize)
            };
            return DatabaseHelper.ExecuteQuery("sp_HienThiLoaiSanPham", parameters);
        }

        /// <summary>
        /// Thêm mới một loại sản phẩm
        /// </summary>
        public void Them(string ma, string ten)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new("@MaLoai", ma),
                new("@TenLoai", ten)
            };

            DatabaseHelper.ExecuteNonQuery("sp_ThemLoaiSanPham", parameters);
        }

        /// <summary>
        /// Cập nhật thông tin loại sản phẩm
        /// </summary>
        public void CapNhat(string ma, string ten)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new("@MaLoai", ma),
                new("@TenLoai", ten)
            };

            DatabaseHelper.ExecuteNonQuery("sp_CapNhatLoaiSanPham", parameters);
        }

        /// <summary>
        /// Xóa loại sản phẩm theo mã
        /// </summary>
        public void Xoa(string ma)
        {
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@MaLoai", ma)
            };

            DatabaseHelper.ExecuteNonQuery("sp_XoaLoaiSanPham", parameters);
        }

        /// <summary>
        /// Tìm kiếm loại sản phẩm theo từ khóa (Mã hoặc Tên)
        /// </summary>
        public DataTable TimKiem(string kw, int pageNumber = 1, int pageSize = 10000)
        {
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@TuKhoa", kw ?? ""),
                new SqlParameter("@PageNumber", pageNumber),
                new SqlParameter("@PageSize", pageSize)
            };

            return DatabaseHelper.ExecuteQuery("sp_TimKiemLoaiSanPham", parameters);
        }

        /// <summary>
        /// Tự động sinh mã loại sản phẩm mới (VD: L01, L02)
        /// </summary>
        public string TaoMa()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery("sp_TuDongTaoMaLoaiSP");

            if (dt.Rows.Count == 0)
            {
                return "L01";
            }

            // Lấy mã cuối cùng trong DB (VD: "L05")
            string lastId = dt.Rows[0][0].ToString()!.Trim();

            // Cắt 1 ký tự đầu ("L"), chuyển phần còn lại thành số và cộng thêm 1
            int nextNumber = int.Parse(lastId.Substring(1)) + 1;

            // Format số mới thành chuỗi 2 chữ số (VD: 6 -> "06") và ghép với tiền tố
            return "L" + nextNumber.ToString("D2");
        }
    }
}