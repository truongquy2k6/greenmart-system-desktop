namespace DTO
{
    public class KhuyenMaiDTO
    {
        public string MaKM { get; set; } = "";
        public string TenKM { get; set; } = "";
        public string MoTa { get; set; } = "";
        public string LoaiKM { get; set; } = "";
        public decimal GiaTri { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public decimal DieuKien { get; set; }
        public string TrangThai { get; set; } = "Đang áp dụng";
    }
}
