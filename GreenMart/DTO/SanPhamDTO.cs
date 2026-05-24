namespace DTO
{
    public class SanPhamDTO
    {
        public string MaSP { get; set; } = "";
        public string TenSP { get; set; } = "";
        public decimal DonGia { get; set; }
        public string DonViTinh { get; set; } = "";
        public string HinhAnh { get; set; } = "";
        public string MoTa { get; set; } = "";
        public string MaNCC { get; set; } = "";
        public string MaLoai { get; set; } = "";
        public string TrangThai { get; set; } = "Đang kinh doanh";
        public DateTime NgayTao { get; set; } = DateTime.Now;
        // JOIN fields
        public string TenNCC { get; set; } = "";
        public string TenLoai { get; set; } = "";
    }
}
