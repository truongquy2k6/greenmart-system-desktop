namespace DTO
{
    public class KhoDTO
    {
        public string MaKho { get; set; } = "";
        public string TenKho { get; set; } = "";
        public string DiaChi { get; set; } = "";
        public string SoDienThoai { get; set; } = "";
        public string MaCH { get; set; } = "";
        public string TenCH { get; set; } = "";
    }

    public class ChiTietKhoDTO
    {
        public string MaKho { get; set; } = "";
        public string TenKho { get; set; } = "";
        public string MaSP { get; set; } = "";
        public string TenSP { get; set; } = "";
        public int SoLuong { get; set; }
    }
}
