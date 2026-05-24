USE QuanLyHeThongGreenMart;
GO
CREATE OR ALTER PROC sp_DongBoKho
AS
BEGIN
    INSERT INTO ChiTietKho (MaKho, MaSP, SoLuong)
    SELECT k.MaKho, s.MaSP, 0
    FROM Kho k CROSS JOIN SanPham s
    WHERE NOT EXISTS (
        SELECT 1 FROM ChiTietKho c WHERE c.MaKho = k.MaKho AND c.MaSP = s.MaSP
    )
END
