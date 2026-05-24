using System.Windows;
using System.Windows.Controls;
using BUS;
using DTO;
using System.Net.Http;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Windows.Media.Imaging;

namespace GreenMart.UserControls
{
    public class BankInfo
    {
        public string bin { get; set; } = "";
        public string shortName { get; set; } = "";
        public string name { get; set; } = "";
        public string logo { get; set; } = "";
        public string DisplayName => $"[{shortName}] {name}";
    }

    public class BankApiResponse
    {
        public string code { get; set; } = "";
        public string desc { get; set; } = "";
        public List<BankInfo> data { get; set; } = new List<BankInfo>();
    }

    public partial class ucCauHinh : UserControl
    {
        public ucCauHinh()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadBanks();
            LoadVietQRConfig();
        }

        private CauHinhBUS bus = new CauHinhBUS();
        private List<BankInfo> banks = new List<BankInfo>();

        private async Task LoadBanks()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync("https://api.vietqr.io/v2/banks");
                    var result = JsonSerializer.Deserialize<BankApiResponse>(response);
                    if (result != null && result.data != null)
                    {
                        banks = result.data;
                        cboBank.ItemsSource = banks;
                    }
                }
            }
            catch { }
        }

        private void LoadVietQRConfig()
        {
            var config = bus.GetCauHinh(MainWindow.CurrentCH);
            cboBank.SelectedValue = config.BankId;
            txtAccountNo.Text = config.AccountNo;
            txtAccountName.Text = config.AccountName;
            UpdatePreview(null, null);
        }

        private void UpdatePreview(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) return;
            string bankId = cboBank.SelectedValue?.ToString() ?? "";
            string accountNo = txtAccountNo.Text.Trim();
            string accountName = txtAccountName.Text.Trim().ToUpper();

            if (string.IsNullOrEmpty(bankId) || string.IsNullOrEmpty(accountNo))
            {
                imgQRPreview.Source = null;
                txtNoPreview.Visibility = Visibility.Visible;
                lblPreviewName.Visibility = Visibility.Collapsed;
                lblPreviewBank.Visibility = Visibility.Collapsed;
                brdPreviewAmount.Visibility = Visibility.Collapsed;
                return;
            }

            txtNoPreview.Visibility = Visibility.Collapsed;
            lblPreviewName.Visibility = Visibility.Visible;
            lblPreviewBank.Visibility = Visibility.Visible;
            brdPreviewAmount.Visibility = Visibility.Visible;
            
            lblPreviewName.Text = string.IsNullOrEmpty(accountName) ? "TÊN CHỦ TÀI KHOẢN" : accountName;
            lblPreviewBank.Text = $"{((BankInfo)cboBank.SelectedItem)?.shortName ?? bankId} - {accountNo}";

            // Load Image
            string encodedName = Uri.EscapeDataString(accountName);
            string url = $"https://img.vietqr.io/image/{bankId}-{accountNo}-compact2.jpg?amount=100000&addInfo=Thanh toan don hang&accountName={encodedName}";

            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(url);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                imgQRPreview.Source = bitmap;
            }
            catch { }
        }

        private void btnLuuVietQR_Click(object sender, RoutedEventArgs e)
        {
            if (cboBank.SelectedValue == null || string.IsNullOrWhiteSpace(txtAccountNo.Text))
            {
                MessageBox.Show("Vui lòng chọn Ngân hàng và nhập Số tài khoản!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var config = new CauHinhDTO
            {
                MaCH = MainWindow.CurrentCH,
                BankId = cboBank.SelectedValue.ToString() ?? "",
                AccountNo = txtAccountNo.Text.Trim(),
                AccountName = txtAccountName.Text.Trim().ToUpper()
            };

            if (bus.SaveCauHinh(config))
                MessageBox.Show("Đã lưu cấu hình thanh toán VietQR!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("Lỗi khi lưu cấu hình!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
