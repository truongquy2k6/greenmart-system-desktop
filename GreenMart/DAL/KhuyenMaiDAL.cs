using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace DAL
{
    /// <summary>
    /// Lớp xử lý dữ liệu (DAL) cho Khuyến Mãi
    /// </summary>
    public class KhuyenMaiDAL
    {
        /// <summary>
        /// Lấy danh sách tất cả các chương trình khuyến mãi
        /// </summary>
        public DataTable HienThi()
        {
            return DatabaseHelper.ExecuteQuery("sp_HienThiKhuyenMai");
        }

        /// <summary>
        /// Thêm một chương trình khuyến mãi mới
        /// </summary>
        public void Them(string ma, string ten, string mt, string loai, decimal gt, DateTime bd, DateTime kt, decimal dk, string tt)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new("@MaKM", ma),
                new("@TenKM", ten),
                new("@MoTa", mt),
                new("@LoaiKM", loai),
                new("@GiaTri", gt),
                new("@NgayBatDau", bd),
                new("@NgayKetThuc", kt),
                new("@DieuKien", dk),
                new("@TrangThai", tt)
            };

            DatabaseHelper.ExecuteNonQuery("sp_ThemKhuyenMai", parameters);
        }

        /// <summary>
        /// Cập nhật thông tin chương trình khuyến mãi
        /// </summary>
        public void CapNhat(string ma, string ten, string mt, string loai, decimal gt, DateTime bd, DateTime kt, decimal dk, string tt)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new("@MaKM", ma),
                new("@TenKM", ten),
                new("@MoTa", mt),
                new("@LoaiKM", loai),
                new("@GiaTri", gt),
                new("@NgayBatDau", bd),
                new("@NgayKetThuc", kt),
                new("@DieuKien", dk),
                new("@TrangThai", tt)
            };

            DatabaseHelper.ExecuteNonQuery("sp_CapNhatKhuyenMai", parameters);
        }

        /// <summary>
        /// Xóa chương trình khuyến mãi (Lưu ý về các ràng buộc dữ liệu nếu khuyến mãi đã được áp dụng)
        /// </summary>
        public void Xoa(string ma)
        {
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@MaKM", ma)
            };

            DatabaseHelper.ExecuteNonQuery("sp_XoaKhuyenMai", parameters);
        }

        /// <summary>
        /// Tự động sinh mã khuyến mãi mới (VD: KM01, KM02)
        /// </summary>
        public string TaoMa()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery("sp_TuDongTaoMaKM");

            if (dt.Rows.Count == 0)
            {
                return "KM01";
            }

            // Lấy mã cuối cùng trong DB (VD: "KM05")
            string lastId = dt.Rows[0][0].ToString()!.Trim();

            // Cắt 2 ký tự đầu ("KM"), chuyển phần còn lại thành số và cộng thêm 1
            int nextNumber = int.Parse(lastId.Substring(2)) + 1;

            // Format số mới thành chuỗi 2 chữ số (VD: 6 -> "06") và ghép với tiền tố
            return "KM" + nextNumber.ToString("D2");
        }
    }
}