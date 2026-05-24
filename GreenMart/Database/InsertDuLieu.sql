USE QuanLyHeThongGreenMart
GO

-- =============================================
-- BƯỚC 1: XÓA DỮ LIỆU CŨ (THEO THỨ TỰ PHỤ THUỘC)
-- =============================================
DELETE FROM ChiTietHoaDon;
DELETE FROM ChiTietKho;
DELETE FROM ApDungKhuyenMai;
DELETE FROM LichSuTruyCap;
DELETE FROM LichSuChinhSua;
DELETE FROM HoaDon;
DELETE FROM Kho;
DELETE FROM NhanVien;
DELETE FROM SanPham;
DELETE FROM KhachHang;
DELETE FROM CuaHang;
DELETE FROM NhaCungCap;
DELETE FROM LoaiSanPham;
DELETE FROM KhuyenMai;
GO

-- =============================================
-- 1. LOẠI SẢN PHẨM (10 Dòng)
-- =============================================
INSERT INTO LoaiSanPham (MaLoai, TenLoai) VALUES
('L01', N'Thịt tươi sống'),
('L02', N'Hải sản đông lạnh'),
('L03', N'Rau củ quả'),
('L04', N'Trái cây nhập khẩu'),
('L05', N'Sữa và sản phẩm từ sữa'),
('L06', N'Bánh kẹo và đồ ăn vặt'),
('L07', N'Nước giải khát'),
('L08', N'Gia vị và dầu ăn'),
('L09', N'Hóa phẩm vệ sinh'),
('L10', N'Đồ dùng gia đình');

-- =============================================
-- 2. NHÀ CUNG CẤP (10 Dòng)
-- =============================================
INSERT INTO NhaCungCap (MaNCC, TenNCC, DiaChi, SoDienThoai, Email) VALUES
('NCC01', N'Công ty C.P Việt Nam', N'KCN Biên Hòa, Đồng Nai', '02513836251', 'cpvietnam@cp.com.vn'),
('NCC02', N'Vissan Việt Nam', N'420 Nơ Trang Long, Bình Thạnh, TP.HCM', '02838433907', 'vissan@vissan.com.vn'),
('NCC03', N'Vinamilk', N'10 Tân Trào, Quận 7, TP.HCM', '02854155555', 'vinamilk@vinamilk.com.vn'),
('NCC04', N'Acecook Việt Nam', N'KCN Tân Bình, Tân Phú, TP.HCM', '02838154066', 'acecook@acecook.com.vn'),
('NCC05', N'Unilever Việt Nam', N'KCN Tây Bắc Củ Chi, TP.HCM', '02838236622', 'unilever@unilever.com'),
('NCC06', N'Masan Group', N'Quận 1, TP.HCM', '02862563862', 'masan@masan.vn'),
('NCC07', N'Công ty Ba Huân', N'Huyện Bình Chánh, TP.HCM', '02837603417', 'bahuan@bahuan.vn'),
('NCC08', N'Nông sản Đà Lạt', N'Thành phố Đà Lạt, Lâm Đồng', '02633822111', 'dalatfarm@dalat.vn'),
('NCC09', N'Coca-Cola Việt Nam', N'Thủ Đức, TP.HCM', '02838961000', 'coca@coca.com.vn'),
('NCC10', N'TH True Milk', N'Nghệ An', '1800545440', 'th@thmilk.vn');

-- =============================================
-- 3. CỬA HÀNG (10 Dòng)
-- =============================================
INSERT INTO CuaHang (MaCH, TenCH, DiaChi, SoDienThoai, Email) VALUES
('CH01', N'GreenMart Gò Vấp', N'123 Lê Đức Thọ, Gò Vấp, TP.HCM', '02838111001', 'gv@greenmart.vn'),
('CH02', N'GreenMart Quận 12', N'456 Nguyễn Ảnh Thủ, Quận 12, TP.HCM', '02838111002', 'q12@greenmart.vn'),
('CH03', N'GreenMart Tân Bình', N'789 Trường Chinh, Tân Bình, TP.HCM', '02838111003', 'tb@greenmart.vn'),
('CH04', N'GreenMart Thủ Đức', N'101 Võ Văn Ngân, Thủ Đức, TP.HCM', '02838111004', 'td@greenmart.vn'),
('CH05', N'GreenMart Quận 7', N'202 Nguyễn Thị Thập, Quận 7, TP.HCM', '02838111005', 'q7@greenmart.vn'),
('CH06', N'GreenMart Bình Thạnh', N'303 Phan Văn Trị, Bình Thạnh, TP.HCM', '02838111006', 'bt@greenmart.vn'),
('CH07', N'GreenMart Quận 1', N'404 Lê Lợi, Quận 1, TP.HCM', '02838111007', 'q1@greenmart.vn'),
('CH08', N'GreenMart Quận 10', N'505 Ba Tháng Hai, Quận 10, TP.HCM', '02838111008', 'q10@greenmart.vn'),
('CH09', N'GreenMart Hóc Môn', N'606 Lý Nam Đế, Hóc Môn, TP.HCM', '02838111009', 'hm@greenmart.vn'),
('CH10', N'GreenMart Bình Chánh', N'707 Quốc Lộ 50, Bình Chánh, TP.HCM', '02838111010', 'bc@greenmart.vn');

-- =============================================
-- 4. KHÁCH HÀNG (10 Dòng)
-- =============================================
INSERT INTO KhachHang (MaKH, HoTen, SoDienThoai, DiaChi, Email, DiemTichLuy) VALUES
('KH01', N'Nguyễn Văn An', '0901234567', N'Gò Vấp, TP.HCM', 'an.nv@gmail.com', 150),
('KH02', N'Trần Thị Bình', '0912345678', N'Quận 12, TP.HCM', 'binh.tt@gmail.com', 200),
('KH03', N'Lê Hoàng Cường', '0923456789', N'Tân Bình, TP.HCM', 'cuong.lh@gmail.com', 50),
('KH04', N'Phạm Minh Đức', '0934567890', N'Thủ Đức, TP.HCM', 'duc.pm@gmail.com', 0),
('KH05', N'Hoàng Thanh Em', '0945678901', N'Quận 7, TP.HCM', 'em.ht@gmail.com', 1000),
('KH06', N'Đỗ Quốc Huy', '0956789012', N'Bình Thạnh, TP.HCM', 'huy.dq@gmail.com', 75),
('KH07', N'Vũ Kim Lan', '0967890123', N'Quận 1, TP.HCM', 'lan.vk@gmail.com', 300),
('KH08', N'Ngô Gia Minh', '0978901234', N'Quận 10, TP.HCM', 'minh.ng@gmail.com', 120),
('KH09', N'Bùi Tuyết Nhi', '0989012345', N'Hóc Môn, TP.HCM', 'nhi.bt@gmail.com', 20),
('KH10', N'Đặng Quang Phước', '0990123456', N'Bình Chánh, TP.HCM', 'phuoc.dq@gmail.com', 450);

-- =============================================
-- 5. KHUYẾN MÃI (10 Dòng)
-- =============================================
INSERT INTO KhuyenMai (MaKM, TenKM, MoTa, LoaiKM, GiaTri, NgayBatDau, NgayKetThuc, DieuKien, TrangThai) VALUES
('KM01', N'Chào hè rực rỡ', N'Giảm 10% tổng hóa đơn', N'Giảm theo %', 10, '2026-05-01', '2026-05-31', 200000, N'Đang áp dụng'),
('KM02', N'Tri ân khách hàng', N'Giảm 50k cho đơn từ 500k', N'Giảm theo số tiền', 50000, '2026-05-01', '2026-12-31', 500000, N'Đang áp dụng'),
('KM03', N'Giờ vàng giá sốc', N'Giảm 20% mặt hàng rau củ', N'Giảm theo %', 20, '2026-05-05', '2026-05-10', 0, N'Chưa bắt đầu'),
('KM04', N'Cuối tuần vui vẻ', N'Giảm 30k cho đơn từ 300k', N'Giảm theo số tiền', 30000, '2026-05-09', '2026-05-11', 300000, N'Chưa bắt đầu'),
('KM05', N'Mừng khai trương', N'Giảm 15% tổng hóa đơn', N'Giảm theo %', 15, '2026-04-01', '2026-04-30', 100000, N'Đã kết thúc'),
('KM06', N'Combo tiết kiệm', N'Giảm 5% cho hóa đơn sữa', N'Giảm theo %', 5, '2026-05-01', '2026-06-30', 200000, N'Đang áp dụng'),
('KM07', N'Siêu sale 6.6', N'Giảm 66k cho đơn từ 666k', N'Giảm theo số tiền', 66000, '2026-06-06', '2026-06-06', 666000, N'Chưa bắt đầu'),
('KM08', N'Quà tặng mẹ', N'Giảm 10% đồ dùng gia đình', N'Giảm theo %', 10, '2026-05-10', '2026-05-12', 150000, N'Chưa bắt đầu'),
('KM09', N'Hội viên bạc', N'Giảm 2% đơn hàng', N'Giảm theo %', 2, '2026-01-01', '2026-12-31', 0, N'Đang áp dụng'),
('KM10', N'Hội viên vàng', N'Giảm 5% đơn hàng', N'Giảm theo %', 5, '2026-01-01', '2026-12-31', 0, N'Đang áp dụng');

-- =============================================
-- 6. SẢN PHẨM (10 Dòng)
-- =============================================
INSERT INTO SanPham (MaSP, TenSP, DonGia, DonViTinh, HinhAnh, MoTa, MaNCC, MaLoai) VALUES
('SP01', N'Thịt ba chỉ heo', 150000, N'Kg', 'bachi.jpg', N'Thịt tươi trong ngày', 'NCC01', 'L01'),
('SP02', N'Tôm thẻ chân trắng', 220000, N'Kg', 'tomthe.jpg', N'Tôm tươi đông lạnh', 'NCC02', 'L02'),
('SP03', N'Rau muống sạch', 15000, N'Bó', 'raumuong.jpg', N'Rau VietGAP', 'NCC08', 'L03'),
('SP04', N'Táo Envy Mỹ', 120000, N'Kg', 'taoenvy.jpg', N'Giòn, ngọt, thơm', 'NCC08', 'L04'),
('SP05', N'Sữa tươi TH True Milk', 35000, N'Lốc', 'thmilk.jpg', N'Lốc 4 hộp 180ml', 'NCC10', 'L05'),
('SP06', N'Bánh quy OREO', 18000, N'Gói', 'oreo.jpg', N'Vị socola kẹp kem', 'NCC04', 'L06'),
('SP07', N'Coca-Cola 1.5L', 20000, N'Chai', 'coca1.5.jpg', N'Nước ngọt có gas', 'NCC09', 'L07'),
('SP08', N'Nước mắm Nam Ngư', 45000, N'Chai', 'namngu.jpg', N'Chai 750ml', 'NCC06', 'L08'),
('SP09', N'Nước giặt OMO Matic', 185000, N'Túi', 'omo.jpg', N'Túi 3.5kg', 'NCC05', 'L09'),
('SP10', N'Chảo chống dính Sunhouse', 250000, N'Cái', 'chao.jpg', N'Đường kính 26cm', 'NCC05', 'L10');

-- =============================================
-- 7. NHÂN VIÊN (10 Dòng)
-- Mật khẩu mặc định là '123' (đã hash SHA256)
-- =============================================
INSERT INTO NhanVien (MaNV, HoTen, ChucVu, NgaySinh, GioiTinh, SoDienThoai, DiaChi, TenDangNhap, MatKhau, MaCH) VALUES
('NV01', N'Trần Quản Lý', N'Quản lý', '1990-01-01', N'Nam', '0901000001', N'Gò Vấp', 'admin', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'CH01'),
('NV02', N'Nguyễn Thu Ngân', N'Nhân viên bán hàng', '1995-05-15', N'Nữ', '0901000002', N'Quận 12', 'cashier1', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'CH01'),
('NV03', N'Lê Văn Kho', N'Nhân viên kho', '1992-10-20', N'Nam', '0901000003', N'Tân Bình', 'warehouse1', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'CH01'),
('NV04', N'Phạm Thị Bán', N'Nhân viên bán hàng', '1998-03-12', N'Nữ', '0901000004', N'Thủ Đức', 'sales1', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'CH02'),
('NV05', N'Hoàng Văn Quản', N'Quản lý', '1988-12-30', N'Nam', '0901000005', N'Quận 7', 'manager2', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'CH02'),
('NV06', N'Đỗ Thu Thảo', N'Nhân viên bán hàng', '1996-07-07', N'Nữ', '0901000006', N'Bình Thạnh', 'cashier2', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'CH03'),
('NV07', N'Vũ Minh Tuấn', N'Nhân viên kho', '1994-08-08', N'Nam', '0901000007', N'Quận 1', 'warehouse2', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'CH03'),
('NV08', N'Ngô Thị Lan', N'Nhân viên bán hàng', '1999-09-09', N'Nữ', '0901000008', N'Quận 10', 'sales2', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'CH04'),
('NV09', N'Bùi Văn Tùng', N'Nhân viên bán hàng', '1993-11-11', N'Nam', '0901000009', N'Hóc Môn', 'sales3', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'CH05'),
('NV10', N'Đặng Thị Kim', N'Nhân viên bán hàng', '1997-02-28', N'Nữ', '0901000010', N'Bình Chánh', 'cashier3', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'CH06');

-- =============================================
-- 8. KHO (10 Dòng)
-- =============================================
INSERT INTO Kho (MaKho, TenKho, DiaChi, SoDienThoai, MaCH) VALUES
('K01', N'Kho Gò Vấp', N'123 Lê Đức Thọ, Gò Vấp', '0902000001', 'CH01'),
('K02', N'Kho Quận 12', N'456 Nguyễn Ảnh Thủ, Q12', '0902000002', 'CH02'),
('K03', N'Kho Tân Bình', N'789 Trường Chinh, Tân Bình', '0902000003', 'CH03'),
('K04', N'Kho Thủ Đức', N'101 Võ Văn Ngân, Thủ Đức', '0902000004', 'CH04'),
('K05', N'Kho Quận 7', N'202 Nguyễn Thị Thập, Q7', '0902000005', 'CH05'),
('K06', N'Kho Bình Thạnh', N'303 Phan Văn Trị, Bình Thạnh', '0902000006', 'CH06'),
('K07', N'Kho Quận 1', N'404 Lê Lợi, Q1', '0902000007', 'CH07'),
('K08', N'Kho Quận 10', N'505 Ba Tháng Hai, Q10', '0902000008', 'CH08'),
('K09', N'Kho Hóc Môn', N'606 Lý Nam Đế, Hóc Môn', '0902000009', 'CH09'),
('K10', N'Kho Bình Chánh', N'707 Quốc Lộ 50, Bình Chánh', '0902000010', 'CH10');

-- =============================================
-- 9. HÓA ĐƠN (10 Dòng)
-- =============================================
INSERT INTO HoaDon (MaHD, NgayLap, TongTien, MaKH, MaNV, MaCH, MaKM, GiamGia, PhuongThucThanhToan, TrangThai) VALUES
('HD01', '2026-05-01 08:30:00', 300000, 'KH01', 'NV02', 'CH01', 'KM01', 30000, N'Tiền mặt', N'Đã thanh toán'),
('HD02', '2026-05-01 10:15:00', 150000, 'KH02', 'NV02', 'CH01', NULL, 0, N'Thẻ', N'Đã thanh toán'),
('HD03', '2026-05-02 09:00:00', 500000, 'KH03', 'NV04', 'CH02', 'KM02', 50000, N'Chuyển khoản', N'Đã thanh toán'),
('HD04', '2026-05-02 14:45:00', 220000, 'KH04', 'NV04', 'CH02', NULL, 0, N'Tiền mặt', N'Đã thanh toán'),
('HD05', '2026-05-03 11:20:00', 750000, 'KH05', 'NV06', 'CH03', 'KM02', 50000, N'Thẻ', N'Đã thanh toán'),
('HD06', '2026-05-03 16:30:00', 120000, 'KH06', 'NV06', 'CH03', NULL, 0, N'Tiền mặt', N'Đã thanh toán'),
('HD07', '2026-05-04 10:00:00', 1000000, 'KH07', 'NV08', 'CH04', 'KM02', 50000, N'Chuyển khoản', N'Đã thanh toán'),
('HD08', '2026-05-04 13:10:00', 45000, 'KH08', 'NV08', 'CH04', NULL, 0, N'Tiền mặt', N'Đã thanh toán'),
('HD09', '2026-05-05 15:00:00', 850000, 'KH09', 'NV09', 'CH05', 'KM02', 50000, N'Thẻ', N'Đã thanh toán'),
('HD10', '2026-05-05 18:20:00', 300000, 'KH10', 'NV09', 'CH05', 'KM01', 30000, N'Tiền mặt', N'Đã thanh toán');

-- =============================================
-- 10. CHI TIẾT HÓA ĐƠN (10 Dòng)
-- =============================================
INSERT INTO ChiTietHoaDon (MaHD, MaSP, SoLuong, DonGia, ThanhTien) VALUES
('HD01', 'SP01', 2, 150000, 300000),
('HD02', 'SP05', 3, 35000, 105000),
('HD02', 'SP03', 3, 15000, 45000),
('HD03', 'SP02', 2, 220000, 440000),
('HD03', 'SP06', 3, 18000, 54000),
('HD04', 'SP02', 1, 220000, 220000),
('HD05', 'SP09', 4, 185000, 740000),
('HD05', 'SP03', 1, 15000, 15000),
('HD06', 'SP04', 1, 120000, 120000),
('HD07', 'SP10', 4, 250000, 1000000);

-- =============================================
-- 11. CHI TIẾT KHO (10 Dòng)
-- =============================================
INSERT INTO ChiTietKho (MaKho, MaSP, SoLuong) VALUES
('K01', 'SP01', 100),
('K02', 'SP02', 50),
('K03', 'SP03', 200),
('K04', 'SP04', 80),
('K05', 'SP05', 300),
('K06', 'SP06', 500),
('K07', 'SP07', 400),
('K08', 'SP08', 150),
('K09', 'SP09', 60),
('K10', 'SP10', 40);

-- =============================================
-- 12. ÁP DỤNG KHUYẾN MÃI (10 Dòng)
-- =============================================
INSERT INTO ApDungKhuyenMai (MaKM, MaSP, MaLoai) VALUES
('KM01', NULL, NULL),
('KM02', NULL, NULL),
('KM03', NULL, 'L03'),
('KM04', NULL, NULL),
('KM06', NULL, 'L05'),
('KM08', NULL, 'L10'),
('KM09', NULL, NULL),
('KM10', NULL, NULL),
('KM02', 'SP09', NULL),
('KM01', 'SP01', NULL);

-- =============================================
-- 13. LỊCH SỬ TRUY CẬP (10 Dòng)
-- =============================================
INSERT INTO LichSuTruyCap (MaNV, ThoiGianDangNhap, ThoiGianDangXuat, DiaChiIP, ThietBi) VALUES
('NV01', '2026-05-01 08:00:00', '2026-05-01 17:00:00', '192.168.1.10', N'PC-Admin'),
('NV02', '2026-05-01 08:05:00', '2026-05-01 16:30:00', '192.168.1.11', N'POS-01'),
('NV03', '2026-05-01 08:10:00', '2026-05-01 17:15:00', '192.168.1.12', N'Warehouse-Tablet'),
('NV04', '2026-05-02 08:00:00', '2026-05-02 17:00:00', '192.168.2.10', N'POS-02'),
('NV05', '2026-05-02 08:30:00', '2026-05-02 18:00:00', '192.168.2.11', N'Laptop-Manager'),
('NV06', '2026-05-03 08:00:00', '2026-05-03 17:00:00', '192.168.3.10', N'POS-03'),
('NV07', '2026-05-03 08:15:00', '2026-05-03 17:30:00', '192.168.3.11', N'Handheld-Scanner'),
('NV08', '2026-05-04 08:00:00', '2026-05-04 17:00:00', '192.168.4.10', N'POS-04'),
('NV09', '2026-05-05 08:00:00', '2026-05-05 17:00:00', '192.168.5.10', N'POS-05'),
('NV10', '2026-05-05 13:00:00', '2026-05-05 21:00:00', '192.168.6.10', N'POS-06');

-- =============================================
-- 14. LỊCH SỬ CHỈNH SỬA (10 Dòng)
-- =============================================
INSERT INTO LichSuChinhSua (TenBang, MaBanGhi, HanhDong, NoiDungCu, NoiDungMoi, MaNV, ThoiGian) VALUES
('SanPham', 'SP01', 'UPDATE', N'Gia: 145000', N'Gia: 150000', 'NV01', GETDATE()),
('NhanVien', 'NV02', 'UPDATE', N'DiaChi: Cu Chi', N'DiaChi: Quan 12', 'NV01', GETDATE()),
('KhachHang', 'KH01', 'UPDATE', N'Diem: 140', N'Diem: 150', 'NV02', GETDATE()),
('SanPham', 'SP03', 'UPDATE', N'Gia: 14000', N'Gia: 15000', 'NV05', GETDATE()),
('Kho', 'K01', 'UPDATE', N'SDT: 0901...', N'SDT: 0902...', 'NV01', GETDATE()),
('KhuyenMai', 'KM01', 'UPDATE', N'GiaTri: 5', N'GiaTri: 10', 'NV01', GETDATE()),
('CuaHang', 'CH01', 'UPDATE', N'Ten: BHX GV', N'Ten: GreenMart GV', 'NV01', GETDATE()),
('SanPham', 'SP09', 'UPDATE', N'Gia: 180000', N'Gia: 185000', 'NV05', GETDATE()),
('NhaCungCap', 'NCC01', 'UPDATE', N'Email: cp@cp.com', N'Email: cpvietnam@cp.com.vn', 'NV01', GETDATE()),
('LoaiSanPham', 'L01', 'UPDATE', N'Ten: Thit', N'Ten: Thit tuoi song', 'NV01', GETDATE());
GO
