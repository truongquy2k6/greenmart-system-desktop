using BUS;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình CORS để cho phép ứng dụng di động kết nối (ngrok / IP nội bộ)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Giữ nguyên PascalCase giống như cột Database và Entity của Mobile App
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null;
});

var app = builder.Build();

app.UseCors("AllowAll");

// Khởi tạo các Business Logic Layer (BUS)
var sanPhamBus = new SanPhamBUS();
var loaiSpBus = new LoaiSanPhamBUS();
var cuaHangBus = new CuaHangBUS();
var khuyenMaiBus = new KhuyenMaiBUS();
var khachHangBus = new KhachHangBUS();
var hoaDonBus = new HoaDonBUS();

// ---------------- API SẢN PHẨM & LOẠI SẢN PHẨM ----------------
// Lấy danh sách sản phẩm (có thể lọc theo tên hoặc loại)
app.MapGet("/api/sanpham", (string? kw, string? maLoai) =>
{
    try
    {
        var dt = sanPhamBus.HienThi();
        var list = dt.AsEnumerable().Select(row => new
        {
            MaSP = row["MaSP"].ToString()?.Trim() ?? "",
            TenSP = row["TenSP"].ToString()?.Trim() ?? "",
            DonGia = Convert.ToDouble(row["DonGia"]),
            DonViTinh = row["DonViTinh"].ToString()?.Trim() ?? "",
            HinhAnh = row["HinhAnh"].ToString()?.Trim() ?? "",
            MoTa = row["MoTa"].ToString()?.Trim() ?? "",
            MaNCC = row["MaNCC"].ToString()?.Trim() ?? "",
            MaLoai = row["MaLoai"].ToString()?.Trim() ?? "",
            TrangThai = row["TrangThai"].ToString()?.Trim() ?? "",
            NgayTao = row.Table.Columns.Contains("NgayTao") && row["NgayTao"] != DBNull.Value 
                ? new DateTimeOffset(Convert.ToDateTime(row["NgayTao"])).ToUnixTimeMilliseconds() 
                : DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        }).ToList();

        if (!string.IsNullOrEmpty(kw))
        {
            list = list.Where(p => p.TenSP.Contains(kw, StringComparison.OrdinalIgnoreCase) || p.MoTa.Contains(kw, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (!string.IsNullOrEmpty(maLoai))
        {
            list = list.Where(p => p.MaLoai.Trim() == maLoai.Trim()).ToList();
        }

        return Results.Ok(list);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

// Lấy danh sách loại sản phẩm
app.MapGet("/api/loaisanpham", () =>
{
    try
    {
        var dt = loaiSpBus.HienThi();
        var list = dt.AsEnumerable().Select(row => new
        {
            MaLoai = row["MaLoai"].ToString()?.Trim() ?? "",
            TenLoai = row["TenLoai"].ToString()?.Trim() ?? ""
        }).ToList();
        return Results.Ok(list);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

// ---------------- API CỬA HÀNG ----------------
// Lấy danh sách cửa hàng tiện lợi
app.MapGet("/api/cuahang", () =>
{
    try
    {
        var dt = cuaHangBus.HienThi();
        var list = dt.AsEnumerable().Select(row => new
        {
            MaCH = row["MaCH"].ToString()?.Trim() ?? "",
            TenCH = row["TenCH"].ToString()?.Trim() ?? "",
            DiaChi = row["DiaChi"].ToString()?.Trim() ?? "",
            SoDienThoai = row["SoDienThoai"].ToString()?.Trim() ?? "",
            Email = row["Email"].ToString()?.Trim() ?? "",
            Latitude = row.Table.Columns.Contains("Latitude") && row["Latitude"] != DBNull.Value ? Convert.ToDouble(row["Latitude"]) : 0.0,
            Longitude = row.Table.Columns.Contains("Longitude") && row["Longitude"] != DBNull.Value ? Convert.ToDouble(row["Longitude"]) : 0.0
        }).ToList();
        return Results.Ok(list);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

// ---------------- API KHUYẾN MÃI / VOUCHER ----------------
// Lấy danh sách chương trình khuyến mãi đang áp dụng
app.MapGet("/api/khuyenmai", () =>
{
    try
    {
        var dt = khuyenMaiBus.HienThi();
        var list = dt.AsEnumerable().Select(row => new
        {
            MaKM = row["MaKM"].ToString()?.Trim() ?? "",
            TenKM = row["TenKM"].ToString()?.Trim() ?? "",
            MoTa = row["MoTa"].ToString()?.Trim() ?? "",
            LoaiKM = row["LoaiKM"].ToString()?.Trim() ?? "",
            GiaTri = Convert.ToDouble(row["GiaTri"]),
            NgayBatDau = Convert.ToDateTime(row["NgayBatDau"]),
            NgayKetThuc = Convert.ToDateTime(row["NgayKetThuc"]),
            DieuKien = Convert.ToDouble(row["DieuKien"]),
            TrangThai = row["TrangThai"].ToString()?.Trim() ?? ""
        }).ToList();
        // Lọc các khuyến mãi còn hiệu lực
        var now = DateTime.Now;
        var activeList = list.Where(k => 
            k.TrangThai.Trim() == "Đang áp dụng" &&
            k.NgayBatDau <= now &&
            k.NgayKetThuc >= now
        ).ToList();
        return Results.Ok(activeList);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

// ---------------- API KHÁCH HÀNG / THÀNH VIÊN ----------------
// Tìm kiếm thành viên theo số điện thoại (Đăng nhập di động)
app.MapGet("/api/khachhang/tim", (string sdt) =>
{
    try
    {
        var dt = hoaDonBus.TimKH(sdt);
        if (dt.Rows.Count > 0)
        {
            var row = dt.Rows[0];
            var kh = new
            {
                MaKH = row["MaKH"].ToString()?.Trim() ?? "",
                HoTen = row["HoTen"].ToString()?.Trim() ?? "",
                DiemTichLuy = Convert.ToInt32(row["DiemTichLuy"]),
                SoDienThoai = sdt.Trim(),
                DiaChi = "",
                Email = "",
                TrangThai = "Hoạt động"
            };
            return Results.Ok(kh);
        }
        return Results.NotFound(new { message = "Không tìm thấy thành viên với số điện thoại này." });
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

// Đăng ký thành viên mới
app.MapPost("/api/khachhang", (RegisterCustomerRequest request) =>
{
    try
    {
        if (string.IsNullOrEmpty(request.HoTen) || string.IsNullOrEmpty(request.SoDienThoai))
        {
            return Results.BadRequest(new { message = "Họ tên và số điện thoại không được để trống." });
        }

        // Kiểm tra xem khách hàng đã tồn tại chưa
        var dtCheck = hoaDonBus.TimKH(request.SoDienThoai);
        if (dtCheck.Rows.Count > 0)
        {
            return Results.Conflict(new { message = "Số điện thoại này đã được đăng ký." });
        }

        // Tự động sinh mã khách hàng mới
        string maKH = khachHangBus.TaoMa();
        khachHangBus.Them(
            maKH,
            request.HoTen,
            request.SoDienThoai,
            request.DiaChi ?? "",
            request.Email ?? "",
            0, // Điểm tích lũy ban đầu
            "Hoạt động"
        );

        return Results.Ok(new { maKH, message = "Đăng ký thành viên GreenMart thành công!" });
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

// Cập nhật thông tin khách hàng (Điểm tích lũy, trạng thái...)
app.MapPut("/api/khachhang", (UpdateCustomerRequest request) =>
{
    try
    {
        khachHangBus.CapNhat(
            request.MaKH,
            request.HoTen,
            request.SoDienThoai,
            request.DiaChi ?? "",
            request.Email ?? "",
            request.DiemTichLuy,
            request.TrangThai ?? "Hoạt động"
        );
        return Results.Ok(new { message = "Cập nhật thông tin khách hàng thành công!" });
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

// ---------------- API HÓA ĐƠN & CHI TIẾT HÓA ĐƠN ----------------
// Tạo hóa đơn mới (Đặt hàng từ Mobile)
app.MapPost("/api/hoadon", (CreateInvoiceRequest request) =>
{
    try
    {
        if (request.ChiTiet == null || request.ChiTiet.Count == 0)
        {
            return Results.BadRequest(new { message = "Chi tiết đơn hàng không được để trống." });
        }   

        // Tự động sinh mã hóa đơn
        string maHD = hoaDonBus.TaoMa();

        // 1. Tạo hóa đơn chính (Trim các mã để tránh khoảng trắng đệm char(10) gây lỗi)
        string maNVDefault = "NV01"; 
        
        // Ánh xạ phương thức thanh toán từ mobile tương thích với CHECK CONSTRAINT của SQL Server ('Thẻ', 'Chuyển khoản', 'Tiền mặt')
        string rawPttt = request.PhuongThucThanhToan ?? "Chuyển khoản";
        string ptttMapped = "Chuyển khoản";
        if (rawPttt.Contains("Thẻ", StringComparison.OrdinalIgnoreCase))
        {
            ptttMapped = "Thẻ";
        }
        else if (rawPttt.Contains("Tiền mặt", StringComparison.OrdinalIgnoreCase))
        {
            ptttMapped = "Tiền mặt";
        }

        string? maKHMapped = string.IsNullOrWhiteSpace(request.MaKH) || request.MaKH == "null" ? null : request.MaKH.Trim();
        string? maKMMapped = string.IsNullOrWhiteSpace(request.MaKM) || request.MaKM == "null" ? null : request.MaKM.Trim();

        using (var conn = new Microsoft.Data.SqlClient.SqlConnection(DAL.DatabaseHelper.ConnectionString))
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    // 1. Tạo hóa đơn
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.Transaction = trans;
                        cmd.CommandText = "sp_ThemHoaDon";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@MaHD", maHD.Trim());
                        cmd.Parameters.AddWithValue("@TongTien", request.TongTien);
                        cmd.Parameters.AddWithValue("@MaKH", (object?)maKHMapped ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@MaNV", maNVDefault);
                        cmd.Parameters.AddWithValue("@MaKM", (object?)maKMMapped ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@GiamGia", request.GiamGia);
                        cmd.Parameters.AddWithValue("@PhuongThucThanhToan", ptttMapped);
                        cmd.ExecuteNonQuery();
                    }

                    // 2. Thêm từng sản phẩm vào chi tiết hóa đơn
                    foreach (var item in request.ChiTiet)
                    {
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.Transaction = trans;
                            cmd.CommandText = "sp_ThemChiTietHD";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@MaHD", maHD.Trim());
                            cmd.Parameters.AddWithValue("@MaSP", item.MaSP.Trim());
                            cmd.Parameters.AddWithValue("@SoLuong", item.SoLuong);
                            cmd.Parameters.AddWithValue("@DonGia", item.DonGia);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // 3. Tự động đồng bộ cộng điểm tích lũy thành viên thực tế vào SQL Server (10.000đ = 1 điểm)
                    if (!string.IsNullOrEmpty(maKHMapped))
                    {
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.Transaction = trans;
                            cmd.CommandText = "UPDATE KhachHang SET DiemTichLuy = DiemTichLuy + @Points WHERE MaKH = @MaKH";
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Points", (int)(request.TongTien / 10000));
                            cmd.Parameters.AddWithValue("@MaKH", maKHMapped);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    trans.Commit();
                }
                catch (Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        return Results.Ok(new { maHD = maHD.Trim(), message = "Đặt hàng thành công!" });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[GreenMart] Checkout transaction failed: {ex}");
        return Results.BadRequest(new { message = ex.Message });
    }
});

// Lấy lịch sử hóa đơn của khách hàng
app.MapGet("/api/khachhang/{maKH}/hoadon", (string maKH) =>
{
    try
    {
        // Sử dụng tìm kiếm hóa đơn theo khoảng thời gian cực rộng để lấy toàn bộ lịch sử
        var tu = DateTime.Now.AddYears(-5);
        var den = DateTime.Now.AddDays(1);
        var dt = hoaDonBus.TimKiem(tu, den, maKH);
        var list = dt.AsEnumerable().Select(row => new
        {
            MaHD = row["MaHD"].ToString()?.Trim() ?? "",
            NgayLap = Convert.ToDateTime(row["NgayLap"]),
            TongTien = Convert.ToDouble(row["TongTien"]),
            MaKH = row["MaKH"].ToString()?.Trim() ?? "",
            MaNV = row["MaNV"].ToString()?.Trim() ?? "",
            MaCH = row["MaCH"].ToString()?.Trim() ?? "",
            MaKM = row["MaKM"].ToString()?.Trim() ?? "",
            GiamGia = Convert.ToDouble(row["GiamGia"]),
            PhuongThucThanhToan = row["PhuongThucThanhToan"].ToString()?.Trim() ?? "",
            TrangThai = row["TrangThai"].ToString()?.Trim() ?? ""
        }).ToList();
        return Results.Ok(list);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

// Lấy chi tiết hóa đơn
app.MapGet("/api/hoadon/{maHD}/chitiet", (string maHD) =>
{
    try
    {
        var dt = hoaDonBus.LayChiTiet(maHD);
        var list = dt.AsEnumerable().Select(row => new
        {
            MaHD = row["MaHD"].ToString()?.Trim() ?? "",
            MaSP = row["MaSP"].ToString()?.Trim() ?? "",
            SoLuong = Convert.ToInt32(row["SoLuong"]),
            DonGia = Convert.ToDouble(row["DonGia"]),
            ThanhTien = Convert.ToDouble(row["ThanhTien"])
        }).ToList();
        return Results.Ok(list);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

// ---------------- API CẤU HÌNH THANH TOÁN CỬA HÀNG ----------------
app.MapGet("/api/cuahang/{maCH}/cauhinh", (string maCH) =>
{
    try
    {
        var bus = new CauHinhBUS();
        var ch = bus.GetCauHinh(maCH);
        if (ch != null && !string.IsNullOrEmpty(ch.MaCH))
        {
            return Results.Ok(new
            {
                MaCH = ch.MaCH.Trim(),
                BankId = ch.BankId.Trim(),
                AccountNo = ch.AccountNo.Trim(),
                AccountName = ch.AccountName.Trim()
            });
        }
        return Results.NotFound(new { message = "Chưa cấu hình tài khoản nhận tiền cho cửa hàng này." });
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

// ---------------- API CHATBOT AI SONG KHỎE (PROXY QUA GEMINI) ----------------
var chatbotHandler = async (HttpContext context) =>
{
    try
    {
        string? clientType = context.Request.Headers["X-Client-Type"];
        string? apiKey = null;

        if (string.Equals(clientType, "Desktop", StringComparison.OrdinalIgnoreCase))
        {
            apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY_Desktop");
        }
        else if (string.Equals(clientType, "Mobile", StringComparison.OrdinalIgnoreCase))
        {
            apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY_Mobile");
        }

        // Fallback hierarchy: Generic environment key -> Mobile key -> Desktop key -> Appsettings config -> Default placeholder
        apiKey ??= Environment.GetEnvironmentVariable("GEMINI_API_KEY")
            ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY_Mobile")
            ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY_Desktop")
            ?? builder.Configuration["ApiKey"]
            ?? "YOUR_GEMINI_API_KEY_HERE";

        using var reader = new StreamReader(context.Request.Body);
        string requestBody = await reader.ReadToEndAsync();
        
        string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.1-flash-lite:generateContent?key={apiKey}";
        
        using var httpClient = new HttpClient();
        var content = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(url, content);
        
        string responseString = await response.Content.ReadAsStringAsync();
        return Results.Content(responseString, "application/json", statusCode: (int)response.StatusCode);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
};

app.MapPost("/api/chatbot/ask", chatbotHandler);
app.MapPost("/api/chatbot", chatbotHandler);

app.Run();

// ---------------- CLASS HỖ TRỢ / REQUEST DTO ----------------
public record RegisterCustomerRequest(string HoTen, string SoDienThoai, string? DiaChi, string? Email);
public record UpdateCustomerRequest(string MaKH, string HoTen, string SoDienThoai, string? DiaChi, string? Email, int DiemTichLuy, string? TrangThai);
public record CreateInvoiceRequest(
    string MaKH,
    decimal TongTien,
    string? MaKM,
    decimal GiamGia,
    string? PhuongThucThanhToan,
    List<InvoiceDetailRequest> ChiTiet
);
public record InvoiceDetailRequest(string MaSP, int SoLuong, decimal DonGia);
