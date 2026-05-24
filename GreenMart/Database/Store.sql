USE QuanLyHeThongGreenMart
GO

-------------------------------------------------------TIỆN ÍCH HỆ THỐNG-------------------------------------------------------
CREATE OR ALTER PROC sp_GhiNhanChinhSua
    @MaNV char(10), @TenBang nvarchar(50), @HanhDong nvarchar(50), 
    @MaBanGhi char(10), @NoiDungCu nvarchar(max), @NoiDungMoi nvarchar(max)
AS BEGIN
    INSERT INTO LichSuChinhSua(MaNV, TenBang, HanhDong, MaBanGhi, NoiDungCu, NoiDungMoi, ThoiGian)
    VALUES (@MaNV, @TenBang, @HanhDong, @MaBanGhi, @NoiDungCu, @NoiDungMoi, GETDATE())
END
GO


-------------------------------------------------------ĐĂNG NHẬP-------------------------------------------------------
CREATE OR ALTER PROC sp_DangNhap
    @TenDangNhap varchar(50),
    @MatKhau varchar(255)
AS
BEGIN
    SELECT * FROM NhanVien 
    WHERE TenDangNhap COLLATE SQL_Latin1_General_CP1_CS_AS = @TenDangNhap 
      AND MatKhau = @MatKhau
      AND TrangThai = N'Đang làm việc'
END
GO

-------------------------------------------------------TRANG CHỦ-------------------------------------------------------
CREATE OR ALTER PROC sp_LayThongKeTongQuan
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

CREATE OR ALTER PROC sp_LayBieuDoDoanhThu
AS
BEGIN
    SELECT FORMAT(NgayLap, 'dd/MM') as Ngay, SUM(TongTien) as Tien FROM HoaDon 
    WHERE NgayLap >= DATEADD(day, -7, GETDATE())
    GROUP BY FORMAT(NgayLap, 'dd/MM'), CAST(NgayLap AS DATE) ORDER BY CAST(NgayLap AS DATE) ASC
END
GO

CREATE OR ALTER PROC sp_LayTopSanPham
AS
BEGIN
    SELECT TOP 5 SP.TenSP, SUM(CT.SoLuong) as DaBan
    FROM ChiTietHoaDon CT JOIN SanPham SP ON CT.MaSP = SP.MaSP
    JOIN HoaDon HD ON CT.MaHD = HD.MaHD
    GROUP BY SP.TenSP ORDER BY DaBan DESC
END
GO

CREATE OR ALTER PROC sp_LayHoaDonGanDay
AS
BEGIN
    SELECT TOP 10 HD.MaHD, HD.NgayLap, HD.TongTien, HD.TrangThai,
        ISNULL(KH.HoTen, N'Khách vãng lai') as TenKhachHang
    FROM HoaDon HD LEFT JOIN KhachHang KH ON HD.MaKH = KH.MaKH
    ORDER BY HD.NgayLap DESC
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
GO

-------------------------------------------------------BÁN HÀNG (POS)-------------------------------------------------------
CREATE OR ALTER PROC sp_TimKiemSanPham
    @TuKhoa nvarchar(100), @MaCH char(10)
AS
BEGIN
    SELECT SP.MaSP, SP.TenSP, SP.DonGia, SP.DonViTinh, SUM(CTK.SoLuong) AS TonKho
    FROM SanPham SP
    INNER JOIN ChiTietKho CTK ON SP.MaSP = CTK.MaSP
    INNER JOIN Kho K ON CTK.MaKho = K.MaKho
    WHERE K.MaCH = @MaCH AND SP.TrangThai = N'Đang kinh doanh'
      AND (SP.MaSP LIKE '%' + @TuKhoa + '%' OR SP.TenSP LIKE N'%' + @TuKhoa + '%')
    GROUP BY SP.MaSP, SP.TenSP, SP.DonGia, SP.DonViTinh
    HAVING SUM(CTK.SoLuong) > 0
END
GO

CREATE OR ALTER PROC sp_ThemHoaDon
    @MaHD char(10), @TongTien decimal(18,2), @MaKH char(10), @MaNV char(10),
    @MaKM char(10) = NULL, @GiamGia decimal(12,2) = 0, @PhuongThucThanhToan nvarchar(50) = N'Tiền mặt'
AS
BEGIN
    DECLARE @MaCH char(10)
    SELECT @MaCH = MaCH FROM NhanVien WHERE MaNV = @MaNV
    IF @MaCH IS NULL BEGIN RAISERROR(N'Không tìm thấy cửa hàng!', 16, 1) RETURN END
    INSERT INTO HoaDon (MaHD, NgayLap, TongTien, MaKH, MaNV, MaCH, MaKM, GiamGia, PhuongThucThanhToan)
    VALUES (@MaHD, GETDATE(), @TongTien, @MaKH, @MaNV, @MaCH, @MaKM, @GiamGia, @PhuongThucThanhToan)
END
GO

CREATE OR ALTER PROC sp_ThemChiTietHD
    @MaHD char(10), @MaSP char(10), @SoLuong int, @DonGia decimal(18,2)
AS
BEGIN
    DECLARE @MaCH char(10), @MaKho char(10), @TonKho int
    SELECT @MaCH = MaCH FROM HoaDon WHERE MaHD = @MaHD
    SELECT TOP 1 @MaKho = K.MaKho, @TonKho = CTK.SoLuong
    FROM Kho K JOIN ChiTietKho CTK ON K.MaKho = CTK.MaKho
    WHERE K.MaCH = @MaCH AND CTK.MaSP = @MaSP AND CTK.SoLuong > 0
    IF @MaKho IS NULL BEGIN RAISERROR(N'Sản phẩm đã hết hàng!', 16, 1) RETURN END
    IF @TonKho < @SoLuong BEGIN RAISERROR(N'Không đủ số lượng trong kho!', 16, 1) RETURN END
    INSERT INTO ChiTietHoaDon VALUES (@MaHD, @MaSP, @SoLuong, @DonGia, @SoLuong * @DonGia)
    UPDATE ChiTietKho SET SoLuong = SoLuong - @SoLuong WHERE MaKho = @MaKho AND MaSP = @MaSP
END
GO

CREATE OR ALTER PROC sp_TimKhachHangTheoSDT @SDT varchar(15)
AS SELECT MaKH, HoTen, DiemTichLuy FROM KhachHang WHERE SoDienThoai = @SDT
GO

CREATE OR ALTER PROC sp_TuDongTaoMaHD
AS SELECT TOP 1 MaHD FROM HoaDon ORDER BY CAST(SUBSTRING(MaHD, 3, 10) AS INT) DESC
GO

-------------------------------------------------------SẢN PHẨM-------------------------------------------------------
CREATE OR ALTER PROC sp_HienThiSanPham
AS
BEGIN
    SELECT SP.*, NCC.TenNCC, LSP.TenLoai
    FROM SanPham SP INNER JOIN NhaCungCap NCC ON SP.MaNCC = NCC.MaNCC INNER JOIN LoaiSanPham LSP ON SP.MaLoai = LSP.MaLoai
END
GO

CREATE OR ALTER PROC sp_ThemSanPham(@MaSP char(10), @TenSP nvarchar(100), @DonGia decimal(10,2), @DonViTinh nvarchar(20), @HinhAnh nvarchar(255), @MoTa nvarchar(255), @MaNCC char(10), @MaLoai char(10), @TrangThai nvarchar(50) = N'Đang kinh doanh')
AS BEGIN
    INSERT INTO SanPham(MaSP, TenSP, DonGia, DonViTinh, HinhAnh, MoTa, MaNCC, MaLoai, TrangThai) VALUES (@MaSP, @TenSP, @DonGia, @DonViTinh, @HinhAnh, @MoTa, @MaNCC, @MaLoai, @TrangThai)
END
GO

CREATE OR ALTER PROC sp_CapNhatSanPham(@MaSP char(10), @TenSP nvarchar(100), @DonGia decimal(10,2), @DonViTinh nvarchar(20), @HinhAnh nvarchar(255), @MoTa nvarchar(255), @MaNCC char(10), @MaLoai char(10), @TrangThai nvarchar(50))
AS BEGIN
    UPDATE SanPham SET TenSP=@TenSP, DonGia=@DonGia, DonViTinh=@DonViTinh, HinhAnh=@HinhAnh, MoTa=@MoTa, MaNCC=@MaNCC, MaLoai=@MaLoai, TrangThai=@TrangThai WHERE MaSP=@MaSP
END
GO

CREATE OR ALTER PROC sp_XoaSanPham(@MaSP char(10)) AS DELETE SanPham WHERE MaSP = @MaSP
GO

CREATE OR ALTER PROC sp_TimKiemSanPham1(@TuKhoa nvarchar(100))
AS
BEGIN
    SELECT SP.*, NCC.TenNCC, LSP.TenLoai
    FROM SanPham SP INNER JOIN NhaCungCap NCC ON SP.MaNCC = NCC.MaNCC INNER JOIN LoaiSanPham LSP ON SP.MaLoai = LSP.MaLoai
    WHERE SP.MaSP LIKE '%'+@TuKhoa+'%' OR SP.TenSP LIKE N'%'+@TuKhoa+N'%' OR NCC.TenNCC LIKE N'%'+@TuKhoa+N'%' OR LSP.TenLoai LIKE N'%'+@TuKhoa+N'%'
END
GO

CREATE OR ALTER PROC sp_TuDongTaoMaSanPham AS SELECT TOP 1 MaSP FROM SanPham ORDER BY CAST(SUBSTRING(MaSP, 3, 10) AS INT) DESC
GO

-------------------------------------------------------LOẠI SẢN PHẨM-------------------------------------------------------
CREATE OR ALTER PROC sp_HienThiLoaiSanPham AS SELECT * FROM LoaiSanPham
GO
CREATE OR ALTER PROC sp_ThemLoaiSanPham(@MaLoai char(10), @TenLoai nvarchar(50)) AS INSERT INTO LoaiSanPham VALUES(@MaLoai, @TenLoai)
GO
CREATE OR ALTER PROC sp_CapNhatLoaiSanPham(@MaLoai char(10), @TenLoai nvarchar(50)) AS UPDATE LoaiSanPham SET TenLoai=@TenLoai WHERE MaLoai=@MaLoai
GO
CREATE OR ALTER PROC sp_XoaLoaiSanPham(@MaLoai char(10)) AS DELETE LoaiSanPham WHERE MaLoai = @MaLoai
GO
CREATE OR ALTER PROC sp_TimKiemLoaiSanPham(@TuKhoa nvarchar(100)) AS SELECT * FROM LoaiSanPham WHERE TenLoai LIKE N'%'+@TuKhoa+N'%' OR MaLoai LIKE '%'+@TuKhoa+'%'
GO
CREATE OR ALTER PROC sp_TuDongTaoMaLoaiSP AS SELECT TOP 1 MaLoai FROM LoaiSanPham ORDER BY CAST(SUBSTRING(MaLoai, 2, 10) AS INT) DESC
GO

-------------------------------------------------------NHÀ CUNG CẤP-------------------------------------------------------
CREATE OR ALTER PROC sp_HienThiNhaCungCap AS SELECT * FROM NhaCungCap
GO
CREATE OR ALTER PROC sp_ThemNhaCungCap(@MaNCC char(10), @TenNCC nvarchar(100), @DiaChi nvarchar(200), @SoDienThoai varchar(15), @Email varchar(100))
AS INSERT INTO NhaCungCap(MaNCC,TenNCC,DiaChi,SoDienThoai,Email) VALUES(@MaNCC,@TenNCC,@DiaChi,@SoDienThoai,@Email)
GO
CREATE OR ALTER PROC sp_CapNhatNhaCungCap(@MaNCC char(10), @TenNCC nvarchar(100), @DiaChi nvarchar(200), @SoDienThoai varchar(15), @Email varchar(100))
AS UPDATE NhaCungCap SET TenNCC=@TenNCC, DiaChi=@DiaChi, SoDienThoai=@SoDienThoai, Email=@Email WHERE MaNCC=@MaNCC
GO
CREATE OR ALTER PROC sp_XoaNhaCungCap(@MaNCC char(10)) AS DELETE NhaCungCap WHERE MaNCC = @MaNCC
GO
CREATE OR ALTER PROC sp_TimKiemNhaCungCap(@TuKhoa nvarchar(100)) AS SELECT * FROM NhaCungCap WHERE TenNCC LIKE N'%'+@TuKhoa+N'%' OR MaNCC LIKE '%'+@TuKhoa+'%' OR SoDienThoai LIKE '%'+@TuKhoa+'%' OR Email LIKE '%'+@TuKhoa+'%'
GO
CREATE OR ALTER PROC sp_TuDongTaoMaNCC AS SELECT TOP 1 MaNCC FROM NhaCungCap ORDER BY CAST(SUBSTRING(MaNCC, 4, 10) AS INT) DESC
GO

-------------------------------------------------------KHÁCH HÀNG-------------------------------------------------------
CREATE OR ALTER PROC sp_HienThiKhachHang AS SELECT * FROM KhachHang
GO
CREATE OR ALTER PROC sp_ThemKhachHang(@MaKH char(10), @HoTen nvarchar(100), @SoDienThoai varchar(15), @DiaChi nvarchar(200), @Email varchar(100), @DiemTichLuy int, @TrangThai nvarchar(50) = N'Hoạt động')
AS INSERT INTO KhachHang(MaKH,HoTen,SoDienThoai,DiaChi,Email,DiemTichLuy,TrangThai) VALUES(@MaKH,@HoTen,@SoDienThoai,@DiaChi,@Email,@DiemTichLuy,@TrangThai)
GO
CREATE OR ALTER PROC sp_CapNhatKhachHang(@MaKH char(10), @HoTen nvarchar(100), @SoDienThoai varchar(15), @DiaChi nvarchar(200), @Email varchar(100), @DiemTichLuy int, @TrangThai nvarchar(50))
AS UPDATE KhachHang SET HoTen=@HoTen, SoDienThoai=@SoDienThoai, DiaChi=@DiaChi, Email=@Email, DiemTichLuy=@DiemTichLuy, TrangThai=@TrangThai WHERE MaKH=@MaKH
GO
CREATE OR ALTER PROC sp_XoaKhachHang(@MaKH char(10))
AS BEGIN
    UPDATE HoaDon SET MaKH = NULL WHERE MaKH = @MaKH
    DELETE KhachHang WHERE MaKH = @MaKH
END
GO
CREATE OR ALTER PROC sp_TuDongTaoMaKhachHang AS SELECT TOP 1 MaKH FROM KhachHang ORDER BY CAST(SUBSTRING(MaKH, 3, 10) AS INT) DESC
GO

-------------------------------------------------------NHÂN VIÊN-------------------------------------------------------
CREATE OR ALTER PROC sp_HienThiNhanVien
AS SELECT NV.*, CH.TenCH
    FROM NhanVien NV INNER JOIN CuaHang CH ON NV.MaCH = CH.MaCH
GO
CREATE OR ALTER PROC sp_ThemNhanVien(@MaNV char(10), @HoTen nvarchar(100), @ChucVu nvarchar(50), @NgaySinh date, @GioiTinh nchar(10), @SoDienThoai varchar(15), @DiaChi nvarchar(200), @TenDangNhap varchar(50), @MatKhau varchar(255), @MaCH char(10), @TrangThai nvarchar(50) = N'Đang làm việc')
AS INSERT INTO NhanVien(MaNV,HoTen,ChucVu,NgaySinh,GioiTinh,SoDienThoai,DiaChi,TenDangNhap,MatKhau,MaCH,TrangThai) VALUES(@MaNV,@HoTen,@ChucVu,@NgaySinh,@GioiTinh,@SoDienThoai,@DiaChi,@TenDangNhap,@MatKhau,@MaCH,@TrangThai)
GO
CREATE OR ALTER PROC sp_CapNhatNhanVien(@MaNV char(10), @HoTen nvarchar(100), @ChucVu nvarchar(50), @NgaySinh date, @GioiTinh nchar(10), @SoDienThoai varchar(15), @DiaChi nvarchar(200), @TenDangNhap varchar(50), @MatKhau varchar(255), @MaCH char(10), @TrangThai nvarchar(50))
AS UPDATE NhanVien SET HoTen=@HoTen, ChucVu=@ChucVu, NgaySinh=@NgaySinh, GioiTinh=@GioiTinh, SoDienThoai=@SoDienThoai, DiaChi=@DiaChi, TenDangNhap=@TenDangNhap, MatKhau=CASE WHEN @MatKhau='' THEN MatKhau ELSE @MatKhau END, MaCH=@MaCH, TrangThai=@TrangThai WHERE MaNV=@MaNV
GO
CREATE OR ALTER PROC sp_XoaNhanVien(@MaNV char(10)) AS DELETE NhanVien WHERE MaNV = @MaNV
GO
CREATE OR ALTER PROC sp_TuDongTaoMaNhanVien AS SELECT TOP 1 MaNV FROM NhanVien ORDER BY CAST(SUBSTRING(MaNV, 3, 10) AS INT) DESC
GO

-------------------------------------------------------HOÁ ĐƠN-------------------------------------------------------
CREATE OR ALTER PROC sp_TimKiemHoaDon @TuNgay datetime, @DenNgay datetime, @TuKhoa nvarchar(100) = N''
AS BEGIN
    SELECT HD.MaHD, HD.NgayLap, HD.TongTien, HD.GiamGia, HD.PhuongThucThanhToan, HD.TrangThai,
        ISNULL(KH.HoTen, N'Khách vãng lai') as TenKhachHang, ISNULL(NV.HoTen, N'Hệ thống') as TenNhanVien
    FROM HoaDon HD 
    LEFT JOIN KhachHang KH ON HD.MaKH = KH.MaKH 
    LEFT JOIN NhanVien NV ON HD.MaNV = NV.MaNV
    WHERE (HD.NgayLap >= @TuNgay AND HD.NgayLap <= @DenNgay)
      AND (HD.MaHD LIKE '%'+@TuKhoa+'%' OR ISNULL(KH.HoTen,N'') LIKE N'%'+@TuKhoa+N'%')
    ORDER BY HD.NgayLap DESC
END
GO
CREATE OR ALTER PROC sp_LayChiTietHoaDon @MaHD char(10)
AS SELECT SP.MaSP, SP.TenSP, SP.DonViTinh, CT.SoLuong, CT.DonGia, CT.ThanhTien
    FROM ChiTietHoaDon CT JOIN SanPham SP ON CT.MaSP = SP.MaSP WHERE CT.MaHD = @MaHD
GO
CREATE OR ALTER PROC sp_HuyHoaDon @MaHD char(10)
AS BEGIN
    BEGIN TRY BEGIN TRANSACTION
        UPDATE ChiTietKho SET SoLuong = ChiTietKho.SoLuong + CT.SoLuong
        FROM ChiTietKho JOIN ChiTietHoaDon CT ON ChiTietKho.MaSP = CT.MaSP WHERE CT.MaHD = @MaHD AND ChiTietKho.MaKho = 'K01'
        
        DECLARE @NoiDungCu nvarchar(max) = (SELECT N'Hóa đơn: ' + MaHD + N', Tổng tiền: ' + CAST(TongTien as nvarchar) FROM HoaDon WHERE MaHD = @MaHD)
        DECLARE @MaNV char(10) = (SELECT MaNV FROM HoaDon WHERE MaHD = @MaHD)

        DELETE FROM ChiTietHoaDon WHERE MaHD = @MaHD
        DELETE FROM HoaDon WHERE MaHD = @MaHD

        EXEC sp_GhiNhanChinhSua @MaNV, N'HoaDon', 'DELETE', @MaHD, @NoiDungCu, N'Đã xóa hóa đơn và hoàn kho'
    COMMIT END TRY BEGIN CATCH ROLLBACK RAISERROR(N'Lỗi hủy hóa đơn!', 16, 1) END CATCH
END
GO

-------------------------------------------------------CỬA HÀNG-------------------------------------------------------
CREATE OR ALTER PROC sp_HienThiCuaHang AS SELECT * FROM CuaHang
GO
CREATE OR ALTER PROC sp_ThemCuaHang(@MaCH char(10), @TenCH nvarchar(100), @DiaChi nvarchar(200), @SoDienThoai varchar(15), @Email varchar(100))
AS INSERT INTO CuaHang VALUES(@MaCH,@TenCH,@DiaChi,@SoDienThoai,@Email)
GO
CREATE OR ALTER PROC sp_CapNhatCuaHang(@MaCH char(10), @TenCH nvarchar(100), @DiaChi nvarchar(200), @SoDienThoai varchar(15), @Email varchar(100))
AS UPDATE CuaHang SET TenCH=@TenCH, DiaChi=@DiaChi, SoDienThoai=@SoDienThoai, Email=@Email WHERE MaCH=@MaCH
GO
CREATE OR ALTER PROC sp_XoaCuaHang(@MaCH char(10)) AS DELETE CuaHang WHERE MaCH = @MaCH
GO
CREATE OR ALTER PROC sp_TuDongTaoMaCuaHang AS SELECT TOP 1 MaCH FROM CuaHang ORDER BY CAST(SUBSTRING(MaCH, 3, 10) AS INT) DESC
GO

-------------------------------------------------------KHO-------------------------------------------------------
CREATE OR ALTER PROC sp_HienThiKho
AS SELECT K.*, CH.TenCH FROM Kho K INNER JOIN CuaHang CH ON K.MaCH = CH.MaCH
GO
CREATE OR ALTER PROC sp_HienThiChiTietKhoTheoMa(@MaKho CHAR(10))
AS SELECT CT.MaKho, K.TenKho, SP.MaSP, SP.TenSP, CT.SoLuong FROM ChiTietKho CT JOIN SanPham SP ON CT.MaSP = SP.MaSP JOIN Kho K ON CT.MaKho = K.MaKho WHERE CT.MaKho = @MaKho ORDER BY SP.TenSP
GO

-- Lấy toàn bộ chi tiết kho (tất cả kho) - dùng cho tab Chi tiết kho
CREATE OR ALTER PROC sp_HienThiTatCaChiTietKho
AS SELECT CT.MaKho, K.TenKho, SP.MaSP, SP.TenSP, CT.SoLuong 
   FROM ChiTietKho CT JOIN SanPham SP ON CT.MaSP = SP.MaSP JOIN Kho K ON CT.MaKho = K.MaKho 
   ORDER BY K.TenKho, SP.TenSP
GO
CREATE OR ALTER PROC sp_ThemKho(@MaKho CHAR(10), @TenKho NVARCHAR(100), @DiaChi NVARCHAR(200), @SoDienThoai NVARCHAR(15), @MaCH CHAR(10))
AS INSERT INTO Kho VALUES(@MaKho,@TenKho,@DiaChi,@SoDienThoai,@MaCH)
GO
CREATE OR ALTER PROC sp_CapNhatKho(@MaKho CHAR(10), @TenKho NVARCHAR(100), @DiaChi NVARCHAR(200), @SoDienThoai NVARCHAR(15), @MaCH CHAR(10))
AS UPDATE Kho SET TenKho=@TenKho, DiaChi=@DiaChi, SoDienThoai=@SoDienThoai, MaCH=@MaCH WHERE MaKho=@MaKho
GO
CREATE OR ALTER PROC sp_XoaKho(@MaKho CHAR(10)) 
AS 
BEGIN 
    IF EXISTS (SELECT 1 FROM ChiTietKho WHERE MaKho = @MaKho AND SoLuong > 0)
    BEGIN
        RAISERROR(N'Không thể xóa kho vì vẫn còn hàng tồn kho!', 16, 1);
        RETURN;
    END
    DELETE FROM ChiTietKho WHERE MaKho = @MaKho; 
    DELETE FROM Kho WHERE MaKho = @MaKho;
END
GO
CREATE OR ALTER PROC sp_ThemChiTietKho(@MaKho CHAR(10), @MaSP CHAR(10), @SoLuong INT)
AS BEGIN
    IF EXISTS (SELECT 1 FROM ChiTietKho WHERE MaKho=@MaKho AND MaSP=@MaSP)
        UPDATE ChiTietKho SET SoLuong=SoLuong+@SoLuong WHERE MaKho=@MaKho AND MaSP=@MaSP
    ELSE INSERT INTO ChiTietKho VALUES(@MaKho,@MaSP,@SoLuong)
END
GO
CREATE OR ALTER PROC sp_CapNhatChiTietKho(@MaKho CHAR(10), @MaSP CHAR(10), @SoLuong INT)
AS UPDATE ChiTietKho SET SoLuong=@SoLuong WHERE MaKho=@MaKho AND MaSP=@MaSP
GO
CREATE OR ALTER PROC sp_XoaChiTietKho(@MaKho CHAR(10), @MaSP CHAR(10))
AS DELETE ChiTietKho WHERE MaKho=@MaKho AND MaSP=@MaSP
GO
CREATE OR ALTER PROC sp_TuDongTaoMaKho AS SELECT TOP 1 MaKho FROM Kho ORDER BY CAST(SUBSTRING(MaKho, 2, 10) AS INT) DESC
GO

-------------------------------------------------------KHUYẾN MÃI-------------------------------------------------------
CREATE OR ALTER PROC sp_HienThiKhuyenMai AS SELECT * FROM KhuyenMai ORDER BY NgayBatDau DESC
GO
CREATE OR ALTER PROC sp_ThemKhuyenMai(@MaKM char(10), @TenKM nvarchar(100), @MoTa nvarchar(255), @LoaiKM nvarchar(50), @GiaTri decimal(12,2), @NgayBatDau datetime, @NgayKetThuc datetime, @DieuKien decimal(12,2), @TrangThai nvarchar(50))
AS INSERT INTO KhuyenMai VALUES(@MaKM,@TenKM,@MoTa,@LoaiKM,@GiaTri,@NgayBatDau,@NgayKetThuc,@DieuKien,@TrangThai)
GO
CREATE OR ALTER PROC sp_CapNhatKhuyenMai(@MaKM char(10), @TenKM nvarchar(100), @MoTa nvarchar(255), @LoaiKM nvarchar(50), @GiaTri decimal(12,2), @NgayBatDau datetime, @NgayKetThuc datetime, @DieuKien decimal(12,2), @TrangThai nvarchar(50))
AS UPDATE KhuyenMai SET TenKM=@TenKM, MoTa=@MoTa, LoaiKM=@LoaiKM, GiaTri=@GiaTri, NgayBatDau=@NgayBatDau, NgayKetThuc=@NgayKetThuc, DieuKien=@DieuKien, TrangThai=@TrangThai WHERE MaKM=@MaKM
GO
CREATE OR ALTER PROC sp_XoaKhuyenMai(@MaKM char(10)) AS DELETE KhuyenMai WHERE MaKM = @MaKM
GO
CREATE OR ALTER PROC sp_TuDongTaoMaKM AS SELECT TOP 1 MaKM FROM KhuyenMai ORDER BY CAST(SUBSTRING(MaKM, 3, 10) AS INT) DESC
GO

-------------------------------------------------------LỊCH SỬ-------------------------------------------------------
CREATE OR ALTER PROC sp_LayLichSuTruyCap
AS SELECT TOP 500 L.*, NV.HoTen FROM LichSuTruyCap L JOIN NhanVien NV ON L.MaNV = NV.MaNV ORDER BY ThoiGianDangNhap DESC
GO

-- Ghi nhận đăng nhập: tạo bản ghi mới trong LichSuTruyCap
CREATE OR ALTER PROC sp_GhiNhanTruyCap
    @MaNV CHAR(10),
    @DiaChiIP VARCHAR(50),
    @ThietBi NVARCHAR(100)
AS
BEGIN
    INSERT INTO LichSuTruyCap(MaNV, ThoiGianDangNhap, DiaChiIP, ThietBi)
    VALUES (@MaNV, GETDATE(), @DiaChiIP, @ThietBi)
END
GO

-- Ghi nhận đăng xuất: cập nhật bản ghi chưa có ThoiGianDangXuat của nhân viên
CREATE OR ALTER PROC sp_GhiNhanDangXuat
    @MaNV CHAR(10)
AS
BEGIN
    UPDATE LichSuTruyCap
    SET ThoiGianDangXuat = GETDATE()
    WHERE MaNV = @MaNV AND ThoiGianDangXuat IS NULL
END
GO

CREATE OR ALTER PROC sp_LayLichSuChinhSua @TenBang nvarchar(50) = NULL
AS SELECT L.*, NV.HoTen FROM LichSuChinhSua L LEFT JOIN NhanVien NV ON L.MaNV = NV.MaNV WHERE @TenBang IS NULL OR TenBang = @TenBang ORDER BY ThoiGian DESC
GO
