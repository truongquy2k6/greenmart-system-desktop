using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using BUS;

namespace GreenMart.UserControls
{
    public partial class ucCuaHang : UserControl
    {
        CuaHangBUS bus = new();
        bool isEdit;
        DataTable filteredData = new DataTable();
        int currentPage = 1, pageSize = 10, totalPages = 1;

        public ucCuaHang()
        {
            InitializeComponent();
            LoadData();
        }

        DataTable allData = new DataTable();

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        void LoadData() { allData = bus.HienThi(); ApplyFilter(); }

        void ApplyFilter()
        {
            if (allData == null || allData.Rows.Count == 0) return;

            string kw = txtSearch.Text.ToLower().Trim();
            var filtered = allData.AsEnumerable().Where(r => 
                r["MaCH"].ToString()!.ToLower().Contains(kw) ||
                r["TenCH"].ToString()!.ToLower().Contains(kw) ||
                r["SoDienThoai"].ToString()!.Contains(kw)
            );

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

        void btnTim_Click(object s, RoutedEventArgs e) => ApplyFilter();

        void btnThem_Click(object s, RoutedEventArgs e)
        {
            isEdit = false;
            tft.Text = "Thêm CH";
            txtMa.Text = bus.TaoMa();
            txtTen.Text = txtDC.Text = txtSDT.Text = txtEmail.Text = "";
            fp.Visibility = Visibility.Visible;
        }

        void btnSua_Click(object s, RoutedEventArgs e)
        {
            var r = (DataRowView?)((Button)s).DataContext;
            if (r == null) return;
            isEdit = true;
            tft.Text = "Sửa CH";
            txtMa.Text = r["MaCH"].ToString()!.Trim();
            txtTen.Text = r["TenCH"].ToString()!.Trim();
            txtDC.Text = r["DiaChi"].ToString()!.Trim();
            txtSDT.Text = r["SoDienThoai"].ToString()!.Trim();
            txtEmail.Text = r["Email"].ToString()!.Trim();
            fp.Visibility = Visibility.Visible;
        }

        void btnXoa_Click(object s, RoutedEventArgs e)
        {
            var r = (DataRowView?)((Button)s).DataContext;
            if (r == null) return;
            string ma = r["MaCH"].ToString()!.Trim();
            string ten = r["TenCH"].ToString()!.Trim();

            if (MessageBox.Show($"Xóa cửa hàng '{ten}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    string oldData = $"Tên: {ten}, ĐC: {r["DiaChi"]}, SĐT: {r["SoDienThoai"]}, Email: {r["Email"]}";
                    bus.Xoa(ma);
                    new LichSuBUS().GhiNhanChinhSua(MainWindow.CurrentNV, "CuaHang", "DELETE", ma, oldData, "Đã xóa");
                    LoadData();
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("REFERENCE constraint") || ex.Message.Contains("FOREIGN KEY"))
                        MessageBox.Show("Không thể xóa Cửa hàng này vì đang có dữ liệu liên quan (Nhân viên, Kho hàng...).\nVui lòng xóa các dữ liệu liên quan trước!", "Lỗi xóa", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                string email = txtEmail.Text.Trim();

                if (string.IsNullOrWhiteSpace(ten) || string.IsNullOrWhiteSpace(dc) || string.IsNullOrWhiteSpace(sdt) || string.IsNullOrWhiteSpace(email))
                {
                    MessageBox.Show("Vui lòng điền đầy đủ tất cả các thông tin!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (sdt.Length != 10)
                {
                    MessageBox.Show("Số điện thoại phải bao gồm đúng 10 chữ số!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!email.ToLower().EndsWith("@gmail.com"))
                {
                    MessageBox.Show("Email phải có định dạng @gmail.com!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (isEdit)
                {
                    var r = (DataRowView)dg.SelectedItem;
                    string oldData = $"Tên: {r["TenCH"]}, ĐC: {r["DiaChi"]}, SĐT: {r["SoDienThoai"]}, Email: {r["Email"]}";
                    string newData = $"Tên: {ten}, ĐC: {dc}, SĐT: {sdt}, Email: {email}";

                    bus.CapNhat(ma, ten, dc, sdt, email);
                    new LichSuBUS().GhiNhanChinhSua(MainWindow.CurrentNV, "CuaHang", "UPDATE", ma, oldData, newData);
                }
                else
                {
                    bus.Them(ma, ten, dc, sdt, email);
                    new LichSuBUS().GhiNhanChinhSua(MainWindow.CurrentNV, "CuaHang", "INSERT", ma, "", $"Tên: {ten}, ĐC: {dc}, SĐT: {sdt}, Email: {email}");
                }
                LoadData();
                fp.Visibility = Visibility.Collapsed;
                MessageBox.Show(isEdit ? "Cập nhật cửa hàng thành công!" : "Thêm mới cửa hàng thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        void btnHuy_Click(object s, RoutedEventArgs e) => fp.Visibility = Visibility.Collapsed;
    }
}
