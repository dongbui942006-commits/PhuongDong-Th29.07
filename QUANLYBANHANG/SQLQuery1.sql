-- Tạo database
CREATE DATABASE QLBanHang;
GO

USE QLBanHang;
GO

-- Bảng khách hàng
CREATE TABLE KhachHang (
    MaKH NVARCHAR(10) PRIMARY KEY,
    TenKH NVARCHAR(50),
    DiaChi NVARCHAR(100),
    DienThoai NVARCHAR(15)
);

-- Bảng nhân viên
CREATE TABLE NhanVien (
    MaNV NVARCHAR(10) PRIMARY KEY,
    TenNV NVARCHAR(50)
);

-- Bảng hàng hóa
CREATE TABLE HangHoa (
    MaHang NVARCHAR(10) PRIMARY KEY,
    TenHang NVARCHAR(50),
    DonGia FLOAT
);

-- Bảng hóa đơn
CREATE TABLE HoaDon (
    MaHD NVARCHAR(10) PRIMARY KEY,
    NgayLap DATE,
    MaNV NVARCHAR(10),
    MaKH NVARCHAR(10),
    TongTien FLOAT,
    FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV),
    FOREIGN KEY (MaKH) REFERENCES KhachHang(MaKH)
);

-- Bảng chi tiết hóa đơn
CREATE TABLE ChiTietHD (
    MaHD NVARCHAR(10),
    MaHang NVARCHAR(10),
    SoLuong INT,
    GiamGia FLOAT,
    ThanhTien FLOAT,
    PRIMARY KEY (MaHD, MaHang),
    FOREIGN KEY (MaHD) REFERENCES HoaDon(MaHD),
    FOREIGN KEY (MaHang) REFERENCES HangHoa(MaHang)
);

-- Dữ liệu mẫu
INSERT INTO KhachHang VALUES 
('KH01', N'Bùi Phương Đông', N'Hà Nội', '0912345672'),
('KH02', N'Hoàng Văn Thắng', N'Hồ Chí Minh', '0987454321'),
('KH03', N'Nguyễn Minh Dũng ', N'Hà Nội', '0912367678'),
('KH04', N'Nguyễn Quang Lâm', N'Hồ Chí Minh', '0997654321'),
('KH05', N'Nguyễn Tuấn Anh', N'Hà Nội', '0912345628'),
('KH06', N'Nguyễn Hữu Dũng ', N'Hồ Chí Minh', '0927654321'),
('KH07', N'Nguyễn Văn Hưng', N'Hà Nội', '0912346778');


INSERT INTO NhanVien VALUES 
('NV01', N'Nguyễn Văn Nhân'),
('NV02', N'Lê Thị Hoa'),
('NV03', N'Nguyễn Văn Thăng'),
('NV04', N'Lê Thị Huyền '),
('NV05', N'Nguyễn Văn Thuần'),
('NV06', N'Lê Thị Nga');



INSERT INTO HangHoa VALUES 
('HH01', N'Điện thoại', 5000000),
('HH02', N'Máy tính bảng', 7000000),
('HH03', N'Laptop', 15000000),
('HH04', N'TiVi', 500000000),
('HH05', N'BimBim', 7000),
('HH06', N'Bàn Phím ', 150000);