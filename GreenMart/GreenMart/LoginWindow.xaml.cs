using System;
using System.Data;
using System.Windows;
using System.Windows.Input;
using BUS;

namespace GreenMart
{
    public partial class LoginWindow : Window
    {
        NhanVienBUS bus = new();

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            XyLyDangNhap();
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                XyLyDangNhap();
            }
        }

        private void XyLyDangNhap()
        {
            string u = txtUsername.Text.Trim();
            string p = txtPassword.Password;
            if (string.IsNullOrEmpty(u) || string.IsNullOrEmpty(p))
            {
                txtError.Text = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.";
                return;
            }

            try
            {
                DataTable dt = bus.DangNhap(u, p);
                if (dt != null && dt.Rows.Count > 0)
                {
                    // Đăng nhập thành công
                    string maNV = dt.Rows[0]["MaNV"].ToString()!;
                    string hoTen = dt.Rows[0]["HoTen"].ToString()!;
                    string chucVu = dt.Rows[0]["ChucVu"].ToString()!;
                    string maCH = dt.Rows[0]["MaCH"].ToString()!;

                    // Ghi nhận lịch sử
                    try { new LichSuBUS().GhiNhanTruyCap(maNV, "127.0.0.1", "Desktop PC"); } catch { }

                    // Mở MainWindow
                    MainWindow main = new MainWindow(hoTen, chucVu, maNV, maCH);
                    main.Show();
                    this.Close();
                }
                else
                {
                    txtError.Text = "Tên đăng nhập hoặc mật khẩu không chính xác.";
                }
            }
            catch (Exception ex)
            {
                txtError.Text = "Lỗi kết nối CSDL: " + ex.Message;
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
