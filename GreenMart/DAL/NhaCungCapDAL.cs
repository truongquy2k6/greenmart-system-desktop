using Microsoft.Data.SqlClient;
using System.Data;

namespace DAL
{
    /// <summary>
    /// Lớp xử lý dữ liệu (DAL) cho Nhà Cung Cấp
    /// </summary>
    public class NhaCungCapDAL
    {
        /// <summary>
        /// Lấy danh sách tất cả nhà cung cấp
        /// </summary>
        public DataTable HienThi()
        {
            return DatabaseHelper.ExecuteQuery("sp_HienThiNhaCungCap");
        }

        /// <summary>
        /// Thêm mới nhà cung cấp
        /// </summary>
        public void Them(string ma, string ten, string dc, string sdt, string email)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new("@MaNCC", ma),
                new("@TenNCC", ten),
                new("@DiaChi", dc),
                new("@SoDienThoai", sdt),
                new("@Email", email)
            };

            DatabaseHelper.ExecuteNonQuery("sp_ThemNhaCungCap", parameters);
        }

        /// <summary>
        /// Cập nhật thông tin nhà cung cấp
        /// </summary>
        public void CapNhat(string ma, string ten, string dc, string sdt, string email)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new("@MaNCC", ma),
                new("@TenNCC", ten),
                new("@DiaChi", dc),
                new("@SoDienThoai", sdt),
                new("@Email", email)
            };

            DatabaseHelper.ExecuteNonQuery("sp_CapNhatNhaCungCap", parameters);
        }

        /// <summary>
        /// Xóa nhà cung cấp theo mã (Lưu ý: nên dùng xóa mềm/đổi trạng thái nếu NCC đã có giao dịch)
        /// </summary>
        public void Xoa(string ma)
        {
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@MaNCC", ma)
            };

            DatabaseHelper.ExecuteNonQuery("sp_XoaNhaCungCap", parameters);
        }

        /// <summary>
        /// Tìm kiếm nhà cung cấp theo từ khóa (Mã, Tên, SĐT, Email)
        /// </summary>
        public DataTable TimKiem(string kw)
        {
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@TuKhoa", kw)
            };

            return DatabaseHelper.ExecuteQuery("sp_TimKiemNhaCungCap", parameters);
        }

        /// <summary>
        /// Tự động sinh mã nhà cung cấp mới (VD: NCC01, NCC02)
        /// </summary>
        public string TaoMa()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery("sp_TuDongTaoMaNCC");

            if (dt.Rows.Count == 0)
            {
                return "NCC01";
            }

            // Lấy mã cuối cùng trong DB (VD: "NCC05")
            string lastId = dt.Rows[0][0].ToString()!.Trim();

            // Cắt 3 ký tự đầu ("NCC"), chuyển phần còn lại thành số và cộng thêm 1
            int nextNumber = int.Parse(lastId.Substring(3)) + 1;

            // Format số mới thành chuỗi 2 chữ số (VD: 6 -> "06") và ghép với tiền tố
            return "NCC" + nextNumber.ToString("D2");
        }
    }
}