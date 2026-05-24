using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using BUS;
using GreenMart.Services;

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
        string currentImageUrl = "";
        string localImagePath = "";
        DataRowView? currentEditRow;

        public ucSanPham()
        {
            InitializeComponent();
            
            // Tải dữ liệu Loai
            DataTable dtLoai = busLoai.HienThi();
            DataRow drLoai = dtLoai.NewRow();
            drLoai["MaLoai"] = "";
            drLoai["TenLoai"] = "-- Tất cả danh mục --";
            dtLoai.Rows.InsertAt(drLoai, 0);
            cboFilterLoai.ItemsSource = dtLoai.DefaultView;
            cboFilterLoai.SelectedIndex = 0;

            // Tải dữ liệu NCC
            DataTable dtNCC = busNCC.HienThi();
            DataRow drNCC = dtNCC.NewRow();
            drNCC["MaNCC"] = "";
            drNCC["TenNCC"] = "-- Tất cả NCC --";
            dtNCC.Rows.InsertAt(drNCC, 0);
            cboFilterNCC.ItemsSource = dtNCC.DefaultView;
            cboFilterNCC.SelectedIndex = 0;

            // Lấy dữ liệu DVT từ danh sách SP hiện có
            DataTable dtAllSP = bus.HienThi();
            var dvtList = dtAllSP.AsEnumerable()
                                 .Select(r => r["DonViTinh"].ToString()?.Trim())
                                 .Where(d => !string.IsNullOrEmpty(d))
                                 .Distinct()
                                 .ToList();
            
            DataTable dtDVT = new DataTable();
            dtDVT.Columns.Add("DonViTinh");
            dtDVT.Rows.Add("-- Tất cả --");
            foreach(var dvt in dvtList)
            {
                dtDVT.Rows.Add(dvt);
            }
            cboFilterDVT.ItemsSource = dtDVT.DefaultView;
            cboFilterDVT.SelectedIndex = 0;

            LoadData();
            cboLoai.ItemsSource = busLoai.HienThi().DefaultView;
            cboNCC.ItemsSource = busNCC.HienThi().DefaultView;
        }

        void Filter_SelectionChanged(object s, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                currentPage = 1;
                LoadData();
            }
        }

        void btnTim_Click(object s, RoutedEventArgs e) 
        {
            currentPage = 1;
            LoadData();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        void LoadData()
        {
            string kw = txtSearch?.Text?.Trim() ?? "";
            string loai = cboFilterLoai?.SelectedValue?.ToString() ?? "";
            string ncc = cboFilterNCC?.SelectedValue?.ToString() ?? "";
            string dvt = cboFilterDVT?.SelectedValue?.ToString() ?? "";
            if (dvt == "-- Tất cả --") dvt = "";
            
            string trangThai = (cboFilterTrangThai?.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            if (trangThai == "-- Tất cả --") trangThai = "";

            allData = bus.LocNangCao(kw, loai, ncc, dvt, trangThai, currentPage, pageSize);

            UpdateGrid();
        }

        void UpdateGrid()
        {
            if (allData.Rows.Count == 0) { icProducts.ItemsSource = null; txtPage.Text = "0 / 0"; return; }
            
            int totalRows = Convert.ToInt32(allData.Rows[0]["TotalRows"]);
            totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
            
            if (currentPage > totalPages) currentPage = totalPages;
            if (currentPage < 1) currentPage = 1;
            txtPage.Text = $"{currentPage} / {totalPages}";
            
            icProducts.ItemsSource = allData.DefaultView;
        }
        void btnPrev_Click(object s, RoutedEventArgs e) { if (currentPage > 1) { currentPage--; LoadData(); } }
        void btnNext_Click(object s, RoutedEventArgs e) { if (currentPage < totalPages) { currentPage++; LoadData(); } }

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

            // Load HinhAnh
            string url = r["HinhAnh"]?.ToString();
            if (!string.IsNullOrEmpty(url) && Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                pImgDetail.Source = new BitmapImage(new Uri(url));
            }
            else
            {
                pImgDetail.Source = null;
            }

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
            currentEditRow = null;
            tft.Text = "Thêm sản phẩm";
            txtMa.Text = bus.TaoMa();
            txtTen.Text = txtGia.Text = txtDVT.Text = txtMoTa.Text = "";
            cboLoai.SelectedIndex = cboNCC.SelectedIndex = -1;
            cboTrangThai.SelectedIndex = 0;
            currentImageUrl = "";
            localImagePath = "";
            imgPreview.Source = null;
            txtTrangThaiAnh.Text = "";
            fp.Visibility = Visibility.Visible;
        }

        void btnSua_Click(object s, RoutedEventArgs e)
        {
            var r = (DataRowView?)((Button)s).DataContext;
            if (r == null) return;
            isEdit = true;
            currentEditRow = r;
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

            currentImageUrl = r["HinhAnh"]?.ToString();
            if (!string.IsNullOrEmpty(currentImageUrl) && Uri.IsWellFormedUriString(currentImageUrl, UriKind.Absolute))
            {
                imgPreview.Source = new BitmapImage(new Uri(currentImageUrl));
            }
            else
            {
                imgPreview.Source = null;
            }
            localImagePath = "";
            txtTrangThaiAnh.Text = "";
            fp.Visibility = Visibility.Visible;
        }

        async void btnXoa_Click(object s, RoutedEventArgs e)
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
                    string oldUrl = r["HinhAnh"]?.ToString() ?? "";
                    
                    bus.Xoa(ma);
                    new LichSuBUS().GhiNhanChinhSua(MainWindow.CurrentNV, "SanPham", "DELETE", ma, oldData, "Đã xóa");
                    
                    // Xóa ảnh trên Cloudinary
                    if (!string.IsNullOrEmpty(oldUrl))
                    {
                        await CloudinaryHelper.DeleteImageAsync(oldUrl);
                    }
                    
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

        async void btnLuu_Click(object s, RoutedEventArgs e)
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

                // Nếu có chọn ảnh mới từ máy tính, tiến hành upload trước khi lưu
                if (!string.IsNullOrEmpty(localImagePath))
                {
                    txtTrangThaiAnh.Text = "Đang tải ảnh và lưu...";
                    txtTrangThaiAnh.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.ColorConverter.ConvertFromString("#F39C12") as System.Windows.Media.Color? ?? System.Windows.Media.Colors.Orange);
                    
                    string url = await CloudinaryHelper.UploadImageAsync(localImagePath);
                    if (!string.IsNullOrEmpty(url))
                    {
                        currentImageUrl = url;
                        txtTrangThaiAnh.Text = "Tải ảnh thành công!";
                        txtTrangThaiAnh.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.ColorConverter.ConvertFromString("#2E7D32") as System.Windows.Media.Color? ?? System.Windows.Media.Colors.Green);
                    }
                    else
                    {
                        MessageBox.Show("Lỗi khi tải ảnh lên Cloudinary! Vẫn tiếp tục lưu dữ liệu không kèm ảnh mới.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    localImagePath = ""; // Reset
                }

                string ma = txtMa.Text.Trim();
                string ten = txtTen.Text.Trim();
                string dvt = txtDVT.Text.Trim();
                string loai = cboLoai.SelectedValue.ToString()!;
                string ncc = cboNCC.SelectedValue.ToString()!;
                string tt = (cboTrangThai.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Đang kinh doanh";
                string mt = txtMoTa.Text.Trim();

                if (isEdit && currentEditRow != null)
                {
                    var r = currentEditRow;
                    string oldData = $"Tên: {r["TenSP"]}, Giá: {r["DonGia"]}, TT: {r["TrangThai"]}";
                    string newData = $"Tên: {ten}, Giá: {gia}, TT: {tt}";
                    string oldUrl = r["HinhAnh"]?.ToString() ?? "";
                    
                    bus.CapNhat(ma, ten, gia, dvt, currentImageUrl, mt, ncc, loai, tt);
                    new LichSuBUS().GhiNhanChinhSua(MainWindow.CurrentNV, "SanPham", "UPDATE", ma, oldData, newData);
                    
                    // Xóa ảnh cũ trên Cloudinary nếu có chọn ảnh mới
                    if (!string.IsNullOrEmpty(oldUrl) && oldUrl != currentImageUrl)
                    {
                        await CloudinaryHelper.DeleteImageAsync(oldUrl);
                    }
                }
                else
                {
                    bus.Them(ma, ten, gia, dvt, currentImageUrl, mt, ncc, loai, tt);
                    new LichSuBUS().GhiNhanChinhSua(MainWindow.CurrentNV, "SanPham", "INSERT", ma, "", $"Tên: {ten}, Giá: {gia}");
                }
                LoadData();
                fp.Visibility = Visibility.Collapsed;
                MessageBox.Show(isEdit ? "Cập nhật sản phẩm thành công!" : "Thêm mới sản phẩm thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        void btnHuy_Click(object s, RoutedEventArgs e) => fp.Visibility = Visibility.Collapsed;

        void btnChonAnh_Click(object s, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    localImagePath = dlg.FileName;
                    imgPreview.Source = new BitmapImage(new Uri(localImagePath));
                    txtTrangThaiAnh.Text = "Đã chọn ảnh chờ lưu";
                    txtTrangThaiAnh.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.ColorConverter.ConvertFromString("#1976D2") as System.Windows.Media.Color? ?? System.Windows.Media.Colors.Blue);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi đọc ảnh: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
