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
    public partial class ucSanPham : UserControl
    {
        SanPhamBUS bus = new();
        LoaiSanPhamBUS busLoai = new();
        NhaCungCapBUS busNCC = new();
        KhoBUS busKho = new();
        DataTable allData = new DataTable();
        int currentPage = 1, pageSize = 10, totalPages = 1;
        bool isEdit = false;

        public ucSanPham()
        {
            InitializeComponent();
            LoadData();
            cboLoai.ItemsSource = busLoai.HienThi().DefaultView;
            cboNCC.ItemsSource = busNCC.HienThi().DefaultView;
        }

        void btnTim_Click(object s, RoutedEventArgs e) => LoadData();

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        void LoadData()
        {
            allData = bus.HienThi();
            string kw = txtSearch.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(kw))
            {
                var filtered = allData.AsEnumerable().Where(r => 
                    r["TenSP"].ToString()!.ToLower().Contains(kw) || 
                    r["MaSP"].ToString()!.ToLower().Contains(kw) ||
                    r["TenLoai"].ToString()!.ToLower().Contains(kw) ||
                    r["TenNCC"].ToString()!.ToLower().Contains(kw));
                if (filtered.Any()) allData = filtered.CopyToDataTable();
                else allData = allData.Clone();
            }
            currentPage = 1;
            UpdateGrid();
        }

        void UpdateGrid()
        {
            if (allData.Rows.Count == 0) { dg.ItemsSource = null; txtPage.Text = "0 / 0"; return; }
            totalPages = (int)Math.Ceiling((double)allData.Rows.Count / pageSize);
            if (currentPage > totalPages) currentPage = totalPages;
            if (currentPage < 1) currentPage = 1;
            txtPage.Text = $"{currentPage} / {totalPages}";
            var paginated = allData.AsEnumerable().Skip((currentPage - 1) * pageSize).Take(pageSize);
            dg.ItemsSource = paginated.Any() ? paginated.CopyToDataTable().DefaultView : null;
        }
        void btnPrev_Click(object s, RoutedEventArgs e) { if (currentPage > 1) { currentPage--; UpdateGrid(); } }
        void btnNext_Click(object s, RoutedEventArgs e) { if (currentPage < totalPages) { currentPage++; UpdateGrid(); } }

        void btnView_Click(object s, RoutedEventArgs e)
        {
            var r = (DataRowView?)((Button)s).DataContext;
            if (r == null) return;
            
            // Điền dữ liệu vào popup
            pMa.Text = r["MaSP"].ToString();
            pTen.Text = r["TenSP"].ToString();
            pGia.Text = string.Format("{0:#,##0} đ", r["DonGia"]);
            pDVT.Text = r["DonViTinh"].ToString();
            pLoai.Text = r["TenLoai"].ToString();
            pNCC.Text = r["TenNCC"].ToString();
            string status = r["TrangThai"].ToString()!;
            pStatus.Text = status;
            if (status == "Ngừng kinh doanh")
            {
                ((Border)pStatus.Parent).Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FFEBEE"));
                pStatus.Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#C62828"));
            }
            else
            {
                ((Border)pStatus.Parent).Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#E8F5E9"));
                pStatus.Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2E7D32"));
            }
            pMoTa.Text = string.IsNullOrEmpty(r["MoTa"].ToString()) ? "Không có mô tả." : r["MoTa"].ToString();

            // Tính tổng tồn
            try
            {
                DataTable dt = busKho.HienThiTatCaChiTiet();
                var ton = dt.AsEnumerable().Where(x => x["MaSP"].ToString() == r["MaSP"].ToString()).Sum(x => Convert.ToInt64(x["SoLuong"]));
                pTon.Text = $"Tổng tồn: {ton}";
            }
            catch { pTon.Text = "Tổng tồn: N/A"; }

            popDetail.Visibility = Visibility.Visible;
        }

        void btnHuyDetail_Click(object s, RoutedEventArgs e) => popDetail.Visibility = Visibility.Collapsed;

        void btnThem_Click(object s, RoutedEventArgs e)
        {
            isEdit = false;
            tft.Text = "Thêm sản phẩm";
            txtMa.Text = bus.TaoMa();
            txtTen.Text = txtGia.Text = txtDVT.Text = txtMoTa.Text = "";
            cboLoai.SelectedIndex = cboNCC.SelectedIndex = -1;
            cboTrangThai.SelectedIndex = 0;
            fp.Visibility = Visibility.Visible;
        }

        void btnSua_Click(object s, RoutedEventArgs e)
        {
            var r = (DataRowView?)((Button)s).DataContext;
            if (r == null) return;
            isEdit = true;
            tft.Text = "Sửa sản phẩm";
            txtMa.Text = r["MaSP"].ToString()!.Trim();
            txtTen.Text = r["TenSP"].ToString()!.Trim();
            txtGia.Text = r["DonGia"].ToString();
            txtDVT.Text = r["DonViTinh"].ToString()!.Trim();
            txtMoTa.Text = r["MoTa"].ToString();
            cboLoai.SelectedValue = r["MaLoai"];
            cboNCC.SelectedValue = r["MaNCC"];
            
            foreach (ComboBoxItem item in cboTrangThai.Items)
                if (item.Content.ToString() == r["TrangThai"].ToString()) { cboTrangThai.SelectedItem = item; break; }

            fp.Visibility = Visibility.Visible;
        }

        void btnXoa_Click(object s, RoutedEventArgs e)
        {
            var r = (DataRowView?)((Button)s).DataContext;
            if (r == null) return;
            string ma = r["MaSP"].ToString()!.Trim();
            string ten = r["TenSP"].ToString()!.Trim();
            if (MessageBox.Show($"Xóa sản phẩm '{ten}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    string oldData = $"Tên: {ten}, Giá: {r["DonGia"]}, Loại: {r["TenLoai"]}";
                    bus.Xoa(ma);
                    new LichSuBUS().GhiNhanChinhSua(MainWindow.CurrentNV, "SanPham", "DELETE", ma, oldData, "Đã xóa");
                    LoadData();
                    MessageBox.Show("Đã xóa sản phẩm thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("REFERENCE constraint") || ex.Message.Contains("FOREIGN KEY"))
                        MessageBox.Show("Không thể xóa sản phẩm này vì đã có trong hóa đơn hoặc kho hàng.\nHãy chuyển trạng thái sang 'Ngừng kinh doanh'!", "Lỗi xóa", MessageBoxButton.OK, MessageBoxImage.Warning);
                    else
                        MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        void btnLuu_Click(object s, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtTen.Text) || 
                    string.IsNullOrWhiteSpace(txtGia.Text) || 
                    string.IsNullOrWhiteSpace(txtDVT.Text) || 
                    string.IsNullOrWhiteSpace(txtMoTa.Text) || 
                    cboLoai.SelectedValue == null || 
                    cboNCC.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng điền đầy đủ tất cả các thông tin sản phẩm!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!decimal.TryParse(txtGia.Text, out decimal gia) || gia < 0)
                {
                    MessageBox.Show("Đơn giá phải là số hợp lệ và lớn hơn hoặc bằng 0!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string ma = txtMa.Text.Trim();
                string ten = txtTen.Text.Trim();
                string dvt = txtDVT.Text.Trim();
                string loai = cboLoai.SelectedValue.ToString()!;
                string ncc = cboNCC.SelectedValue.ToString()!;
                string tt = (cboTrangThai.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Đang kinh doanh";
                string mt = txtMoTa.Text.Trim();

                if (isEdit)
                {
                    var r = (DataRowView)dg.SelectedItem;
                    string oldData = $"Tên: {r["TenSP"]}, Giá: {r["DonGia"]}, TT: {r["TrangThai"]}";
                    string newData = $"Tên: {ten}, Giá: {gia}, TT: {tt}";
                    
                    bus.CapNhat(ma, ten, gia, dvt, "", mt, ncc, loai, tt);
                    new LichSuBUS().GhiNhanChinhSua(MainWindow.CurrentNV, "SanPham", "UPDATE", ma, oldData, newData);
                }
                else
                {
                    bus.Them(ma, ten, gia, dvt, "", mt, ncc, loai, tt);
                    new LichSuBUS().GhiNhanChinhSua(MainWindow.CurrentNV, "SanPham", "INSERT", ma, "", $"Tên: {ten}, Giá: {gia}");
                }
                LoadData();
                fp.Visibility = Visibility.Collapsed;
                MessageBox.Show(isEdit ? "Cập nhật sản phẩm thành công!" : "Thêm mới sản phẩm thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        void btnHuy_Click(object s, RoutedEventArgs e) => fp.Visibility = Visibility.Collapsed;
    }
}
