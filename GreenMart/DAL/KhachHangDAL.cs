using Microsoft.Data.SqlClient;
using System.Data;

namespace DAL
{
    public class KhachHangDAL
    {
        // 1. Phương thức hiển thị (Ngắn gọn nên có thể giữ trên 1 dòng hoặc xuống dòng cho thoáng)
        public DataTable HienThi()
        {
            return DatabaseHelper.ExecuteQuery("sp_HienThiKhachHang");
        }

        // 2. Phương thức Thêm (Nhiều tham số nên xuống dòng để dễ kiểm soát)
        public void Them(string ma, string ten, string sdt, string dc, string email, int diem, string trangThai)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new("@MaKH", ma),
                new("@HoTen", ten),
                new("@SoDienThoai", sdt),
                new("@DiaChi", dc),
                new("@Email", email),
                new("@DiemTichLuy", diem),
                new("@TrangThai", trangThai)
            };

            DatabaseHelper.ExecuteNonQuery("sp_ThemKhachHang", parameters);
        }

        // 3. Phương thức Cập nhật
        public void CapNhat(string ma, string ten, string sdt, string dc, string email, int diem, string trangThai)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new("@MaKH", ma),
                new("@HoTen", ten),
                new("@SoDienThoai", sdt),
                new("@DiaChi", dc),
                new("@Email", email),
                new("@DiemTichLuy", diem),
                new("@TrangThai", trangThai)
            };

            DatabaseHelper.ExecuteNonQuery("sp_CapNhatKhachHang", parameters);
        }

        // 4. Phương thức Xóa
        public void Xoa(string ma)
        {
            SqlParameter[] parameters = new[] { new SqlParameter("@MaKH", ma) };
            DatabaseHelper.ExecuteNonQuery("sp_XoaKhachHang", parameters);
        }

        // 5. Phương thức Tạo mã (Tách logic ra để dễ hiểu)
        public string TaoMa()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery("sp_TuDongTaoMaKhachHang");

            if (dt.Rows.Count == 0)
                return "KH01";

            string lastMa = dt.Rows[0][0].ToString()!.Trim(); // Ví dụ: "KH05"
            int number = int.Parse(lastMa.Substring(2));     // Lấy số 5

            return "KH" + (number + 1).ToString("D2");       // Trả về "KH06"
        }
    }
}