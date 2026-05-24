USE QuanLyHeThongGreenMart;
GO
ALTER PROC [dbo].[sp_LayThongKeTongQuan]
AS
BEGIN
    DECLARE @DoanhThu decimal(12,2) = (SELECT ISNULL(SUM(TongTien), 0) FROM HoaDon WHERE MONTH(NgayLap) = MONTH(GETDATE()) AND YEAR(NgayLap) = YEAR(GETDATE()) AND TrangThai = N'Đã thanh toán');
    DECLARE @SoDon int = (SELECT COUNT(*) FROM HoaDon WHERE MONTH(NgayLap) = MONTH(GETDATE()) AND YEAR(NgayLap) = YEAR(GETDATE()) AND TrangThai = N'Đã thanh toán');
    DECLARE @SoKhach int = (SELECT COUNT(*) FROM KhachHang WHERE TrangThai = N'Hoạt động');
    
    DECLARE @SapHet int = (SELECT COUNT(*) FROM (SELECT MaSP FROM ChiTietKho GROUP BY MaSP HAVING SUM(SoLuong) < 50) as T);
    
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
        15.5 as TangTruong
END
