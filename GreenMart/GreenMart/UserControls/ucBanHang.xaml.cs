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
        LoaiSanPhamBUS busLoai = new();
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
            var dtLoai = busLoai.HienThi();
            DataRow row = dtLoai.NewRow();
            row["MaLoai"] = "";
            row["TenLoai"] = "Tất cả danh mục";
            dtLoai.Rows.InsertAt(row, 0);
            cboLoaiSP.ItemsSource = dtLoai.DefaultView;
            cboLoaiSP.SelectedIndex = 0;

            TimSP();
            txtSearch.Focus();
        }

        void btnTimSP_Click(object s, RoutedEventArgs e) => TimSP();
        void txtSearch_KeyDown(object s, KeyEventArgs e) { if (e.Key == Key.Enter) TimSP(); }
        void txtSearch_TextChanged(object s, TextChangedEventArgs e) { TimSP(); }
        void TimSP() 
        { 
            string maLoai = cboLoaiSP?.SelectedValue?.ToString() ?? "";
            dgSP.ItemsSource = bus.TimSP(txtSearch.Text.Trim(), MainWindow.CurrentCH, maLoai).DefaultView; 
        }

        void cboLoaiSP_SelectionChanged(object s, SelectionChangedEventArgs e) { TimSP(); }

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
            string pttt = (cboPTTT.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Tiền mặt";

            if (khachDua < tongTien && pttt == "Tiền mặt") 
            { 
                MessageBox.Show("Khách đưa chưa đủ tiền!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning); 
                return; 
            }

            // VietQR Integration
            if (pttt == "Chuyển khoản")
            {
                var qrWin = new PaymentQRWindow(tongTien, $"Thanh toan mua hang GreenMart") { Owner = Window.GetWindow(this) };
                qrWin.ShowDialog();
                if (!qrWin.IsPaid) return; // Hủy thanh toán
                
                khachDua = tongTien; // Tự động ghi nhận khách đã chuyển đủ
            }

            try
            {
                string maHD = bus.TaoMa();
                decimal giamGia = 0;
                decimal.TryParse(txtGiamGia.Text.Replace(",", ""), out giamGia);

                bus.ThemHoaDon(maHD, tongTien, maKH == "" ? null! : maKH, MainWindow.CurrentNV, null, giamGia, pttt);
                foreach (var item in cart)
                    bus.ThemChiTiet(maHD, item.MaSP, item.SoLuong, item.DonGia);
                
                if (MessageBox.Show($"Thanh toán thành công!\nMã HD: {maHD}\nTổng thu: {tongTien:#,##0} ₫\nTiền thừa: {Math.Max(0, khachDua - tongTien):#,##0} ₫\n\nBạn có muốn in hóa đơn ra file PDF không?", "Thành công", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    // Convert cart to DataTable
                    DataTable dtChiTiet = new DataTable();
                    dtChiTiet.Columns.Add("TenSP", typeof(string));
                    dtChiTiet.Columns.Add("SoLuong", typeof(int));
                    dtChiTiet.Columns.Add("DonGia", typeof(decimal));
                    dtChiTiet.Columns.Add("ThanhTien", typeof(decimal));
                    foreach (var item in cart)
                        dtChiTiet.Rows.Add(item.TenSP, item.SoLuong, item.DonGia, item.ThanhTien);
                        
                    string tenKH = txtKH.Text.Split('-')[0].Trim();
                    var dtStore = bus.LayThongTinCuaHangTuHoaDon(maHD);
                    string storeName = dtStore.Rows.Count > 0 ? dtStore.Rows[0]["TenCH"].ToString()! : "GREEN MART";
                    string storeAddr = dtStore.Rows.Count > 0 ? dtStore.Rows[0]["DiaChi"].ToString()! : "123 Nguyễn Văn Linh, Q.7, TP.HCM";
                    string storePhone = dtStore.Rows.Count > 0 ? dtStore.Rows[0]["SoDienThoai"].ToString()! : "1900 1234";

                    GreenMart.Services.PdfHelper.ExportInvoiceToPdf(
                        maHD, MainWindow.CurrentNV, tenKH == "Không tìm thấy → Khách vãng lai" ? "" : tenKH, 
                        dtChiTiet, tongTien, giamGia, khachDua, Math.Max(0, khachDua - tongTien), DateTime.Now,
                        storeName, storeAddr, storePhone);
                }
                
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
