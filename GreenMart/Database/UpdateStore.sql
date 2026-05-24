USE [QuanLyHeThongGreenMart]
GO

CREATE OR ALTER PROCEDURE sp_LocNangCaoSanPham
    @TuKhoa NVARCHAR(100),
    @MaLoai NVARCHAR(50),
    @MaNCC NVARCHAR(50),
    @DonViTinh NVARCHAR(50),
    @TrangThai NVARCHAR(50),
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SELECT sp.*, 
           l.TenLoai, 
           ncc.TenNCC,
           COUNT(*) OVER() AS TotalRows
    FROM SanPham sp
    LEFT JOIN LoaiSanPham l ON sp.MaLoai = l.MaLoai
    LEFT JOIN NhaCungCap ncc ON sp.MaNCC = ncc.MaNCC
    WHERE (@TuKhoa = '' OR @TuKhoa IS NULL OR sp.TenSP LIKE N'%' + @TuKhoa + '%' OR sp.MaSP LIKE N'%' + @TuKhoa + '%' OR l.TenLoai LIKE N'%' + @TuKhoa + '%' OR ncc.TenNCC LIKE N'%' + @TuKhoa + '%')
      AND (@MaLoai = '' OR @MaLoai IS NULL OR sp.MaLoai = @MaLoai)
      AND (@MaNCC = '' OR @MaNCC IS NULL OR sp.MaNCC = @MaNCC)
      AND (@DonViTinh = '' OR @DonViTinh IS NULL OR sp.DonViTinh = @DonViTinh)
      AND (@TrangThai = '' OR @TrangThai IS NULL OR sp.TrangThai = @TrangThai)
    ORDER BY sp.MaSP
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY
END
GO

CREATE OR ALTER PROC sp_HienThiSanPham
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SELECT SP.*, 
           LSP.TenLoai, 
           NCC.TenNCC,
           COUNT(*) OVER() AS TotalRows
    FROM SanPham SP 
    LEFT JOIN NhaCungCap NCC ON SP.MaNCC = NCC.MaNCC 
    LEFT JOIN LoaiSanPham LSP ON SP.MaLoai = LSP.MaLoai
    ORDER BY SP.MaSP
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY
END
GO

CREATE OR ALTER PROC sp_TimKiemLoaiSanPham
    @TuKhoa NVARCHAR(100) = '',
    @PageNumber INT = 1,
    @PageSize INT = 10000
AS
BEGIN
    SELECT *, COUNT(*) OVER() AS TotalRows
    FROM LoaiSanPham
    WHERE @TuKhoa IS NULL OR @TuKhoa = '' 
       OR TenLoai LIKE N'%' + @TuKhoa + '%' 
       OR MaLoai LIKE N'%' + @TuKhoa + '%'
    ORDER BY MaLoai
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY
END
GO

CREATE OR ALTER PROC sp_HienThiLoaiSanPham
    @PageNumber INT = 1,
    @PageSize INT = 10000
AS
BEGIN
    SELECT *, COUNT(*) OVER() AS TotalRows
    FROM LoaiSanPham
    ORDER BY MaLoai
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY
END
GO

CREATE OR ALTER PROC sp_LayThongTinCuaHangTuHoaDon
    @MaHD NVARCHAR(50)
AS
BEGIN
    SELECT ch.MaCH, ch.TenCuaHang, ch.DiaChi, ch.SoDienThoai, ch.Email
    FROM HoaDon hd
    JOIN NhanVien nv ON hd.MaNV = nv.MaNV
    JOIN CuaHang ch ON nv.MaCH = ch.MaCH
    WHERE hd.MaHD = @MaHD
END
GO
