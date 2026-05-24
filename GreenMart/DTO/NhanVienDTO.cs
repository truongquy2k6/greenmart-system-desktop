namespace DTO
{
    public class NhanVienDTO
    {
        public string MaNV { get; set; } = "";
        public string HoTen { get; set; } = "";
        public string ChucVu { get; set; } = "";
        public DateTime NgaySinh { get; set; }
        public string GioiTinh { get; set; } = "";
        public string SoDienThoai { get; set; } = "";
        public string DiaChi { get; set; } = "";
        public string TenDangNhap { get; set; } = "";
        public string MatKhau { get; set; } = "";
        public string MaCH { get; set; } = "";
        public string TrangThai { get; set; } = "Đang làm việc";
        public DateTime NgayVaoLam { get; set; } = DateTime.Now;
        // JOIN
        public string TenCH { get; set; } = "";
    }
}
