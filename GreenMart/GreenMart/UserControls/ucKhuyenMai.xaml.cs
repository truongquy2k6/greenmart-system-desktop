using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using BUS;

namespace GreenMart.UserControls
{
    public partial class ucKhuyenMai : UserControl
    {
        KhuyenMaiBUS bus = new();
        bool isEdit;
        DataTable filteredData = new DataTable();
        int currentPage = 1, pageSize = 10, totalPages = 1;

        public ucKhuyenMai()
        {
            InitializeComponent();
            fboLoai.SelectedIndex = 0;
            fboStatus.SelectedIndex = 0;
            LoadData();
        }

        DataTable allData = new DataTable();
        void LoadData() { allData = bus.HienThi(); ApplyFilter(); }

        void ApplyFilter()
        {
            if (allData == null || allData.Rows.Count == 0) return;

            string kw = txtSearch.Text.ToLower().Trim();
            string loai = (fboLoai.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Tất cả loại";
            string tt = (fboStatus.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Tất cả trạng thái";

            var filtered = allData.AsEnumerable().Where(r => 
            {
                bool matchKW = string.IsNullOrEmpty(kw) || 
                               r["MaKM"].ToString()!.ToLower().Contains(kw) || 
                               r["TenKM"].ToString()!.ToLower().Contains(kw);
                
                bool matchLoai = loai == "Tất cả loại" || r["LoaiKM"].ToString() == loai;
                bool matchTT = tt == "Tất cả trạng thái" || r["TrangThai"].ToString() == tt;

                return matchKW && matchLoai && matchTT;
            });

            filteredData = filtered.Any() ? filtered.CopyToDataTable() : allData.Clone();
            currentPage = 1;
            UpdateGrid();
        }

        void UpdateGrid()
        {
            if (filteredData.Rows.Count == 0) { dg.ItemsSource = null; txtPage.Text = "0 / 0"; return; }
            totalPages = (int)Math.Ceiling((double)filteredData.Rows.Count / pageSize);
            if (currentPage > totalPages) currentPage = totalPages;
            if (currentPage < 1) currentPage = 1;
            txtPage.Text = $"{currentPage} / {totalPages}";
            var paginated = filteredData.AsEnumerable().Skip((currentPage - 1) * pageSize).Take(pageSize);
            dg.ItemsSource = paginated.Any() ? paginated.CopyToDataTable().DefaultView : null;
        }

        void btnPrev_Click(object s, RoutedEventArgs e) { if (currentPage > 1) { currentPage--; UpdateGrid(); } }
        void btnNext_Click(object s, RoutedEventArgs e) { if (currentPage < totalPages) { currentPage++; UpdateGrid(); } }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        void btnTim_Click(object s, RoutedEventArgs e) => ApplyFilter();
        void Filter_Changed(object s, SelectionChangedEventArgs e) => ApplyFilter();

        void btnThem_Click(object s, RoutedEventArgs e)
        {
            isEdit = false;
            tft.Text = "Thêm KM";
            txtMa.Text = bus.TaoMa();
            txtTen.Text = txtGT.Text = txtDK.Text = txtMoTa.Text = "";
            dpBD.SelectedDate = DateTime.Now;
            dpKT.SelectedDate = DateTime.Now.AddDays(7);
            cboLoai.SelectedIndex = 0;
            cboTrangThai.SelectedIndex = 0;
            fp.Visibility = Visibility.Visible;
        }

        void btnSua_Click(object s, RoutedEventArgs e)
        {
            var r = (DataRowView?)((Button)s).DataContext;
            if (r == null) return;
            isEdit = true;
            tft.Text = "Sửa KM";
            txtMa.Text = r["MaKM"].ToString()!.Trim();
            txtTen.Text = r["TenKM"].ToString()!.Trim();
            txtGT.Text = r["GiaTri"].ToString();
            txtDK.Text = r["DieuKien"].ToString();
            txtMoTa.Text = r["MoTa"].ToString();
            dpBD.SelectedDate = Convert.ToDateTime(r["NgayBatDau"]);
            dpKT.SelectedDate = Convert.ToDateTime(r["NgayKetThuc"]);
            
            foreach (ComboBoxItem item in cboLoai.Items)
                if (item.Content.ToString() == r["LoaiKM"].ToString()) { cboLoai.SelectedItem = item; break; }
            
            foreach (ComboBoxItem item in cboTrangThai.Items)
                if (item.Content.ToString() == r["TrangThai"].ToString()) { cboTrangThai.SelectedItem = item; break; }

            fp.Visibility = Visibility.Visible;
        }

        void btnXoa_Click(object s, RoutedEventArgs e)
        {
            var r = (DataRowView?)((Button)s).DataContext;
            if (r == null) return;
            string ma = r["MaKM"].ToString()!.Trim();
            string ten = r["TenKM"].ToString()!.Trim();

            if (MessageBox.Show($"Xóa khuyến mãi '{ten}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    string oldData = $"Tên: {ten}, Loại: {r["LoaiKM"]}, Giá trị: {r["GiaTri"]}";
                    bus.Xoa(ma);
                    new LichSuBUS().GhiNhanChinhSua(MainWindow.CurrentNV, "KhuyenMai", "DELETE", ma, oldData, "Đã xóa");
                    LoadData();
                    MessageBox.Show("Đã xóa khuyến mãi thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("REFERENCE constraint") || ex.Message.Contains("FOREIGN KEY"))
                        MessageBox.Show("Không thể xóa khuyến mãi này vì nó đã được áp dụng cho các hóa đơn.\nBạn chỉ có thể cập nhật thông tin hoặc xóa các hóa đơn liên quan trước!", "Không thể xóa", MessageBoxButton.OK, MessageBoxImage.Warning);
                    else
                        MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        void btnLuu_Click(object s, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtTen.Text) || string.IsNullOrWhiteSpace(txtGT.Text) || string.IsNullOrWhiteSpace(txtDK.Text))
                {
                    MessageBox.Show("Vui lòng điền đầy đủ Tên chương trình, Giá trị giảm và Đơn tối thiểu!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!decimal.TryParse(txtGT.Text, out decimal gt)) { MessageBox.Show("Giá trị giảm không hợp lệ!", "Thông báo"); return; }
                if (!decimal.TryParse(txtDK.Text, out decimal dk)) { MessageBox.Show("Đơn tối thiểu không hợp lệ!", "Thông báo"); return; }

                string ma = txtMa.Text.Trim();
                string ten = txtTen.Text.Trim();
                string moTa = txtMoTa.Text.Trim();
                string loai = (cboLoai.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
                string trangThai = (cboTrangThai.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Đang áp dụng";
                DateTime bd = dpBD.SelectedDate ?? DateTime.Now;
                DateTime kt = dpKT.SelectedDate ?? DateTime.Now;

                if (kt < bd) { MessageBox.Show("Ngày kết thúc phải sau ngày bắt đầu!", "Thông báo"); return; }

                if (isEdit)
                {
                    var r = (DataRowView)dg.SelectedItem;
                    string oldData = $"Tên: {r["TenKM"]}, Loại: {r["LoaiKM"]}, GT: {r["GiaTri"]}";
                    string newData = $"Tên: {ten}, Loại: {loai}, GT: {gt}";

                    bus.CapNhat(ma, ten, moTa, loai, gt, bd, kt, dk, trangThai);
                    new LichSuBUS().GhiNhanChinhSua(MainWindow.CurrentNV, "KhuyenMai", "UPDATE", ma, oldData, newData);
                }
                else
                {
                    bus.Them(ma, ten, moTa, loai, gt, bd, kt, dk, trangThai);
                    new LichSuBUS().GhiNhanChinhSua(MainWindow.CurrentNV, "KhuyenMai", "INSERT", ma, "", $"Tên: {ten}, Loại: {loai}, GT: {gt}");
                }
                LoadData();
                fp.Visibility = Visibility.Collapsed;
                MessageBox.Show(isEdit ? "Cập nhật khuyến mãi thành công!" : "Thêm mới khuyến mãi thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        void btnHuy_Click(object s, RoutedEventArgs e) => fp.Visibility = Visibility.Collapsed;
    }
}
