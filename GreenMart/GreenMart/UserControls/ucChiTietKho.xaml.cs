using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using BUS;

namespace GreenMart.UserControls
{
    public partial class ucChiTietKho : UserControl
    {
        KhoBUS bus = new();
        SanPhamBUS busSP = new();
        DataTable allData = new DataTable();
        int currentPage = 1, pageSize = 10, totalPages = 1;
        bool isEdit = false;

        public ucChiTietKho()
        {
            InitializeComponent();
            LoadKho();
            cboSP.ItemsSource = busSP.HienThi().DefaultView;
            LoadAll();
        }

        void LoadKho()
        {
            var dtKho = bus.HienThi();
            cboKho.ItemsSource = dtKho.DefaultView;
            cboKhoNhap.ItemsSource = dtKho.DefaultView;
        }

        void LoadAll()
        {
            allData = bus.HienThiTatCaChiTiet();
            currentPage = 1;
            UpdateGrid();
        }

        void LoadByKho(string maKho)
        {
            allData = bus.ChiTiet(maKho);
            currentPage = 1;
            UpdateGrid();
        }

        void UpdateGrid()
        {
            DataTable displayData = allData;
            string kw = txtSearch.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(kw))
            {
                var filtered = allData.AsEnumerable().Where(r =>
                    r["TenSP"].ToString()!.ToLower().Contains(kw) ||
                    r["MaSP"].ToString()!.ToLower().Contains(kw) ||
                    (allData.Columns.Contains("TenKho") && r["TenKho"].ToString()!.ToLower().Contains(kw)));
                if (filtered.Any()) displayData = filtered.CopyToDataTable();
                else { dg.ItemsSource = null; txtPage.Text = "0 / 0"; txtTongSP.Text = "0 loại SP"; txtTongSL.Text = "0 đơn vị"; return; }
            }

            if (displayData.Rows.Count == 0) { dg.ItemsSource = null; txtPage.Text = "0 / 0"; txtTongSP.Text = "0 loại SP"; txtTongSL.Text = "0 đơn vị"; return; }

            // Thống kê
            txtTongSP.Text = $"{displayData.Rows.Count} loại SP";
            long tongSL = displayData.AsEnumerable().Sum(r => { long.TryParse(r["SoLuong"].ToString(), out long v); return v; });
            txtTongSL.Text = $"{tongSL:N0} đơn vị";

            totalPages = (int)Math.Ceiling((double)displayData.Rows.Count / pageSize);
            if (currentPage > totalPages) currentPage = totalPages;
            if (currentPage < 1) currentPage = 1;
            txtPage.Text = $"{currentPage} / {totalPages}";
            var paginated = displayData.AsEnumerable().Skip((currentPage - 1) * pageSize).Take(pageSize);
            dg.ItemsSource = paginated.Any() ? paginated.CopyToDataTable().DefaultView : null;
        }

        void cboKho_Changed(object s, SelectionChangedEventArgs e)
        {
            var item = cboKho.SelectedItem as DataRowView;
            if (item == null) { LoadAll(); return; }
            LoadByKho(item["MaKho"].ToString()!.Trim());
        }

        void btnTim_Click(object s, RoutedEventArgs e) { currentPage = 1; UpdateGrid(); }
        void txtSearch_KeyDown(object s, KeyEventArgs e) { if (e.Key == Key.Enter) { currentPage = 1; UpdateGrid(); } }
        void btnPrev_Click(object s, RoutedEventArgs e) { if (currentPage > 1) { currentPage--; UpdateGrid(); } }
        void btnNext_Click(object s, RoutedEventArgs e) { if (currentPage < totalPages) { currentPage++; UpdateGrid(); } }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        void btnDongBo1Kho_Click(object s, RoutedEventArgs e)
        {
            if (cboKho.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn một kho cụ thể ở danh sách thả xuống trước khi đồng bộ!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string maKho = cboKho.SelectedValue.ToString()!;
            string tenKho = cboKho.Text;

            if (MessageBox.Show($"Bạn có muốn tự động đồng bộ tất cả sản phẩm vào kho '{tenKho}' (với số lượng bằng 0 nếu chưa có) không?", "Xác nhận đồng bộ", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    var parameters = new Microsoft.Data.SqlClient.SqlParameter[]
                    {
                        new("@MaKho", maKho)
                    };
                    DAL.DatabaseHelper.ExecuteNonQuery("sp_DongBo1Kho", parameters);
                    LoadByKho(maKho);
                    MessageBox.Show($"Đã đồng bộ sản phẩm vào kho '{tenKho}' thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Lỗi đồng bộ", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        void btnNhapHang_Click(object s, RoutedEventArgs e)
        {
            isEdit = false;
            txtTitleNhap.Text = "Nhập hàng:";
            fpNhap.Visibility = fpNhap.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            cboKhoNhap.IsEnabled = true;
            cboSP.IsEnabled = true;
            cboKhoNhap.SelectedIndex = -1;
            cboSP.SelectedIndex = -1;
            txtSL.Text = "";
        }

        void btnHuyNhap_Click(object s, RoutedEventArgs e) => fpNhap.Visibility = Visibility.Collapsed;

        void btnLuuNhap_Click(object s, RoutedEventArgs e)
        {
            string maKho = cboKhoNhap.SelectedValue?.ToString() ?? "";
            string maSP = cboSP.SelectedValue?.ToString() ?? "";
            string tenKho = cboKhoNhap.Text;
            string tenSP = cboSP.Text;

            if (string.IsNullOrWhiteSpace(maKho) || string.IsNullOrWhiteSpace(maSP) || string.IsNullOrWhiteSpace(txtSL.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin kho, sản phẩm và số lượng!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtSL.Text, out int sl) || sl <= 0)
            {
                MessageBox.Show("Số lượng nhập phải là số nguyên dương lớn hơn 0!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                if (isEdit)
                {
                    var row = (DataRowView)dg.SelectedItem;
                    string oldSL = row["SoLuong"].ToString();
                    bus.CapNhatChiTiet(maKho, maSP, sl);
                    new LichSuBUS().GhiNhanChinhSua(MainWindow.CurrentNV, "ChiTietKho", "UPDATE", $"{maKho}-{maSP}", $"Kho: {tenKho}, SP: {tenSP}, SL: {oldSL}", $"SL mới: {sl}");
                }
                else
                {
                    bus.ThemChiTiet(maKho, maSP, sl);
                    new LichSuBUS().GhiNhanChinhSua(MainWindow.CurrentNV, "ChiTietKho", "INSERT", $"{maKho}-{maSP}", "", $"Nhập: Kho: {tenKho}, SP: {tenSP}, SL: {sl}");
                }
                fpNhap.Visibility = Visibility.Collapsed;
                txtSL.Text = "";
                // Reload
                var khoSelected = cboKho.SelectedItem as DataRowView;
                if (khoSelected != null) LoadByKho(khoSelected["MaKho"].ToString()!.Trim());
                else LoadAll();
                MessageBox.Show(isEdit ? "Đã cập nhật số lượng thành công!" : "Đã nhập hàng thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        void btnSuaCT_Click(object s, RoutedEventArgs e)
        {
            var row = ((Button)s).DataContext as DataRowView;
            if (row == null) return;
            isEdit = true;
            txtTitleNhap.Text = "Cập nhật SL:";
            fpNhap.Visibility = Visibility.Visible;
            
            cboKhoNhap.SelectedValue = row["MaKho"];
            cboSP.SelectedValue = row["MaSP"];
            txtSL.Text = row["SoLuong"].ToString();
            
            cboKhoNhap.IsEnabled = false;
            cboSP.IsEnabled = false;
        }

        void btnXoaCT_Click(object s, RoutedEventArgs e)
        {
            var row = ((Button)s).DataContext as DataRowView;
            if (row == null) return;
            string maSP = row["MaSP"].ToString()!.Trim();
            string tenSP = row["TenSP"].ToString()!.Trim();
            string maKho = row["MaKho"].ToString()!.Trim();
            string tenKho = row["TenKho"].ToString()!.Trim();
            string sl = row["SoLuong"].ToString();

            if (MessageBox.Show($"Xóa '{tenSP}' khỏi kho?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    bus.XoaChiTiet(maKho, maSP);
                    new LichSuBUS().GhiNhanChinhSua(MainWindow.CurrentNV, "ChiTietKho", "DELETE", $"{maKho}-{maSP}", $"Kho: {tenKho}, SP: {tenSP}, SL: {sl}", "Đã xóa");
                    
                    var khoSelected = cboKho.SelectedItem as DataRowView;
                    if (khoSelected != null) LoadByKho(khoSelected["MaKho"].ToString()!.Trim());
                    else LoadAll();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error); }
            }
        }
    }
}
