using Microsoft.Data.SqlClient;
using System.Data;

namespace DAL
{
    public class CuaHangDAL
    {
        /// <summary>
        /// Lấy danh sách toàn bộ cửa hàng
        /// </summary>
        public DataTable HienThi()
        {
            return DatabaseHelper.ExecuteQuery("sp_HienThiCuaHang");
        }

        /// <summary>
        /// Thêm cửa hàng mới
        /// </summary>
        public void Them(string ma, string ten, string dc, string sdt, string email)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new("@MaCH", ma),
                new("@TenCH", ten),
                new("@DiaChi", dc),
                new("@SoDienThoai", sdt),
                new("@Email", email)
            };

            DatabaseHelper.ExecuteNonQuery("sp_ThemCuaHang", parameters);
        }

        /// <summary>
        /// Cập nhật thông tin cửa hàng
        /// </summary>
        public void CapNhat(string ma, string ten, string dc, string sdt, string email)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new("@MaCH", ma),
                new("@TenCH", ten),
                new("@DiaChi", dc),
                new("@SoDienThoai", sdt),
                new("@Email", email)
            };

            DatabaseHelper.ExecuteNonQuery("sp_CapNhatCuaHang", parameters);
        }

        /// <summary>
        /// Xóa cửa hàng theo mã
        /// </summary>
        public void Xoa(string ma)
        {
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@MaCH", ma)
            };

            DatabaseHelper.ExecuteNonQuery("sp_XoaCuaHang", parameters);
        }

        /// <summary>
        /// Tự động sinh mã cửa hàng kế tiếp (VD: CH01 -> CH02)
        /// </summary>
        public string TaoMa()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery("sp_TuDongTaoMaCuaHang");

            if (dt.Rows.Count == 0)
            {
                return "CH01";
            }

            // Lấy mã cuối cùng trong DB, ví dụ "CH05"
            string lastId = dt.Rows[0][0].ToString()!.Trim();

            // Cắt chuỗi lấy phần số (bỏ "CH"), tăng lên 1 và format lại 2 chữ số
            int nextNumber = int.Parse(lastId.Substring(2)) + 1;

            return "CH" + nextNumber.ToString("D2");
        }
    }
}