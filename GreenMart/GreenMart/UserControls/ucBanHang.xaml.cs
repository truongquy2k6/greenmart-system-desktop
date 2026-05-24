using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using BUS;

namespace GreenMart.UserControls
{
    public class CartItem : INotifyPropertyChanged
    {
        public string MaSP { get; set; } = "";
        public string TenSP { get; set; } = "";
        private int _soLuong;
        public int SoLuong { get { return _soLuong; } set { _soLuong = value; OnPropertyChanged(nameof(SoLuong)); OnPropertyChanged(nameof(ThanhTien)); } }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public partial class ucBanHang : UserControl
    {
        HoaDonBUS bus = new();
        ObservableCollection<CartItem> cart = new();
        string maKH = "";
        decimal tongTien = 0;

        public ucBanHang()
        {
            InitializeComponent();
            dgCart.ItemsSource = cart;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TimSP();
            txtSearch.Focus();
        }

        void btnTimSP_Click(object s, RoutedEventArgs e) => TimSP();
        void txtSearch_KeyDown(object s, KeyEventArgs e) { if (e.Key == Key.Enter) TimSP(); }
        void txtSearch_TextChanged(object s, TextChangedEventArgs e) { TimSP(); }
        void TimSP() => dgSP.ItemsSource = bus.TimSP(txtSearch.Text.Trim(), MainWindow.CurrentCH).DefaultView;

        void dgSP_DblClick(object s, MouseButtonEventArgs e) { AddToCart(); }
        void btnAddCart_Click(object s, RoutedEventArgs e) { AddToCart(); }

        void AddToCart()
        {
            var r = (DataRowView?)dgSP.SelectedItem;
            if (r == null) return;
            string ma = r["MaSP"].ToString()!.Trim();
            
            // Lấy tồn kho hiện tại
            int tonKho = Convert.ToInt32(r["TonKho"]);
            
            var exist = cart.FirstOrDefault(c => c.MaSP == ma);
            if (exist != null) 
            { 
                if(exist.SoLuong >= tonKho) { MessageBox.Show("Không đủ số lượng trong kho!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
                exist.SoLuong++; 
            }
            else 
            {
                if(tonKho <= 0) { MessageBox.Show("Sản phẩm đã hết hàng!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
                cart.Add(new CartItem { MaSP = ma, TenSP = r["TenSP"].ToString()!.Trim(), SoLuong = 1, DonGia = Convert.ToDecimal(r["DonGia"]) });
            }
            UpdateTotal();
        }

        void btnGiam_Click(object s, RoutedEventArgs e) { var item = (CartItem)((Button)s).DataContext; if (item.SoLuong > 1) item.SoLuong--; UpdateTotal(); }
        void btnTang_Click(object s, RoutedEventArgs e) 
        { 
            var item = (CartItem)((Button)s).DataContext; 
            
            // Check tồn kho (tùy chọn, nếu cần truy xuất DB lại hoặc tìm từ dgSP)
            foreach (DataRowView r in dgSP.ItemsSource)
            {
                if (r["MaSP"].ToString()!.Trim() == item.MaSP)
                {
                    int tonKho = Convert.ToInt32(r["TonKho"]);
                    if (item.SoLuong >= tonKho) { MessageBox.Show("Không đủ số lượng trong kho!"); return; }
                    break;
                }
            }
            item.SoLuong++; 
            UpdateTotal(); 
        }
        void btnXoaCart_Click(object s, RoutedEventArgs e) { var item = (CartItem)((Button)s).DataContext; cart.Remove(item); UpdateTotal(); }

        void UpdateTotal()
        {
            if (txtTong == null || txtCartCount == null || txtGiamGia == null) return;
            
            decimal sum = cart.Sum(c => c.ThanhTien);
            decimal giamGia = 0;
            decimal.TryParse(txtGiamGia.Text.Replace(",", ""), out giamGia);
            
            tongTien = Math.Max(0, sum - giamGia);
            txtTong.Text = string.Format("{0:#,##0} ₫", tongTien);
            txtCartCount.Text = $"({cart.Sum(c => c.SoLuong)})";
            TinhTienThua();
        }

        void txtTien_TextChanged(object s, TextChangedEventArgs e) { UpdateTotal(); }

        void TinhTienThua()
        {
            if (txtTienThua == null || txtKhachDua == null) return;
            decimal khachDua = 0;
            decimal.TryParse(txtKhachDua.Text.Replace(",", ""), out khachDua);
            decimal tienThua = khachDua - tongTien;
            txtTienThua.Text = string.Format("{0:#,##0} ₫", Math.Max(0, tienThua));
            txtTienThua.Foreground = tienThua < 0 ? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red) : new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(44, 62, 80));
        }

        void txtSDT_KeyDown(object s, KeyEventArgs e) { if (e.Key == Key.Enter) CheckKH(); }
        void btnTimKH_Click(object s, RoutedEventArgs e) { CheckKH(); }
        
        void CheckKH()
        {
            var dt = bus.TimKH(txtSDT.Text.Trim());
            if (dt.Rows.Count > 0) { maKH = dt.Rows[0]["MaKH"].ToString()!.Trim(); txtKH.Text = $"{dt.Rows[0]["HoTen"]} - Điểm: {dt.Rows[0]["DiemTichLuy"]}"; }
            else { maKH = ""; txtKH.Text = "Không tìm thấy → Khách vãng lai"; }
        }

        void btnThanhToan_Click(object s, RoutedEventArgs e)
        {
            if (cart.Count == 0) { MessageBox.Show("Giỏ hàng trống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            
            decimal khachDua = 0;
            decimal.TryParse(txtKhachDua.Text.Replace(",", ""), out khachDua);
            if (khachDua < tongTien && cboPTTT.SelectedIndex == 0) 
            { 
                MessageBox.Show("Khách đưa chưa đủ tiền!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning); 
                return; 
            }

            try
            {
                string maHD = bus.TaoMa();
                string pttt = (cboPTTT.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Tiền mặt";
                decimal giamGia = 0;
                decimal.TryParse(txtGiamGia.Text.Replace(",", ""), out giamGia);

                bus.ThemHoaDon(maHD, tongTien, maKH == "" ? null! : maKH, MainWindow.CurrentNV, null, giamGia, pttt);
                foreach (var item in cart)
                    bus.ThemChiTiet(maHD, item.MaSP, item.SoLuong, item.DonGia);
                
                MessageBox.Show($"Thanh toán thành công!\nMã HD: {maHD}\nTổng thu: {tongTien:#,##0} ₫\nTiền thừa: {Math.Max(0, khachDua - tongTien):#,##0} ₫", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Reset form
                cart.Clear();
                txtGiamGia.Text = "0";
                txtKhachDua.Text = "0";
                txtSDT.Text = "";
                CheckKH();
                UpdateTotal(); 
                TimSP();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi thanh toán: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
    }
}
