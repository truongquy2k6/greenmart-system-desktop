USE QuanLyHeThongGreenMart;
GO
CREATE OR ALTER PROC sp_DongBo1Kho @MaKho varchar(10)
AS
BEGIN
    INSERT INTO ChiTietKho (MaKho, MaSP, SoLuong)
    SELECT @MaKho, s.MaSP, 0
    FROM SanPham s
    WHERE NOT EXISTS (
        SELECT 1 FROM ChiTietKho c WHERE c.MaKho = @MaKho AND c.MaSP = s.MaSP
    )
END
