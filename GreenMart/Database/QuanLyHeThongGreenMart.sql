--Tạo database Quản lý hệ thống bách hoá xanh 
create database QuanLyHeThongGreenMart
go
--Đi tới database Quản lý hệ thống bách hoá xanh 
use QuanLyHeThongGreenMart 
go


--Bảng sản phẩm
create table SanPham
(
	MaSP char(10) primary key,
	TenSP nvarchar(100),
	DonGia decimal(10,2) check (DonGia >= 0),
	DonViTinh nvarchar(20),
	HinhAnh nvarchar(255),
	MoTa nvarchar(255),
	MaNCC char(10),
	MaLoai char(10),
	TrangThai nvarchar(50) default N'Đang kinh doanh' check (TrangThai in (N'Đang kinh doanh', N'Ngừng kinh doanh')),
	NgayTao datetime default getdate()
)
go

--Bảng loại sản phẩm
create table LoaiSanPham
(
	MaLoai char(10) primary key,
	TenLoai nvarchar(50) unique
)
go

--Bảng nhà cung cấp
create table NhaCungCap
(
	MaNCC char(10) primary key,
	TenNCC nvarchar(100) unique,
	DiaChi nvarchar(200),
	SoDienThoai varchar(15) unique check (SoDienThoai not like '%[^0-9]%'),
	Email varchar(100) unique check (Email like '%@%'),
	TrangThai nvarchar(50) default N'Hoạt động' check (TrangThai in (N'Hoạt động', N'Ngừng hoạt động'))
)
go

--Bảng khách hàng
create table KhachHang
(
	MaKH char(10) primary key,
	HoTen nvarchar(100),
	SoDienThoai varchar(15) unique check (SoDienThoai not like '%[^0-9]%'),
	DiaChi nvarchar(200),
	Email varchar(100) unique check (Email like '%@%'),
	DiemTichLuy int default 0 check (DiemTichLuy >= 0),
	NgayTao datetime default getdate(),
	TrangThai nvarchar(50) default N'Hoạt động' check (TrangThai in (N'Hoạt động', N'Khóa'))
)
go

--Bảng hoá đơn
create table HoaDon
(
	MaHD char(10) primary key,
	NgayLap datetime,
	TongTien decimal(12,2) check (TongTien >= 0),
	MaKH char(10),
	MaNV char(10),
	MaCH CHAR(10),
	MaKM char(10), -- Mã khuyến mãi áp dụng cho cả hoá đơn
	GiamGia decimal(12,2) default 0,
	PhuongThucThanhToan nvarchar(50) check (PhuongThucThanhToan in (N'Tiền mặt', N'Chuyển khoản', N'Thẻ')), 
	TrangThai nvarchar(50) default N'Đã thanh toán' check (TrangThai in (N'Chờ xử lý', N'Đã thanh toán', N'Đã hủy'))
)
go

--Bảng chi tiết hoá đơn
create table ChiTietHoaDon
(
	MaHD char(10),
	MaSP char(10),
	SoLuong int check (SoLuong > 0),
	DonGia decimal(12,2) check (DonGia >= 0),
	ThanhTien decimal (12,2) check (ThanhTien >= 0),
	primary key(MaHD, MaSP)
)
go

--Bảng nhân viên
create table NhanVien
(
	MaNV char(10) primary key,
	HoTen nvarchar(100),
	ChucVu nvarchar(50) check (ChucVu in (N'Nhân viên bán hàng', N'Nhân viên kho', N'Quản lý')),
	NgaySinh date check (NgaySinh < getdate()),
	GioiTinh nchar(10) check (GioiTinh in (N'Nam', N'Nữ')),
	SoDienThoai varchar(15) unique check (SoDienThoai not like '%[^0-9]%'),
	DiaChi nvarchar(200),
	TenDangNhap varchar(50) unique,
	MatKhau varchar(255),
	MaCH char(10),
	TrangThai nvarchar(50) default N'Đang làm việc' check (TrangThai in (N'Đang làm việc', N'Đã nghỉ việc')),
	NgayVaoLam date default getdate(),
	constraint CK_NV_Ngay check (NgayVaoLam >= NgaySinh)
)
go


-- Bảng Cửa Hàng
create table CuaHang
(
    MaCH char(10) primary key,
    TenCH nvarchar(100) unique,
    DiaChi nvarchar(200) unique,
    SoDienThoai varchar(15) unique check (SoDienThoai not like '%[^0-9]%'),
    Email varchar(100) unique check (Email like '%@%')
)
go

-- Bảng Kho
create table Kho
(
    MaKho char(10) primary key,
    TenKho nvarchar(100) unique,
    DiaChi nvarchar(200),
    SoDienThoai nvarchar(15) unique check (SoDienThoai not like '%[^0-9]%'),
    MaCH char(10) 
)
go

-- Chi tiết kho (sản phẩm trong từng kho)
create table ChiTietKho
(
    MaKho char(10),
    MaSP char(10),
    SoLuong int check (SoLuong >= 0),
	primary key (MaKho, MaSP)
)
go

-- Bảng Khuyến Mãi
create table KhuyenMai
(
	MaKM char(10) primary key,
	TenKM nvarchar(100),
	MoTa nvarchar(255),
	LoaiKM nvarchar(50) check (LoaiKM in (N'Giảm theo %', N'Giảm theo số tiền')), 
	GiaTri decimal(12,2) check (GiaTri > 0),
	NgayBatDau datetime,
	NgayKetThuc datetime,
	DieuKien decimal(12,2) default 0, -- Gia tri don hang toi thieu
	TrangThai nvarchar(50) default N'Đang áp dụng' check (TrangThai in (N'Đang áp dụng', N'Đã kết thúc', N'Chưa bắt đầu')),
	constraint CK_KM_Ngay check (NgayKetThuc >= NgayBatDau),
	constraint CK_KM_GiaTri check (not (LoaiKM = N'Giảm theo %' and GiaTri > 100))
)
go

-- Bảng áp dụng khuyến mãi cho sản phẩm/loại sản phẩm
create table ApDungKhuyenMai
(
	MaAD int identity(1,1) primary key,
	MaKM char(10),
	MaSP char(10) null,
	MaLoai char(10) null,
	constraint CK_ADKM_OnlyOne check (not (MaSP is not null and MaLoai is not null)) -- Chỉ được chọn 1 trong 2
)
go

-- Bảng Lịch sử truy cập
create table LichSuTruyCap
(
	MaTruyCap int identity(1,1) primary key,
	MaNV char(10),
	ThoiGianDangNhap datetime default getdate(),
	ThoiGianDangXuat datetime null,
	DiaChiIP varchar(50),
	ThietBi nvarchar(255)
)
go

-- Bảng Lịch sử chỉnh sửa dữ liệu
create table LichSuChinhSua
(
	MaLS int identity(1,1) primary key,
	TenBang nvarchar(50),
	MaBanGhi char(20),
	HanhDong nvarchar(20) check (HanhDong in ('INSERT', 'UPDATE', 'DELETE')), 
	NoiDungCu nvarchar(max),
	NoiDungMoi nvarchar(max),
	MaNV char(10),
	ThoiGian datetime default getdate()
)
go


---RÀNG BUỘC

--Ràng buộc bảng sản phẩm
alter table SanPham
add constraint FK_SanPham_LoaiSanPham
foreign key(MaLoai)
references LoaiSanPham(MaLoai);
go

alter table SanPham
add constraint FK_SanPham_NhaCungCap
foreign key(MaNCC)
references NhaCungCap(MaNCC)
go


--Ràng buộc bảng hoá đơn
alter table HoaDon
add constraint FK_HoaDon_KhachHang
foreign key(MaKH)
references KhachHang(MaKH)
go

alter table HoaDon
add constraint FK_HoaDon_NhanVien
foreign key(MaNV)
references NhanVien(MaNV)
go

alter table HoaDon
add constraint FK_HoaDon_CuaHang
foreign key(MaCH)
references CuaHang(MaCH);
go

--Ràng buộc bảng chi tiết hoá đơn
alter table ChiTietHoaDon
add constraint FK_CTHD_HD 
foreign key (MaHD) 
references HoaDon(MaHD)
go
alter table ChiTietHoaDon
add constraint FK_CTHD_SP 
foreign key (MaSP) 
references SanPham(MaSP)
go

--Ràng buộc bảng nhân viên
alter table NhanVien
add constraint FK_NhanVien_CuaHang
foreign key(MaCH)
references CuaHang(MaCH);
go
--Ràng buộc bảng kho 
alter table Kho
add constraint FK_Kho_CuaHang
foreign key(MaCH)
references CuaHang(MaCH);
go
--Ràng buộc bảng chi tiết kho
alter table ChiTietKho
add constraint FK_ChiTietKho_Kho
foreign key (MaKho)
references Kho(MaKho);
go
alter table ChiTietKho
add constraint FK_ChiTietKho_SanPham
foreign key (MaSP)
references SanPham(MaSP);
go

-- Ràng buộc bảng hoá đơn với khuyến mãi
alter table HoaDon
add constraint FK_HoaDon_KhuyenMai
foreign key(MaKM)
references KhuyenMai(MaKM);
go

-- Ràng buộc bảng áp dụng khuyến mãi
alter table ApDungKhuyenMai
add constraint FK_ADKM_KM
foreign key(MaKM)
references KhuyenMai(MaKM);
go

alter table ApDungKhuyenMai
add constraint FK_ADKM_SP
foreign key(MaSP)
references SanPham(MaSP);
go

alter table ApDungKhuyenMai
add constraint FK_ADKM_Loai
foreign key(MaLoai)
references LoaiSanPham(MaLoai);
go

-- Ràng buộc bảng lịch sử truy cập
alter table LichSuTruyCap
add constraint FK_LSTC_NV
foreign key(MaNV)
references NhanVien(MaNV);
go

-- Ràng buộc bảng lịch sử chỉnh sửa
alter table LichSuChinhSua
add constraint FK_LSCS_NV
foreign key(MaNV)
references NhanVien(MaNV);
go
