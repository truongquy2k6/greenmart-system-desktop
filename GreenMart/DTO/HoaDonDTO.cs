namespace DTO
{
    public class HoaDonDTO
    {
        public string MaHD { get; set; } = "";
        public DateTime NgayLap { get; set; } = DateTime.Now;
        public decimal TongTien { get; set; }
        public string MaKH { get; set; } = "";
        public string MaNV { get; set; } = "";
        public string MaCH { get; set; } = "";
        public string MaKM { get; set; } = "";
        public decimal GiamGia { get; set; }
        public string PhuongThucThanhToan { get; set; } = "Tiền mặt";
        public string TrangThai { get; set; } = "Đã thanh toán";
        // JOIN
        public string TenKhachHang { get; set; } = "";
        public string TenNhanVien { get; set; } = "";
    }

    public class ChiTietHoaDonDTO
    {
        public string MaHD { get; set; } = "";
        public string MaSP { get; set; } = "";
        public string TenSP { get; set; } = "";
        public string DonViTinh { get; set; } = "";
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
    }
}
