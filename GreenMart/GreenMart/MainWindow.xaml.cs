using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GreenMart.UserControls;

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
        public static string CurrentRole = ""; // "Quản lý" | "Nhân viên kho" | "Nhân viên bán hàng"

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
        }

        /// <summary>
        /// Áp dụng phân quyền menu theo chức vụ
        /// </summary>
        private void ApplyRolePermissions(string role)
        {
            switch (role)
            {
                case "Quản lý":
                    // Quản lý: toàn quyền — không ẩn gì
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
                    SetVisible(grpLichSu, false);
                    SetVisible(btnLSTC, false);
                    SetVisible(btnLSCS, false);
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
                    SetVisible(grpLichSu, false);
                    SetVisible(btnLSTC, false);
                    SetVisible(btnLSCS, false);
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
                    SetVisible(grpLichSu, false);
                    SetVisible(btnLSTC, false);
                    SetVisible(btnLSCS, false);
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
                "Quản lý"            => true, // Quản lý toàn quyền
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
                new LoginWindow().Show();
                this.Close();
            }
        }
    }
}