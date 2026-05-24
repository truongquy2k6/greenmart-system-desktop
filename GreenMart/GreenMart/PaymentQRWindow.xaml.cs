using System;
using System.Windows;
using System.Windows.Media.Imaging;
using BUS;
using DTO;

namespace GreenMart
{
    public partial class PaymentQRWindow : Window
    {
        public bool IsPaid { get; private set; } = false;

        public PaymentQRWindow(decimal amount, string description)
        {
            InitializeComponent();
            txtAmount.Text = string.Format("{0:#,##0} ₫", amount);
            LoadQR(amount, description);
        }

        private void LoadQR(decimal amount, string description)
        {
            try
            {
                var config = new CauHinhBUS().GetCauHinh(MainWindow.CurrentCH);
                if (string.IsNullOrEmpty(config.BankId) || string.IsNullOrEmpty(config.AccountNo))
                {
                    MessageBox.Show("Chưa cấu hình tài khoản ngân hàng. Vui lòng cấu hình trước khi thanh toán QR.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // URL encode the description and account name
                string encodedDesc = Uri.EscapeDataString(description);
                string encodedName = Uri.EscapeDataString(config.AccountName);
                
                // Format: https://img.vietqr.io/image/{bank_bin}-{account_no}-compact2.jpg?amount={amount}&addInfo={desc}&accountName={name}
                string url = $"https://img.vietqr.io/image/{config.BankId}-{config.AccountNo}-compact2.jpg?amount={amount}&addInfo={encodedDesc}&accountName={encodedName}";

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(url);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                // Hide loading spinner and show image
                imgQR.Source = bitmap;
                loadingProgress.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                loadingProgress.Visibility = Visibility.Collapsed;
                MessageBox.Show("Không thể tải mã QR: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDong_Click(object sender, RoutedEventArgs e)
        {
            IsPaid = false;
            this.Close();
        }

        private void btnDaNhan_Click(object sender, RoutedEventArgs e)
        {
            IsPaid = true;
            this.Close();
        }
    }
}
