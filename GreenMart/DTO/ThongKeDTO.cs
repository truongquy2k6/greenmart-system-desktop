namespace DTO
{
    public class ThongKeDTO
    {
        public decimal DoanhThu { get; set; }
        public int SoDon { get; set; }
        public int SoKhach { get; set; }
        public int SapHet { get; set; }
        public decimal LoiNhuan { get; set; }
    }

    public class LichSuTruyCapDTO
    {
        public int MaTruyCap { get; set; }
        public string MaNV { get; set; } = "";
        public string HoTen { get; set; } = "";
        public DateTime ThoiGianDangNhap { get; set; }
        public DateTime? ThoiGianDangXuat { get; set; }
        public string DiaChiIP { get; set; } = "";
        public string ThietBi { get; set; } = "";
    }

    public class LichSuChinhSuaDTO
    {
        public int MaLS { get; set; }
        public string TenBang { get; set; } = "";
        public string MaBanGhi { get; set; } = "";
        public string HanhDong { get; set; } = "";
        public string NoiDungCu { get; set; } = "";
        public string NoiDungMoi { get; set; } = "";
        public string MaNV { get; set; } = "";
        public string HoTen { get; set; } = "";
        public DateTime ThoiGian { get; set; }
    }
}
