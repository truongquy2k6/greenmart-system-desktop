using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Linq;
using BUS;

namespace GreenMart.UserControls
{
    public partial class ucKhoHang : UserControl
    {
        KhoBUS bus = new();
        CuaHangBUS busCH = new();
        DataTable allData = new DataTable();
        int currentPage = 1, pageSize = 10, totalPages = 1;
        bool isEdit;

        public ucKhoHang()
        {
            InitializeComponent();
            LoadData();
            cboCH.ItemsSource = busCH.HienThi().DefaultView;
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
                    r["TenKho"].ToString()!.ToLower().Contains(kw) || 
                    r["DiaChi"].ToString()!.ToLower().Contains(kw) ||
                    r["MaKho"].ToString()!.ToLower().Contains(kw) ||
                    r["SoDienThoai"].ToString()!.Contains(kw)
                );
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

        void btnThem_Click(object s, RoutedEventArgs e)
        {
            isEdit = false;
            tft.Text = "Thêm kho hàng";
            txtMa.Text = bus.TaoMa();
            txtTen.Text = txtDC.Text = txtSDT.Text = "";
            cboCH.SelectedIndex = -1;
            fp.Visibility = Visibility.Visible;
        }

        void btnSua_Click(object s, RoutedEventArgs e)
        {
            var r = (DataRowView?)((System.Windows.Controls.Button)s).DataContext;
            if (r == null) return;
            isEdit = true;
            tft.Text = "Cập nhật kho hàng";
            txtMa.Text = r["MaKho"].ToString()!.Trim();
            txtTen.Text = r["TenKho"].ToString()!.Trim();
            txtDC.Text = r["DiaChi"].ToString()!.Trim();
            txtSDT.Text = r["SoDienThoai"].ToString()!.Trim();
            cboCH.SelectedValue = r["MaCH"];
            fp.Visibility = Visibility.Visible;
        }

        void btnXoa_Click(object s, RoutedEventArgs e)
        {
            var r = (DataRowView?)((System.Windows.Controls.Button)s).DataContext;
            if (r == null) return;
            string ma = r["MaKho"].ToString()!.Trim();
            string ten = r["TenKho"].ToString()!.Trim();
            if (MessageBox.Show($"Xóa kho '{ten}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    string oldData = $"Mã: {ma}, Tên: {ten}, ĐC: {r["DiaChi"]}, SĐT: {r["SoDienThoai"]}, CH: {r["TenCH"]}";
                    bus.Xoa(ma);
                    new LichSuBUS().GhiNhanChinhSua(MainWindow.CurrentNV, "Kho", "DELETE", ma, oldData, "Đã xóa");
                    LoadData();
                    MessageBox.Show("Đã xóa kho hàng thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("hàng tồn kho"))
                        MessageBox.Show(ex.Message, "Lỗi xóa", MessageBoxButton.OK, MessageBoxImage.Warning);
                    else if (ex.Message.Contains("REFERENCE constraint") || ex.Message.Contains("FOREIGN KEY"))
                        MessageBox.Show("Không thể xóa kho này vì đang có dữ liệu liên quan khác!", "Lỗi xóa", MessageBoxButton.OK, MessageBoxImage.Warning);
                    else
                        MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        void btnLuu_Click(object s, RoutedEventArgs e)
        {
            try
            {
                string ma = txtMa.Text.Trim();
                string ten = txtTen.Text.Trim();
                string dc = txtDC.Text.Trim();
                string sdt = txtSDT.Text.Trim();
                string ch = cboCH.SelectedValue?.ToString() ?? "";
                string tenCH = cboCH.Text;

                if (string.IsNullOrWhiteSpace(ten) || string.IsNullOrWhiteSpace(dc) || string.IsNullOrWhiteSpace(sdt) || string.IsNullOrWhiteSpace(ch))
                {
                    MessageBox.Show("Vui lòng điền đầy đủ tất cả các thông tin và chọn Cửa hàng!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (sdt.Length != 10)
                {
                    MessageBox.Show("Số điện thoại phải bao gồm đúng 10 chữ số!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (isEdit)
                {
                    var r = (DataRowView)dg.SelectedItem;
                    string oldData = $"Tên: {r["TenKho"]}, ĐC: {r["DiaChi"]}, SĐT: {r["SoDienThoai"]}, CH: {r["TenCH"]}";
                    string newData = $"Tên: {ten}, ĐC: {dc}, SĐT: {sdt}, CH: {tenCH}";
                    
                    bus.CapNhat(ma, ten, dc, sdt, ch);
                    new LichSuBUS().GhiNhanChinhSua(MainWindow.CurrentNV, "Kho", "UPDATE", ma, oldData, newData);
                }
                else
                {
                    bus.Them(ma, ten, dc, sdt, ch);
                    new LichSuBUS().GhiNhanChinhSua(MainWindow.CurrentNV, "Kho", "INSERT", ma, "", $"Tên: {ten}, ĐC: {dc}, SĐT: {sdt}, CH: {tenCH}");
                }
                LoadData();
                fp.Visibility = Visibility.Collapsed;
                MessageBox.Show(isEdit ? "Cập nhật kho hàng thành công!" : "Thêm mới kho hàng thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        void btnHuy_Click(object s, RoutedEventArgs e) => fp.Visibility = Visibility.Collapsed;
    }
}
