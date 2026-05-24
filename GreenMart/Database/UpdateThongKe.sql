USE QuanLyHeThongGreenMart
GO

-- 1. Cập nhật sp_LayThongKeTongQuan để trả về nhiều thông tin hơn
ALTER PROCEDURE sp_LayThongKeTongQuan
AS
BEGIN
    DECLARE @DoanhThu decimal(12,2) = (SELECT ISNULL(SUM(TongTien), 0) FROM HoaDon WHERE MONTH(NgayLap) = MONTH(GETDATE()) AND YEAR(NgayLap) = YEAR(GETDATE()) AND TrangThai = N'Đã thanh toán');
    DECLARE @SoDon int = (SELECT COUNT(*) FROM HoaDon WHERE MONTH(NgayLap) = MONTH(GETDATE()) AND YEAR(NgayLap) = YEAR(GETDATE()) AND TrangThai = N'Đã thanh toán');
    DECLARE @SoKhach int = (SELECT COUNT(*) FROM KhachHang WHERE TrangThai = N'Hoạt động');
    DECLARE @SapHet int = (SELECT COUNT(DISTINCT MaSP) FROM ChiTietKho WHERE SoLuong < 20);
    
    -- Thêm các chỉ số mới
    DECLARE @TongHang int = (SELECT COUNT(*) FROM SanPham WHERE TrangThai = N'Đang kinh doanh');
    DECLARE @GiaTriKho decimal(15,2) = (SELECT ISNULL(SUM(ck.SoLuong * sp.DonGia), 0) FROM ChiTietKho ck JOIN SanPham sp ON ck.MaSP = sp.MaSP);
    DECLARE @DonTrungBinh decimal(12,2) = CASE WHEN @SoDon = 0 THEN 0 ELSE @DoanhThu / @SoDon END;

    SELECT 
        @DoanhThu as DoanhThu, 
        @SoDon as SoDon, 
        @SoKhach as SoKhach, 
        @SapHet as SapHet,
        @TongHang as TongHang,
        @GiaTriKho as GiaTriKho,
        @DonTrungBinh as DonTrungBinh,
        15.5 as TangTruong -- Giả lập % tăng trưởng
END
GO

-- 2. Tạo SP lấy Top Khách Hàng
CREATE OR ALTER PROCEDURE sp_LayTopKhachHang
AS
BEGIN
    SELECT TOP 5 
        kh.HoTen, 
        COUNT(hd.MaHD) as SoDon, 
        SUM(hd.TongTien) as TongChi
    FROM KhachHang kh
    JOIN HoaDon hd ON kh.MaKH = hd.MaKH
    WHERE hd.TrangThai = N'Đã thanh toán'
    GROUP BY kh.HoTen
    ORDER BY TongChi DESC
END
GO

-- 3. Tạo SP lấy Phân bổ doanh thu theo Loại SP
CREATE OR ALTER PROCEDURE sp_LayPhanBoLoaiSP
AS
BEGIN
    SELECT TOP 5
        l.TenLoai,
        SUM(ct.ThanhTien) as DoanhThu
    FROM LoaiSanPham l
    JOIN SanPham s ON l.MaLoai = s.MaLoai
    JOIN ChiTietHoaDon ct ON s.MaSP = ct.MaSP
    JOIN HoaDon h ON ct.MaHD = h.MaHD
    WHERE h.TrangThai = N'Đã thanh toán'
    GROUP BY l.TenLoai
    ORDER BY DoanhThu DESC
END
GO

-- 4. Tạo SP lấy Danh sách hàng sắp hết chi tiết
CREATE OR ALTER PROCEDURE sp_LayDanhSachSapHet
AS
BEGIN
    SELECT TOP 5
        s.TenSP,
        SUM(c.SoLuong) as TonKho,
        s.DonViTinh
    FROM SanPham s
    JOIN ChiTietKho c ON s.MaSP = c.MaSP
    GROUP BY s.TenSP, s.DonViTinh
    HAVING SUM(c.SoLuong) < 50
    ORDER BY TonKho ASC
END
GO
