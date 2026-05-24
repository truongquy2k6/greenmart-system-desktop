namespace DTO
{
    public class KhachHangDTO
    {
        public string MaKH { get; set; } = "";
        public string HoTen { get; set; } = "";
        public string SoDienThoai { get; set; } = "";
        public string DiaChi { get; set; } = "";
        public string Email { get; set; } = "";
        public int DiemTichLuy { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public string TrangThai { get; set; } = "Hoạt động";
    }
}
