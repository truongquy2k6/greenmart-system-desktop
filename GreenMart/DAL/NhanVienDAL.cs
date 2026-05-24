using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace DAL
{
    /// <summary>
    /// Lớp xử lý dữ liệu (DAL) cho Nhân Viên
    /// </summary>
    public class NhanVienDAL
    {
        /// <summary>
        /// Kiểm tra thông tin đăng nhập của nhân viên
        /// </summary>
        public DataTable DangNhap(string tenDN, string matKhau)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new("@TenDangNhap", tenDN),
                new("@MatKhau", matKhau)
            };

            return DatabaseHelper.ExecuteQuery("sp_DangNhap", parameters);
        }

        /// <summary>
        /// Lấy danh sách toàn bộ nhân viên
        /// </summary>
        public DataTable HienThi()
        {
            return DatabaseHelper.ExecuteQuery("sp_HienThiNhanVien");
        }

        /// <summary>
        /// Thêm mới một nhân viên vào hệ thống
        /// </summary>
        public void Them(string ma, string ten, string cv, DateTime ns, string gt, string sdt, string dc, string tdn, string mk, string maCH, string tt)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new("@MaNV", ma),
                new("@HoTen", ten),
                new("@ChucVu", cv),
                new("@NgaySinh", ns),
                new("@GioiTinh", gt),
                new("@SoDienThoai", sdt),
                new("@DiaChi", dc),
                new("@TenDangNhap", tdn),
                new("@MatKhau", mk),
                new("@MaCH", maCH),
                new("@TrangThai", tt)
            };

            DatabaseHelper.ExecuteNonQuery("sp_ThemNhanVien", parameters);
        }

        /// <summary>
        /// Cập nhật thông tin nhân viên
        /// </summary>
        public void CapNhat(string ma, string ten, string cv, DateTime ns, string gt, string sdt, string dc, string tdn, string mk, string maCH, string tt)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new("@MaNV", ma),
                new("@HoTen", ten),
                new("@ChucVu", cv),
                new("@NgaySinh", ns),
                new("@GioiTinh", gt),
                new("@SoDienThoai", sdt),
                new("@DiaChi", dc),
                new("@TenDangNhap", tdn),
                new("@MatKhau", mk),
                new("@MaCH", maCH),
                new("@TrangThai", tt)
            };

            DatabaseHelper.ExecuteNonQuery("sp_CapNhatNhanVien", parameters);
        }

        /// <summary>
        /// Xóa nhân viên theo mã (Lưu ý: nên xử lý đổi trạng thái thay vì xóa cứng nếu đã có giao dịch)
        /// </summary>
        public void Xoa(string ma)
        {
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@MaNV", ma)
            };

            DatabaseHelper.ExecuteNonQuery("sp_XoaNhanVien", parameters);
        }

        /// <summary>
        /// Tự động sinh mã nhân viên mới (VD: NV01, NV02)
        /// </summary>
        public string TaoMaTuDong()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery("sp_TuDongTaoMaNhanVien");

            if (dt.Rows.Count == 0)
            {
                return "NV01";
            }

            // Lấy mã cuối cùng trong DB (VD: "NV05")
            string lastId = dt.Rows[0][0].ToString()!.Trim();

            // Cắt 2 ký tự đầu ("NV"), chuyển phần còn lại thành số và cộng thêm 1
            int nextNumber = int.Parse(lastId.Substring(2)) + 1;

            // Format số mới thành chuỗi 2 chữ số (VD: 6 -> "06") và ghép với tiền tố
            return "NV" + nextNumber.ToString("D2");
        }
    }
}