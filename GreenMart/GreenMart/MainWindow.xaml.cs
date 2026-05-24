using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GreenMart.UserControls;
using Microsoft.Data.SqlClient;
using System.Data;
using DAL;

namespace GreenMart
{
    /// <summary>
    /// Phân quyền theo ChucVu trong DB:
    ///   "Quản lý"            - Toàn quyền truy cập tất cả chức năng
    ///   "Nhân viên kho"      - Kho hàng, Chi tiết kho, Sản phẩm, Loại sản phẩm, Nhà cung cấp
    ///   "Nhân viên bán hàng" - POS, Khách hàng, Hóa đơn, Dashboard
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string CurrentNV = "";
        public static string CurrentCH = "CH01";
        public static string CurrentRole = ""; // "Admin" | "Quản lý" | "Nhân viên kho" | "Nhân viên bán hàng"
        private int _notificationCount = 0;

        public MainWindow(string hoTen = "Quản trị viên", string chucVu = "Admin", string maNV = "NV00", string maCH = "CH01")
        {
            InitializeComponent();
            CurrentNV = maNV;
            CurrentCH = maCH;
            CurrentRole = chucVu;

            // Hiển thị thông tin user
            txtUserName.Text = hoTen;
            txtUserRole.Text = $"{chucVu}";
            txtUserInitial.Text = hoTen.Length > 0 ? hoTen[0].ToString().ToUpper() : "U";
            txtDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

            // Áp dụng phân quyền
            ApplyRolePermissions(chucVu);

            // Load dữ liệu thông báo có sẵn từ Database
            LoadInitialNotifications();

            // Bắt đầu nhận thông báo thời gian thực
            try
            {
                SqlDependency.Start(DatabaseHelper.ConnectionString);
                RegisterNotificationDependency();
            }
            catch (Exception ex)
            {
                Console.WriteLine("SqlDependency error: " + ex.Message);
            }

            // Hiển thị trang mặc định theo vai trò
            if (chucVu == "Nhân viên kho")
            {
                pageContent.Content = new ucKhoHang();
                txtPageTitle.Text = "Quản lý kho hàng";
                btnKhoHang.IsChecked = true;
            }
            else
            {
                pageContent.Content = new ucDashboard();
                txtPageTitle.Text = "Bảng điều khiển";
                btnDashboard.IsChecked = true;
            }

            chatBot.OnCloseClicked = () => chatBot.Visibility = Visibility.Collapsed;
        }

        private void btnOpenChat_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentRole != "Admin" && CurrentRole != "Quản lý")
            {
                MessageBox.Show("Tính năng Trợ lý AI chỉ dành cho Admin và Quản lý!", "Từ chối truy cập", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            chatBot.Visibility = chatBot.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Áp dụng phân quyền menu theo chức vụ
        /// </summary>
        private void ApplyRolePermissions(string role)
        {
            switch (role)
            {
                case "Admin":
                    // Admin: toàn quyền
                    SetRoleBadge("Quản trị hệ thống", "#D32F2F", "#FFEBEE", "Security");
                    break;

                case "Quản lý":
                    // Quản lý: toàn quyền trừ cấu hình
                    SetVisible(btnCauHinh, false);
                    SetRoleBadge("Quản lý", "#1B5E20", "#E8F5E9", "ShieldAccount");
                    break;

                case "Nhân viên kho":
                    // Nhân viên kho: Kho hàng, Chi tiết kho, Sản phẩm, Loại SP, NCC
                    // Ẩn: POS, Dashboard, KH, HĐ, NV, CH, KM, Lịch sử
                    SetVisible(grpBanHang, false);
                    SetVisible(btnPOS, false);
                    SetVisible(grpTongQuan, false);
                    SetVisible(btnDashboard, false);
                    SetVisible(btnKH, false);
                    SetVisible(btnHD, false);
                    SetVisible(grpAdminOnly, false);
                    SetVisible(btnNV, false);
                    SetVisible(btnCH, false);
                    SetVisible(btnKM, false);
                    SetVisible(btnCauHinh, false);
                    SetVisible(grpLichSu, false);
                    SetVisible(btnLSTC, false);
                    SetVisible(btnLSCS, false);
                    SetVisible(btnOpenChat, false);
                    SetRoleBadge("Nhân viên kho", "#1565C0", "#E3F2FD", "Warehouse");
                    break;

                case "Nhân viên bán hàng":
                    // Nhân viên bán hàng: POS, KH, HĐ, Dashboard
                    // Ẩn: SP, Loại SP, NCC, Kho, Chi tiết kho, NV, CH, KM, Lịch sử
                    SetVisible(btnSanPham, false);
                    SetVisible(btnLoaiSP, false);
                    SetVisible(btnNCC, false);
                    SetVisible(btnKhoHang, false);
                    SetVisible(btnChiTietKho, false);
                    SetVisible(grpAdminOnly, false);
                    SetVisible(btnNV, false);
                    SetVisible(btnCH, false);
                    SetVisible(btnKM, false);
                    SetVisible(btnCauHinh, false);
                    SetVisible(grpLichSu, false);
                    SetVisible(btnLSTC, false);
                    SetVisible(btnLSCS, false);
                    SetVisible(btnOpenChat, false);
                    SetRoleBadge("Nhân viên bán hàng", "#6A1B9A", "#F3E5F5", "CashRegister");
                    break;

                default:
                    // Vai trò không xác định → quyền tối thiểu (chỉ xem Dashboard)
                    SetVisible(grpBanHang, false);
                    SetVisible(btnPOS, false);
                    SetVisible(btnSanPham, false);
                    SetVisible(btnLoaiSP, false);
                    SetVisible(btnNCC, false);
                    SetVisible(btnKhoHang, false);
                    SetVisible(btnChiTietKho, false);
                    SetVisible(btnKH, false);
                    SetVisible(btnHD, false);
                    SetVisible(grpAdminOnly, false);
                    SetVisible(btnNV, false);
                    SetVisible(btnCH, false);
                    SetVisible(btnKM, false);
                    SetVisible(btnCauHinh, false);
                    SetVisible(grpLichSu, false);
                    SetVisible(btnLSTC, false);
                    SetVisible(btnLSCS, false);
                    SetVisible(btnOpenChat, false);
                    SetRoleBadge(role, "#37474F", "#ECEFF1", "Account");
                    break;
            }
        }

        private void SetVisible(UIElement el, bool visible)
            => el.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;

        private void SetRoleBadge(string label, string fgColor, string bgColor, string iconKind)
        {
            txtRoleBadge.Text = label;
            txtRoleBadge.Foreground = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(fgColor));
            roleBadge.Background = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(bgColor));
            roleIcon.Foreground = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(fgColor));
            try { roleIcon.Kind = (MaterialDesignThemes.Wpf.PackIconKind)Enum.Parse(typeof(MaterialDesignThemes.Wpf.PackIconKind), iconKind); } catch { }
        }

        private void Nav_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as RadioButton;
            string uid = btn?.Uid ?? "";

            // Kiểm tra quyền truy cập
            if (!HasPermission(uid))
            {
                MessageBox.Show("Bạn không có quyền truy cập chức năng này!", "Từ chối truy cập", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            switch (uid)
            {
                case "Dashboard":    pageContent.Content = new ucDashboard();    txtPageTitle.Text = "Bảng điều khiển";      break;
                case "POS":          pageContent.Content = new ucBanHang();      txtPageTitle.Text = "Bán hàng (POS)";       break;
                case "SanPham":      pageContent.Content = new ucSanPham();      txtPageTitle.Text = "Quản lý sản phẩm";     break;
                case "LoaiSP":       pageContent.Content = new ucLoaiSanPham();  txtPageTitle.Text = "Loại sản phẩm";        break;
                case "NCC":          pageContent.Content = new ucNhaCungCap();   txtPageTitle.Text = "Nhà cung cấp";         break;
                case "KH":           pageContent.Content = new ucKhachHang();    txtPageTitle.Text = "Khách hàng";           break;
                case "NV":           pageContent.Content = new ucNhanVien();     txtPageTitle.Text = "Quản lý nhân viên";    break;
                case "HD":           pageContent.Content = new ucHoaDon();       txtPageTitle.Text = "Quản lý hóa đơn";      break;
                case "CH":           pageContent.Content = new ucCuaHang();      txtPageTitle.Text = "Quản lý cửa hàng";     break;
                case "Kho":          pageContent.Content = new ucKhoHang();      txtPageTitle.Text = "Quản lý kho hàng";     break;
                case "ChiTietKho":   pageContent.Content = new ucChiTietKho();   txtPageTitle.Text = "Chi tiết kho hàng";    break;
                case "KM":           pageContent.Content = new ucKhuyenMai();    txtPageTitle.Text = "Khuyến mãi";           break;
                case "CauHinh":      pageContent.Content = new ucCauHinh();      txtPageTitle.Text = "Cấu hình hệ thống";    break;
                case "LSTC":         pageContent.Content = new ucLichSuTruyCap();txtPageTitle.Text = "Lịch sử truy cập";     break;
                case "LSCS":         pageContent.Content = new ucLichSuChinhSua();txtPageTitle.Text = "Lịch sử chỉnh sửa";  break;
            }
        }

        /// <summary>
        /// Kiểm tra quyền truy cập theo vai trò
        /// </summary>
        private bool HasPermission(string uid)
        {
            return CurrentRole switch
            {
                "Admin"              => true, // Admin toàn quyền
                "Quản lý"            => uid != "CauHinh", // Quản lý không được vào cấu hình
                "Nhân viên kho"      => uid is "Kho" or "ChiTietKho" or "SanPham" or "LoaiSP" or "NCC",
                "Nhân viên bán hàng" => uid is "POS" or "KH" or "HD" or "Dashboard",
                _                    => uid == "Dashboard"
            };
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try { new BUS.LichSuBUS().GhiNhanDangXuat(CurrentNV); } catch { }
                try { SqlDependency.Stop(DatabaseHelper.ConnectionString); } catch { }
                new LoginWindow().Show();
                this.Close();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            try { SqlDependency.Stop(DatabaseHelper.ConnectionString); } catch { }
            base.OnClosed(e);
        }

        private void btnNotifications_Click(object sender, RoutedEventArgs e)
        {
            popupNotifications.IsOpen = !popupNotifications.IsOpen;
            if (popupNotifications.IsOpen)
            {
                _notificationCount = 0;
                badgeNotif.Badge = "";
            }
        }

        private void LoadInitialNotifications()
        {
            try
            {
                // Gọi stored procedure theo yêu cầu để dễ bảo trì
                DataTable dt = DatabaseHelper.ExecuteQuery("sp_LayLichSuChinhSua");

                if (dt.Rows.Count > 0)
                {
                    // Lấy tối đa 15 dòng gần nhất
                    int count = Math.Min(15, dt.Rows.Count);
                    // Lặp từ dưới lên để dòng mới nhất (index 0) được Insert lên đầu (index 0 của panel)
                    for (int i = count - 1; i >= 0; i--)
                    {
                        DataRow row = dt.Rows[i];
                        string maNV = row["MaNV"]?.ToString() ?? "";
                        string bang = row["TenBang"]?.ToString() ?? "";
                        string hd = row["HanhDong"]?.ToString() ?? "";
                        string hoTen = row["HoTen"]?.ToString() ?? maNV;
                        
                        DateTime tg = DateTime.Now;
                        if (row["ThoiGian"] != DBNull.Value)
                            tg = Convert.ToDateTime(row["ThoiGian"]);

                        // Có thể bỏ qua thao tác của chính mình nếu muốn, nhưng để khởi tạo ta hiển thị tất cả
                        AddNotificationUI(hoTen, hd, bang, tg);
                        
                        _notificationCount++;
                    }
                    
                    if (_notificationCount > 0)
                    {
                        badgeNotif.Badge = _notificationCount > 9 ? "9+" : _notificationCount.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi load notifications: " + ex.Message);
            }
        }

        private void RegisterNotificationDependency()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT MaLS, TenBang, HanhDong, MaNV, ThoiGian FROM dbo.LichSuChinhSua", conn))
                    {
                        cmd.Notification = null;
                        SqlDependency dependency = new SqlDependency(cmd);
                        dependency.OnChange += Dependency_OnChange;
                        cmd.ExecuteReader();
                    }
                }
            }
            catch { }
        }

        private void Dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            SqlDependency dependency = sender as SqlDependency;
            if (dependency != null) dependency.OnChange -= Dependency_OnChange;

            if (e.Type == SqlNotificationType.Change)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    // Gọi stored procedure để lấy dòng mới nhất
                    DataTable dt = DatabaseHelper.ExecuteQuery("sp_LayLichSuChinhSua");

                    if (dt.Rows.Count > 0)
                    {
                        // Dòng mới nhất ở index 0 vì SP sắp xếp giảm dần theo thời gian
                        DataRow row = dt.Rows[0];
                        string maNV = row["MaNV"].ToString();
                        
                        // Không thông báo thao tác của chính mình
                        if (maNV != CurrentNV)
                        {
                            string bang = row["TenBang"].ToString();
                            string hd = row["HanhDong"].ToString();
                            DateTime tg = Convert.ToDateTime(row["ThoiGian"]);
                            string hoTen = row["HoTen"]?.ToString() ?? maNV;

                            AddNotificationUI(hoTen, hd, bang, tg);
                            _notificationCount++;
                            badgeNotif.Badge = _notificationCount > 9 ? "9+" : _notificationCount.ToString();
                        }
                    }
                });
            }

            // Đăng ký lại
            RegisterNotificationDependency();
        }

        private void AddNotificationUI(string nvTen, string hd, string bang, DateTime tg)
        {
            if (txtNoNotif != null) txtNoNotif.Visibility = Visibility.Collapsed;

            Border item = new Border
            {
                Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#F8F9FA")),
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(10),
                Margin = new Thickness(10, 5, 10, 5)
            };

            StackPanel sp = new StackPanel();
            
            TextBlock txtTitle = new TextBlock
            {
                Text = $"{nvTen} vừa {hd} dữ liệu {bang}",
                FontWeight = FontWeights.SemiBold,
                Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2C3E50")),
                TextWrapping = TextWrapping.Wrap
            };
            
            TextBlock txtTime = new TextBlock
            {
                Text = tg.ToString("dd/MM/yyyy HH:mm:ss"),
                FontSize = 11,
                Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#7F8C8D")),
                Margin = new Thickness(0, 5, 0, 0)
            };

            sp.Children.Add(txtTitle);
            sp.Children.Add(txtTime);
            item.Child = sp;

            pnlNotificationList.Children.Insert(0, item);
        }
    }
}