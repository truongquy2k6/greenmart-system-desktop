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
    public partial class ucNhaCungCap : UserControl
    {
        NhaCungCapBUS bus = new();
        DataTable allData = new DataTable();
        int currentPage = 1, pageSize = 10, totalPages = 1;
        bool isEdit = false;

        public ucNhaCungCap()
        {
            InitializeComponent();
            LoadData();
        }

        void LoadData()
        {
            allData = bus.HienThi();
            string kw = txtSearch.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(kw))
            {
                var filtered = allData.AsEnumerable().Where(r => 
                    r["TenNCC"].ToString()!.ToLower().Contains(kw) || 
                    r["SoDienThoai"].ToString()!.Contains(kw) ||
                    r["MaNCC"].ToString()!.ToLower().Contains(kw) ||
                    r["Email"].ToString()!.ToLower().Contains(kw)
                );
                if (filtered.Any()) allData = filtered.CopyToDataTable();
                else allData = allData.Clone();
            }
            currentPage = 1;
            UpdateGrid();
        }

        void UpdateGrid()
        {
            if (allData.Rows.Count == 0) { dgData.ItemsSource = null; txtPage.Text = "0 / 0"; return; }
            totalPages = (int)Math.Ceiling((double)allData.Rows.Count / pageSize);
            if (currentPage > totalPages) currentPage = totalPages;
            if (currentPage < 1) currentPage = 1;
            txtPage.Text = $"{currentPage} / {totalPages}";
            var paginated = allData.AsEnumerable().Skip((currentPage - 1) * pageSize).Take(pageSize);
            dgData.ItemsSource = paginated.Any() ? paginated.CopyToDataTable().DefaultView : null;
        }

        void btnPrev_Click(object s, RoutedEventArgs e) { if (currentPage > 1) { currentPage--; UpdateGrid(); } }
        void btnNext_Click(object s, RoutedEventArgs e) { if (currentPage < totalPages) { currentPage++; UpdateGrid(); } }

        void btnTim_Click(object s, RoutedEventArgs e) => LoadData();

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        void btnThem_Click(object s, RoutedEventArgs e)
        {
            isEdit = false;
            txtFormTitle.Text = "Thêm NCC";
            txtMa.Text = bus.TaoMa();
            txtTen.Text = txtDiaChi.Text = txtSDT.Text = txtEmail.Text = "";
            formPanel.Visibility = Visibility.Visible;
        }

        void btnSua_Click(object s, RoutedEventArgs e)
        {
            var r = (DataRowView)dgData.SelectedItem;
            if (r == null) return;
            isEdit = true;
            txtFormTitle.Text = "Sửa NCC";
            txtMa.Text = r["MaNCC"].ToString()!.Trim();
            txtTen.Text = r["TenNCC"].ToString()!.Trim();
            txtDiaChi.Text = r["DiaChi"].ToString()!.Trim();
            txtSDT.Text = r["SoDienThoai"].ToString()!.Trim();
            txtEmail.Text = r["Email"].ToString()!.Trim();
            formPanel.Visibility = Visibility.Visible;
        }

        void btnXoa_Click(object s, RoutedEventArgs e)
        {
            var r = (DataRowView)dgData.SelectedItem;
            if (r == null) return;
            string ma = r["MaNCC"].ToString()!.Trim();
            string ten = r["TenNCC"].ToString()!.Trim();

            if (MessageBox.Show($"Xác nhận xóa nhà cung cấp '{ten}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    string oldData = $"Tên: {ten}, SĐT: {r["SoDienThoai"]}, Email: {r["Email"]}";
                    bus.Xoa(ma);
                    new LichSuBUS().GhiNhanChinhSua(MainWindow.CurrentNV, "NCC", "DELETE", ma, oldData, "Đã xóa");
                    LoadData();
                    MessageBox.Show("Đã xóa nhà cung cấp thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("REFERENCE constraint") || ex.Message.Contains("FOREIGN KEY"))
                        MessageBox.Show("Không thể xóa Nhà cung cấp này vì đang có Sản phẩm thuộc nhà cung cấp này.\nVui lòng xóa các Sản phẩm liên quan trước!", "Lỗi xóa dữ liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                string dc = txtDiaChi.Text.Trim();
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
                    var r = (DataRowView)dgData.SelectedItem;
                    string oldData = $"Tên: {r["TenNCC"]}, SĐT: {r["SoDienThoai"]}, Email: {r["Email"]}";
                    string newData = $"Tên: {ten}, SĐT: {sdt}, Email: {email}";
                    
                    bus.CapNhat(ma, ten, dc, sdt, email);
                    new LichSuBUS().GhiNhanChinhSua(MainWindow.CurrentNV, "NCC", "UPDATE", ma, oldData, newData);
                }
                else
                {
                    bus.Them(ma, ten, dc, sdt, email);
                    new LichSuBUS().GhiNhanChinhSua(MainWindow.CurrentNV, "NCC", "INSERT", ma, "", $"Tên: {ten}, SĐT: {sdt}");
                }
                LoadData();
                formPanel.Visibility = Visibility.Collapsed;
                MessageBox.Show(isEdit ? "Cập nhật nhà cung cấp thành công!" : "Thêm mới nhà cung cấp thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        void btnHuy_Click(object s, RoutedEventArgs e) => formPanel.Visibility = Visibility.Collapsed;
    }
}
