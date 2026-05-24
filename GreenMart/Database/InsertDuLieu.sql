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
DELETE FROM CauHinh;
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
-- 1. LOẠI SẢN PHẨM (20 Dòng)
-- =============================================
INSERT INTO LoaiSanPham (MaLoai, TenLoai) VALUES
('L01', N'Thịt tươi sống'), ('L02', N'Hải sản đông lạnh'), ('L03', N'Rau củ quả'), ('L04', N'Trái cây nhập khẩu'), ('L05', N'Sữa và sản phẩm từ sữa'),
('L06', N'Bánh kẹo và đồ ăn vặt'), ('L07', N'Nước giải khát'), ('L08', N'Gia vị và dầu ăn'), ('L09', N'Hóa phẩm vệ sinh'), ('L10', N'Đồ dùng gia đình'),
('L11', N'Thực phẩm đóng hộp'), ('L12', N'Thực phẩm khô'), ('L13', N'Bỉm tã, đồ dùng em bé'), ('L14', N'Mỹ phẩm chăm sóc cá nhân'), ('L15', N'Thực phẩm chay'),
('L16', N'Đồ uống có cồn'), ('L17', N'Gạo, các loại hạt'), ('L18', N'Mì, miến, phở khô'), ('L19', N'Thực phẩm chức năng'), ('L20', N'Văn phòng phẩm');

-- =============================================
-- 2. NHÀ CUNG CẤP (20 Dòng)
-- =============================================
INSERT INTO NhaCungCap (MaNCC, TenNCC, DiaChi, SoDienThoai, Email) VALUES
('NCC01', N'Công ty C.P Việt Nam', N'Đồng Nai', '02513836251', 'cpvietnam@cp.com.vn'),
('NCC02', N'Vissan Việt Nam', N'TP.HCM', '02838433907', 'vissan@vissan.com.vn'),
('NCC03', N'Vinamilk', N'TP.HCM', '02854155555', 'vinamilk@vinamilk.com.vn'),
('NCC04', N'Acecook Việt Nam', N'TP.HCM', '02838154066', 'acecook@acecook.com.vn'),
('NCC05', N'Unilever Việt Nam', N'TP.HCM', '02838236622', 'unilever@unilever.com'),
('NCC06', N'Masan Group', N'TP.HCM', '02862563862', 'masan@masan.vn'),
('NCC07', N'Công ty Ba Huân', N'TP.HCM', '02837603417', 'bahuan@bahuan.vn'),
('NCC08', N'Nông sản Đà Lạt', N'Lâm Đồng', '02633822111', 'dalatfarm@dalat.vn'),
('NCC09', N'Coca-Cola Việt Nam', N'TP.HCM', '02838961000', 'coca@coca.com.vn'),
('NCC10', N'TH True Milk', N'Nghệ An', '1800545440', 'th@thmilk.vn'),
('NCC11', N'Công ty Vifon', N'TP.HCM', '02838153400', 'vifon@vifon.com.vn'),
('NCC12', N'Tập đoàn KIDO', N'TP.HCM', '02838270468', 'kido@kido.vn'),
('NCC13', N'Suntory PepsiCo', N'Bình Dương', '02743756200', 'pepsi@pepsico.com.vn'),
('NCC14', N'Nestlé Việt Nam', N'Đồng Nai', '02513936666', 'nestle@nestle.com.vn'),
('NCC15', N'Công ty CP Tường An', N'TP.HCM', '02838446210', 'tuongan@tuongan.vn'),
('NCC16', N'Kao Việt Nam', N'Đồng Nai', '02513836100', 'kao@kao.vn'),
('NCC17', N'Lotte Việt Nam', N'Bình Dương', '02743743500', 'lotte@lotte.vn'),
('NCC18', N'Nutifood', N'Bình Dương', '02743756300', 'nutifood@nutifood.vn'),
('NCC19', N'Cholimex', N'TP.HCM', '02838562100', 'cholimex@cholimex.vn'),
('NCC20', N'Orion Vina', N'Bình Dương', '02743754800', 'orion@orion.vn');

-- =============================================
-- 3. CỬA HÀNG (20 Dòng)
-- =============================================
INSERT INTO CuaHang (MaCH, TenCH, DiaChi, SoDienThoai, Email) VALUES
('CH01', N'GreenMart Gò Vấp', N'Gò Vấp, TP.HCM', '02838111001', 'gv@greenmart.vn'),
('CH02', N'GreenMart Quận 12', N'Quận 12, TP.HCM', '02838111002', 'q12@greenmart.vn'),
('CH03', N'GreenMart Tân Bình', N'Tân Bình, TP.HCM', '02838111003', 'tb@greenmart.vn'),
('CH04', N'GreenMart Thủ Đức', N'Thủ Đức, TP.HCM', '02838111004', 'td@greenmart.vn'),
('CH05', N'GreenMart Quận 7', N'Quận 7, TP.HCM', '02838111005', 'q7@greenmart.vn'),
('CH06', N'GreenMart Bình Thạnh', N'Bình Thạnh, TP.HCM', '02838111006', 'bt@greenmart.vn'),
('CH07', N'GreenMart Quận 1', N'Quận 1, TP.HCM', '02838111007', 'q1@greenmart.vn'),
('CH08', N'GreenMart Quận 10', N'Quận 10, TP.HCM', '02838111008', 'q10@greenmart.vn'),
('CH09', N'GreenMart Hóc Môn', N'Hóc Môn, TP.HCM', '02838111009', 'hm@greenmart.vn'),
('CH10', N'GreenMart Bình Chánh', N'Bình Chánh, TP.HCM', '02838111010', 'bc@greenmart.vn'),
('CH11', N'GreenMart Quận 3', N'Quận 3, TP.HCM', '02838111011', 'q3@greenmart.vn'),
('CH12', N'GreenMart Quận 4', N'Quận 4, TP.HCM', '02838111012', 'q4@greenmart.vn'),
('CH13', N'GreenMart Quận 5', N'Quận 5, TP.HCM', '02838111013', 'q5@greenmart.vn'),
('CH14', N'GreenMart Quận 6', N'Quận 6, TP.HCM', '02838111014', 'q6@greenmart.vn'),
('CH15', N'GreenMart Quận 8', N'Quận 8, TP.HCM', '02838111015', 'q8@greenmart.vn'),
('CH16', N'GreenMart Phú Nhuận', N'Phú Nhuận, TP.HCM', '02838111016', 'pn@greenmart.vn'),
('CH17', N'GreenMart Tân Phú', N'Tân Phú, TP.HCM', '02838111017', 'tp@greenmart.vn'),
('CH18', N'GreenMart Củ Chi', N'Củ Chi, TP.HCM', '02838111018', 'cc@greenmart.vn'),
('CH19', N'GreenMart Nhà Bè', N'Nhà Bè, TP.HCM', '02838111019', 'nb@greenmart.vn'),
('CH20', N'GreenMart Quận 11', N'Quận 11, TP.HCM', '02838111020', 'q11@greenmart.vn');

-- =============================================
-- CẤU HÌNH NGÂN HÀNG (20 Dòng)
-- =============================================
INSERT INTO CauHinh (MaCH, BankId, AccountNo, AccountName) VALUES
('CH01', 'VCB', '1012345678', 'GREENMART GO VAP'),
('CH02', 'TCB', '1903345678', 'GREENMART Q12'),
('CH03', 'MB', '9704229200', 'GREENMART TAN BINH'),
('CH04', 'VPB', '150123456', 'GREENMART THU DUC'),
('CH05', 'ACB', '223456789', 'GREENMART Q7'),
('CH06', 'BIDV', '3101000123', 'GREENMART BINH THANH'),
('CH07', 'VCB', '1012345679', 'GREENMART Q1'),
('CH08', 'TCB', '1903345679', 'GREENMART Q10'),
('CH09', 'MB', '9704229201', 'GREENMART HOC MON'),
('CH10', 'VPB', '150123457', 'GREENMART BINH CHANH'),
('CH11', 'ACB', '223456790', 'GREENMART Q3'),
('CH12', 'BIDV', '3101000124', 'GREENMART Q4'),
('CH13', 'VCB', '1012345680', 'GREENMART Q5'),
('CH14', 'TCB', '1903345680', 'GREENMART Q6'),
('CH15', 'MB', '9704229202', 'GREENMART Q8'),
('CH16', 'VPB', '150123458', 'GREENMART PHU NHUAN'),
('CH17', 'ACB', '223456791', 'GREENMART TAN PHU'),
('CH18', 'BIDV', '3101000125', 'GREENMART CU CHI'),
('CH19', 'VCB', '1012345681', 'GREENMART NHA BE'),
('CH20', 'TCB', '1903345681', 'GREENMART Q11');

-- =============================================
-- 4. KHÁCH HÀNG (20 Dòng)
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
('KH10', N'Đặng Quang Phước', '0990123456', N'Bình Chánh, TP.HCM', 'phuoc.dq@gmail.com', 450),
('KH11', N'Lý Công Uẩn', '0911223344', N'Quận 3, TP.HCM', 'uan.lc@gmail.com', 60),
('KH12', N'Mạc Đĩnh Chi', '0922334455', N'Quận 4, TP.HCM', 'chi.md@gmail.com', 80),
('KH13', N'Trương Định', '0933445566', N'Quận 5, TP.HCM', 'dinh.t@gmail.com', 250),
('KH14', N'Hà Huy Tập', '0944556677', N'Quận 6, TP.HCM', 'tap.hh@gmail.com', 10),
('KH15', N'Lê Duẩn', '0955667788', N'Quận 8, TP.HCM', 'duan.l@gmail.com', 35),
('KH16', N'Phan Bội Châu', '0966778899', N'Phú Nhuận, TP.HCM', 'chau.pb@gmail.com', 95),
('KH17', N'Phan Châu Trinh', '0977889900', N'Tân Phú, TP.HCM', 'trinh.pc@gmail.com', 400),
('KH18', N'Lê Quý Đôn', '0988990011', N'Củ Chi, TP.HCM', 'don.lq@gmail.com', 110),
('KH19', N'Nguyễn Trãi', '0999001122', N'Nhà Bè, TP.HCM', 'trai.n@gmail.com', 205),
('KH20', N'Ngô Quyền', '0900112233', N'Quận 11, TP.HCM', 'quyen.n@gmail.com', 500);

-- =============================================
-- 5. KHUYẾN MÃI (20 Dòng)
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
('KM10', N'Hội viên vàng', N'Giảm 5% đơn hàng', N'Giảm theo %', 5, '2026-01-01', '2026-12-31', 0, N'Đang áp dụng'),
('KM11', N'Ngày hội gia đình', N'Giảm 20k cho hóa đơn từ 200k', N'Giảm theo số tiền', 20000, '2026-06-28', '2026-06-28', 200000, N'Chưa bắt đầu'),
('KM12', N'Tuần lễ hải sản', N'Giảm 10% hải sản', N'Giảm theo %', 10, '2026-07-01', '2026-07-07', 0, N'Chưa bắt đầu'),
('KM13', N'Sinh nhật siêu thị', N'Giảm 50% mặt hàng nhất định', N'Giảm theo %', 50, '2026-08-01', '2026-08-05', 0, N'Chưa bắt đầu'),
('KM14', N'Lễ Vu Lan', N'Giảm 15% đồ chay', N'Giảm theo %', 15, '2026-08-15', '2026-08-30', 0, N'Chưa bắt đầu'),
('KM15', N'Tết Trung Thu', N'Giảm 20k khi mua bánh kẹo', N'Giảm theo số tiền', 20000, '2026-09-10', '2026-09-15', 100000, N'Chưa bắt đầu'),
('KM16', N'Black Friday', N'Giảm 30% toàn cửa hàng', N'Giảm theo %', 30, '2026-11-25', '2026-11-27', 0, N'Chưa bắt đầu'),
('KM17', N'Giáng sinh an lành', N'Giảm 100k cho đơn từ 1 triệu', N'Giảm theo số tiền', 100000, '2026-12-20', '2026-12-25', 1000000, N'Chưa bắt đầu'),
('KM18', N'Đón Tết dương lịch', N'Giảm 5% mọi hóa đơn', N'Giảm theo %', 5, '2026-12-31', '2027-01-02', 0, N'Chưa bắt đầu'),
('KM19', N'Tết Nguyên Đán', N'Tặng quà giá trị 50k', N'Giảm theo số tiền', 50000, '2027-01-20', '2027-01-30', 500000, N'Chưa bắt đầu'),
('KM20', N'Lễ tình nhân', N'Giảm 14% cho sô cô la', N'Giảm theo %', 14, '2027-02-13', '2027-02-15', 0, N'Chưa bắt đầu');

-- =============================================
-- 6. SẢN PHẨM (20 Dòng)
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
('SP10', N'Chảo chống dính Sunhouse', 250000, N'Cái', 'chao.jpg', N'Đường kính 26cm', 'NCC05', 'L10'),
('SP11', N'Cá hồi Na Uy', 400000, N'Kg', 'cahoi.jpg', N'Cá tươi fillet', 'NCC02', 'L02'),
('SP12', N'Xúc xích Vissan', 45000, N'Gói', 'xucxich.jpg', N'500g', 'NCC02', 'L01'),
('SP13', N'Sữa đặc Ông Thọ', 25000, N'Lon', 'ongtho.jpg', N'Lon 380g', 'NCC03', 'L05'),
('SP14', N'Mì Hảo Hảo chua cay', 110000, N'Thùng', 'haohao.jpg', N'Thùng 30 gói', 'NCC04', 'L18'),
('SP15', N'Trứng gà Ba Huân', 32000, N'Vỉ', 'trungga.jpg', N'Vỉ 10 quả', 'NCC07', 'L01'),
('SP16', N'Cà chua Đà Lạt', 25000, N'Kg', 'cachua.jpg', N'Chín đỏ, sạch', 'NCC08', 'L03'),
('SP17', N'Cam sành', 30000, N'Kg', 'camsanh.jpg', N'Ngọt, nhiều nước', 'NCC08', 'L04'),
('SP18', N'Dầu ăn Tường An', 55000, N'Chai', 'dauan.jpg', N'Chai 1 lít', 'NCC15', 'L08'),
('SP19', N'Sữa tắm Lifebuoy', 120000, N'Chai', 'lifebuoy.jpg', N'Bảo vệ khỏi vi khuẩn', 'NCC05', 'L14'),
('SP20', N'Bột giặt Aba', 150000, N'Túi', 'aba.jpg', N'Túi 3kg', 'NCC05', 'L09');

-- =============================================
-- 7. NHÂN VIÊN (20 Dòng)
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
('NV10', N'Đặng Thị Kim', N'Nhân viên bán hàng', '1997-02-28', N'Nữ', '0901000010', N'Bình Chánh', 'cashier3', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'CH06'),
('NV11', N'Trần Đại Nghĩa', N'Quản lý', '1985-04-12', N'Nam', '0901000011', N'Quận 3', 'manager3', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'CH11'),
('NV12', N'Lê Hồng Phong', N'Nhân viên bán hàng', '1991-06-18', N'Nam', '0901000012', N'Quận 4', 'sales4', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'CH12'),
('NV13', N'Võ Thị Sáu', N'Nhân viên kho', '1998-10-10', N'Nữ', '0901000013', N'Quận 5', 'warehouse3', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'CH13'),
('NV14', N'Nguyễn Thái Học', N'Nhân viên bán hàng', '1990-12-12', N'Nam', '0901000014', N'Quận 6', 'sales5', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'CH14'),
('NV15', N'Phạm Ngọc Thạch', N'Quản lý', '1982-01-20', N'Nam', '0901000015', N'Quận 8', 'manager4', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'CH15'),
('NV16', N'Trần Hưng Đạo', N'Nhân viên bán hàng', '1992-02-22', N'Nam', '0901000016', N'Phú Nhuận', 'sales6', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'CH16'),
('NV17', N'Lê Lợi', N'Nhân viên kho', '1995-03-30', N'Nam', '0901000017', N'Tân Phú', 'warehouse4', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'CH17'),
('NV18', N'Nguyễn Huệ', N'Nhân viên bán hàng', '1996-04-14', N'Nam', '0901000018', N'Củ Chi', 'sales7', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'CH18'),
('NV19', N'Lý Thường Kiệt', N'Nhân viên bán hàng', '1997-05-15', N'Nam', '0901000019', N'Nhà Bè', 'sales8', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'CH19'),
('NV20', N'Hai Bà Trưng', N'Quản lý', '1989-06-16', N'Nữ', '0901000020', N'Quận 11', 'manager5', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'CH20');

-- =============================================
-- 8. KHO (20 Dòng)
-- =============================================
INSERT INTO Kho (MaKho, TenKho, DiaChi, SoDienThoai, MaCH) VALUES
('K01', N'Kho Gò Vấp', N'Gò Vấp', '0902000001', 'CH01'),
('K02', N'Kho Quận 12', N'Q12', '0902000002', 'CH02'),
('K03', N'Kho Tân Bình', N'Tân Bình', '0902000003', 'CH03'),
('K04', N'Kho Thủ Đức', N'Thủ Đức', '0902000004', 'CH04'),
('K05', N'Kho Quận 7', N'Q7', '0902000005', 'CH05'),
('K06', N'Kho Bình Thạnh', N'Bình Thạnh', '0902000006', 'CH06'),
('K07', N'Kho Quận 1', N'Q1', '0902000007', 'CH07'),
('K08', N'Kho Quận 10', N'Q10', '0902000008', 'CH08'),
('K09', N'Kho Hóc Môn', N'Hóc Môn', '0902000009', 'CH09'),
('K10', N'Kho Bình Chánh', N'Bình Chánh', '0902000010', 'CH10'),
('K11', N'Kho Quận 3', N'Q3', '0902000011', 'CH11'),
('K12', N'Kho Quận 4', N'Q4', '0902000012', 'CH12'),
('K13', N'Kho Quận 5', N'Q5', '0902000013', 'CH13'),
('K14', N'Kho Quận 6', N'Q6', '0902000014', 'CH14'),
('K15', N'Kho Quận 8', N'Q8', '0902000015', 'CH15'),
('K16', N'Kho Phú Nhuận', N'Phú Nhuận', '0902000016', 'CH16'),
('K17', N'Kho Tân Phú', N'Tân Phú', '0902000017', 'CH17'),
('K18', N'Kho Củ Chi', N'Củ Chi', '0902000018', 'CH18'),
('K19', N'Kho Nhà Bè', N'Nhà Bè', '0902000019', 'CH19'),
('K20', N'Kho Quận 11', N'Q11', '0902000020', 'CH20');

-- =============================================
-- 9. HÓA ĐƠN (20 Dòng)
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
('HD10', '2026-05-05 18:20:00', 300000, 'KH10', 'NV09', 'CH05', 'KM01', 30000, N'Tiền mặt', N'Đã thanh toán'),
('HD11', '2026-05-06 09:00:00', 200000, 'KH11', 'NV12', 'CH12', NULL, 0, N'Tiền mặt', N'Đã thanh toán'),
('HD12', '2026-05-06 11:00:00', 600000, 'KH12', 'NV12', 'CH12', 'KM02', 50000, N'Thẻ', N'Đã thanh toán'),
('HD13', '2026-05-07 14:00:00', 150000, 'KH13', 'NV14', 'CH14', NULL, 0, N'Chuyển khoản', N'Đã thanh toán'),
('HD14', '2026-05-07 16:00:00', 400000, 'KH14', 'NV14', 'CH14', 'KM01', 40000, N'Tiền mặt', N'Đã thanh toán'),
('HD15', '2026-05-08 10:30:00', 250000, 'KH15', 'NV16', 'CH16', NULL, 0, N'Thẻ', N'Đã thanh toán'),
('HD16', '2026-05-08 13:45:00', 800000, 'KH16', 'NV16', 'CH16', 'KM02', 50000, N'Chuyển khoản', N'Đã thanh toán'),
('HD17', '2026-05-09 08:15:00', 100000, 'KH17', 'NV18', 'CH18', NULL, 0, N'Tiền mặt', N'Đã thanh toán'),
('HD18', '2026-05-09 12:20:00', 550000, 'KH18', 'NV18', 'CH18', 'KM02', 50000, N'Thẻ', N'Đã thanh toán'),
('HD19', '2026-05-10 15:30:00', 950000, 'KH19', 'NV19', 'CH19', 'KM02', 50000, N'Chuyển khoản', N'Đã thanh toán'),
('HD20', '2026-05-10 19:40:00', 350000, 'KH20', 'NV19', 'CH19', 'KM01', 35000, N'Tiền mặt', N'Đã thanh toán');

-- =============================================
-- 10. CHI TIẾT HÓA ĐƠN (20 Dòng)
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
('HD07', 'SP10', 4, 250000, 1000000),
('HD11', 'SP11', 1, 400000, 400000),
('HD12', 'SP12', 2, 45000, 90000),
('HD13', 'SP13', 4, 25000, 100000),
('HD14', 'SP14', 1, 110000, 110000),
('HD15', 'SP15', 5, 32000, 160000),
('HD16', 'SP16', 3, 25000, 75000),
('HD17', 'SP17', 2, 30000, 60000),
('HD18', 'SP18', 4, 55000, 220000),
('HD19', 'SP19', 2, 120000, 240000),
('HD20', 'SP20', 1, 150000, 150000);

-- =============================================
-- 11. CHI TIẾT KHO (20 Dòng)
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
('K10', 'SP10', 40),
('K11', 'SP11', 100),
('K12', 'SP12', 150),
('K13', 'SP13', 200),
('K14', 'SP14', 250),
('K15', 'SP15', 300),
('K16', 'SP16', 350),
('K17', 'SP17', 400),
('K18', 'SP18', 450),
('K19', 'SP19', 500),
('K20', 'SP20', 550);

-- =============================================
-- 12. ÁP DỤNG KHUYẾN MÃI (20 Dòng)
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
('KM01', 'SP01', NULL),
('KM11', NULL, NULL),
('KM12', NULL, 'L02'),
('KM13', 'SP11', NULL),
('KM14', NULL, 'L15'),
('KM15', NULL, 'L06'),
('KM16', NULL, NULL),
('KM17', NULL, NULL),
('KM18', NULL, NULL),
('KM19', NULL, NULL),
('KM20', 'SP06', NULL);

-- =============================================
-- 13. LỊCH SỬ TRUY CẬP (20 Dòng)
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
('NV10', '2026-05-05 13:00:00', '2026-05-05 21:00:00', '192.168.6.10', N'POS-06'),
('NV11', '2026-05-06 08:00:00', '2026-05-06 17:00:00', '192.168.7.10', N'Laptop-Manager2'),
('NV12', '2026-05-06 08:15:00', '2026-05-06 17:30:00', '192.168.7.11', N'POS-07'),
('NV13', '2026-05-07 08:00:00', '2026-05-07 17:00:00', '192.168.8.10', N'Warehouse-Tablet2'),
('NV14', '2026-05-07 08:30:00', '2026-05-07 18:00:00', '192.168.8.11', N'POS-08'),
('NV15', '2026-05-08 08:00:00', '2026-05-08 17:00:00', '192.168.9.10', N'Laptop-Manager3'),
('NV16', '2026-05-08 08:15:00', '2026-05-08 17:30:00', '192.168.9.11', N'POS-09'),
('NV17', '2026-05-09 08:00:00', '2026-05-09 17:00:00', '192.168.10.10', N'Warehouse-Tablet3'),
('NV18', '2026-05-09 08:30:00', '2026-05-09 18:00:00', '192.168.10.11', N'POS-10'),
('NV19', '2026-05-10 08:00:00', '2026-05-10 17:00:00', '192.168.11.10', N'POS-11'),
('NV20', '2026-05-10 08:15:00', '2026-05-10 17:30:00', '192.168.11.11', N'PC-Admin2');

-- =============================================
-- 14. LỊCH SỬ CHỈNH SỬA (20 Dòng)
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
('LoaiSanPham', 'L01', 'UPDATE', N'Ten: Thit', N'Ten: Thit tuoi song', 'NV01', GETDATE()),
('SanPham', 'SP11', 'UPDATE', N'Gia: 390000', N'Gia: 400000', 'NV11', GETDATE()),
('KhachHang', 'KH11', 'UPDATE', N'Diem: 50', N'Diem: 60', 'NV12', GETDATE()),
('Kho', 'K11', 'UPDATE', N'SDT: 0901...', N'SDT: 0902...', 'NV13', GETDATE()),
('CuaHang', 'CH11', 'UPDATE', N'Ten: BHX Q3', N'Ten: GreenMart Q3', 'NV11', GETDATE()),
('NhaCungCap', 'NCC11', 'UPDATE', N'Email: vifon@vifon.com', N'Email: vifon@vifon.com.vn', 'NV11', GETDATE()),
('LoaiSanPham', 'L11', 'UPDATE', N'Ten: Thuc pham dong hop', N'Ten: Thực phẩm đóng hộp', 'NV11', GETDATE()),
('KhuyenMai', 'KM11', 'UPDATE', N'GiaTri: 15000', N'GiaTri: 20000', 'NV11', GETDATE()),
('SanPham', 'SP12', 'UPDATE', N'Gia: 40000', N'Gia: 45000', 'NV15', GETDATE()),
('KhachHang', 'KH12', 'UPDATE', N'Diem: 70', N'Diem: 80', 'NV16', GETDATE()),
('Kho', 'K12', 'UPDATE', N'SDT: 0901...', N'SDT: 0902...', 'NV17', GETDATE());
GO
