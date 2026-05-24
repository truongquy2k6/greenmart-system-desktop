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
    public partial class ucNhanVien : UserControl
    {
        NhanVienBUS bus = new();
        CuaHangBUS busCH = new();
        DataTable allData = new DataTable();
        int currentPage = 1, pageSize = 10, totalPages = 1;
        bool isEdit = false;

        public ucNhanVien()
        {
            InitializeComponent();
            LoadData();
            
            // Load Store list into filter and form
            var stores = busCH.HienThi();
            cboCH.ItemsSource = stores.DefaultView;

            DataTable filterStores = stores.Copy();
            DataRow allRow = filterStores.NewRow();
            allRow["MaCH"] = "ALL";
            allRow["TenCH"] = "Tất cả cửa hàng";
            filterStores.Rows.InsertAt(allRow, 0);
            fboCH.ItemsSource = filterStores.DefaultView;
            fboCH.SelectedIndex = 0;

            if (MainWindow.CurrentRole != "Admin")
            {
                var fboAdminItem = fboCV.Items.Cast<ComboBoxItem>().FirstOrDefault(x => x.Content?.ToString() == "Admin");
                if (fboAdminItem != null) fboCV.Items.Remove(fboAdminItem);

                var cboAdminItem = cboCV.Items.Cast<ComboBoxItem>().FirstOrDefault(x => x.Content?.ToString() == "Admin");
                if (cboAdminItem != null) cboCV.Items.Remove(cboAdminItem);
            }

            fboCV.SelectedIndex = 0;
            fboStatus.SelectedIndex = 0;
        }

        void LoadData() { allData = bus.HienThi(); ApplyFilter(); }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        DataTable filteredData = new DataTable();
        void ApplyFilter()
        {
            if (allData == null || allData.Rows.Count == 0) return;

            string kw = txtSearch.Text.ToLower().Trim();
            string cv = (fboCV.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Tất cả chức vụ";
            string st = (fboStatus.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Tất cả trạng thái";
            string ch = fboCH.SelectedValue?.ToString()?.Trim() ?? "ALL";

            var query = allData.AsEnumerable().Where(r => 
            {
                bool matchKW = string.IsNullOrEmpty(kw) || 
                               r["MaNV"].ToString()!.ToLower().Contains(kw) || 
                               r["HoTen"].ToString()!.ToLower().Contains(kw) ||
                               r["SoDienThoai"].ToString()!.Contains(kw);
                
                bool matchCV = cv == "Tất cả chức vụ" || r["ChucVu"].ToString() == cv;
                bool matchST = st == "Tất cả trạng thái" || r["TrangThai"].ToString() == st;
                bool matchCH = ch == "ALL" || r["MaCH"].ToString().Trim() == ch;

                return matchKW && matchCV && matchST && matchCH;
            });

            filteredData = query.Any() ? query.CopyToDataTable() : allData.Clone();
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

        void Filter_Changed(object s, SelectionChangedEventArgs e) => ApplyFilter();
        void btnTim_Click(object s, RoutedEventArgs e) => ApplyFilter();

        void btnView_Click(object s, RoutedEventArgs e)
        {
            var r = (DataRowView?)((Button)s).DataContext;
            if (r == null) return;

            pMa.Text = r["MaNV"].ToString();
            pTen.Text = r["HoTen"].ToString();
            pCV.Text = r["ChucVu"].ToString();
            pGT.Text = r["GioiTinh"].ToString();
            pNS.Text = Convert.ToDateTime(r["NgaySinh"]).ToString("dd/MM/yyyy");
            pSDT.Text = r["SoDienThoai"].ToString();
            pDC.Text = r["DiaChi"].ToString();
            pCH.Text = r["TenCH"].ToString();
            pStatus.Text = r["TrangThai"].ToString();

            popDetail.Visibility = Visibility.Visible;
        }

        void btnHuyDetail_Click(object s, RoutedEventArgs e) => popDetail.Visibility = Visibility.Collapsed;

        void btnThem_Click(object s, RoutedEventArgs e)
        {
            isEdit = false;
            tft.Text = "Thêm nhân viên";
            txtMa.Text = bus.TaoMa();
            txtTen.Text = txtSDT.Text = txtDC.Text = txtTDN.Text = "";
            txtMK.Password = "";
            cboCV.SelectedIndex = cboGT.SelectedIndex = cboCH.SelectedIndex = -1;
            cboStatus.SelectedIndex = 0; // Mặc định Đang làm việc
            dpNS.SelectedDate = DateTime.Now.AddYears(-20);
            fp.Visibility = Visibility.Visible;
        }

        void btnSua_Click(object s, RoutedEventArgs e)
        {
            var r = (DataRowView?)((Button)s).DataContext;
            if (r == null) return;
            isEdit = true;
            tft.Text = "Sửa nhân viên";
            txtMa.Text = r["MaNV"].ToString()!.Trim();
            txtTen.Text = r["HoTen"].ToString()!.Trim();
            txtSDT.Text = r["SoDienThoai"].ToString()!.Trim();
            txtDC.Text = r["DiaChi"].ToString()!.Trim();
            txtTDN.Text = r["TenDangNhap"].ToString()!.Trim();
            txtMK.Password = "";
            dpNS.SelectedDate = Convert.ToDateTime(r["NgaySinh"]);
            cboGT.Text = r["GioiTinh"].ToString()?.Trim();
            cboCV.Text = r["ChucVu"].ToString()?.Trim();
            cboCH.SelectedValue = r["MaCH"];
            cboStatus.Text = r["TrangThai"].ToString()?.Trim();

            fp.Visibility = Visibility.Visible;
        }

        void btnXoa_Click(object s, RoutedEventArgs e)
        {
            var r = (DataRowView?)((Button)s).DataContext;
            if (r == null) return;
            string ma = r["MaNV"].ToString()!.Trim();
            if (MessageBox.Show($"Xóa nhân viên '{r["HoTen"]}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    string cu = $"Tên: {r["HoTen"]}, CV: {r["ChucVu"]}";
                    bus.Xoa(ma);
                    new LichSuBUS().GhiNhanChinhSua(MainWindow.CurrentNV, "NhanVien", "DELETE", ma, cu, "Đã xóa");
                    LoadData();
                    MessageBox.Show("Đã xóa nhân viên thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("REFERENCE constraint") || ex.Message.Contains("FOREIGN KEY"))
                        MessageBox.Show("Không thể xóa nhân viên này vì đang có dữ liệu liên quan (Hóa đơn, Lịch sử...).\nVui lòng chuyển trạng thái sang 'Nghỉ việc'!", "Lỗi xóa", MessageBoxButton.OK, MessageBoxImage.Warning);
                    else MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        void btnLuu_Click(object s, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtTen.Text)) { MessageBox.Show("Vui lòng nhập họ tên!", "Thông báo"); return; }
                if (string.IsNullOrWhiteSpace(txtSDT.Text)) { MessageBox.Show("Vui lòng nhập số điện thoại!", "Thông báo"); return; }
                if (txtSDT.Text.Trim().Length != 10) { MessageBox.Show("Số điện thoại phải bao gồm đúng 10 chữ số!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
                if (string.IsNullOrWhiteSpace(txtTDN.Text)) { MessageBox.Show("Vui lòng nhập tên đăng nhập!", "Thông báo"); return; }

                string cv = (cboCV.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
                string gt = (cboGT.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
                string tt = (cboStatus.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Đang làm việc";
                string ch = cboCH.SelectedValue?.ToString() ?? "";
                DateTime ns = dpNS.SelectedDate ?? DateTime.Now;
                string ma = txtMa.Text.Trim();
                string ten = txtTen.Text.Trim();

                if (isEdit)
                {
                    var r = (DataRowView)dg.SelectedItem;
                    string cu = $"Tên: {r["HoTen"]}, CV: {r["ChucVu"]}, TT: {r["TrangThai"]}";
                    string moi = $"Tên: {ten}, CV: {cv}, TT: {tt}";
                    string finalPassword = string.IsNullOrEmpty(txtMK.Password) ? r["MatKhau"].ToString()! : PasswordHelper.HashPassword(txtMK.Password);
                    bus.CapNhat(ma, ten, cv, ns, gt, txtSDT.Text.Trim(), txtDC.Text.Trim(), txtTDN.Text.Trim(), finalPassword, ch, tt);
                    new LichSuBUS().GhiNhanChinhSua(MainWindow.CurrentNV, "NhanVien", "UPDATE", ma, cu, moi);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(txtMK.Password)) { MessageBox.Show("Vui lòng nhập mật khẩu cho nhân viên mới!", "Thông báo"); return; }
                    string finalPassword = PasswordHelper.HashPassword(txtMK.Password);
                    bus.Them(ma, ten, cv, ns, gt, txtSDT.Text.Trim(), txtDC.Text.Trim(), txtTDN.Text.Trim(), finalPassword, ch, tt);
                    new LichSuBUS().GhiNhanChinhSua(MainWindow.CurrentNV, "NhanVien", "INSERT", ma, "", $"Tên: {ten}, CV: {cv}, TT: {tt}");
                }
                LoadData();
                fp.Visibility = Visibility.Collapsed;
                MessageBox.Show("Đã lưu thông tin nhân viên thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        void btnHuy_Click(object s, RoutedEventArgs e) => fp.Visibility = Visibility.Collapsed;
    }
}
