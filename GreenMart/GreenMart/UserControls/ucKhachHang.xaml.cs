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
    public partial class ucKhachHang : UserControl
    {
        KhachHangBUS bus = new(); bool isEdit = false;
        DataTable allData = new DataTable();
        int currentPage = 1, pageSize = 10, totalPages = 1;

        public ucKhachHang() { InitializeComponent(); cboStatus.SelectedIndex = 0; LoadData(); }
        
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
            DateTime? tu = dpTu.SelectedDate;
            DateTime? den = dpDen.SelectedDate;
            string st = (cboStatus.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Tất cả";

            var filtered = allData.AsEnumerable().Where(r => {
                bool matchKW = string.IsNullOrEmpty(kw) || 
                               r["HoTen"].ToString()!.ToLower().Contains(kw) || 
                               r["SoDienThoai"].ToString()!.Contains(kw) ||
                               r["MaKH"].ToString()!.ToLower().Contains(kw);
                
                DateTime ngayTao = Convert.ToDateTime(r["NgayTao"]);
                bool matchTu = !tu.HasValue || ngayTao.Date >= tu.Value.Date;
                bool matchDen = !den.HasValue || ngayTao.Date <= den.Value.Date;
                bool matchST = st == "Tất cả" || r["TrangThai"].ToString() == st;

                return matchKW && matchTu && matchDen && matchST;
            });

            if (filtered.Any()) allData = filtered.CopyToDataTable();
            else allData = allData.Clone();

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
        void btnThem_Click(object s, RoutedEventArgs e) { 
            isEdit=false; 
            txtFormTitle.Text="Thêm KH"; 
            txtMa.Text=bus.TaoMa(); 
            txtTen.Text=txtSDT.Text=txtDiaChi.Text=txtEmail.Text=""; 
            txtDiem.Text="0";
            cboFormStatus.SelectedIndex = 0;
            formPanel.Visibility=Visibility.Visible; 
        }
        void btnSua_Click(object s, RoutedEventArgs e) { 
            var r=(DataRowView)dgData.SelectedItem; 
            if(r==null)return; 
            isEdit=true; 
            txtFormTitle.Text="Sửa KH";
            txtMa.Text=r["MaKH"].ToString()!.Trim(); 
            txtTen.Text=r["HoTen"].ToString()!.Trim(); 
            txtSDT.Text=r["SoDienThoai"].ToString()!.Trim(); 
            txtDiaChi.Text=r["DiaChi"].ToString()!.Trim(); 
            txtEmail.Text=r["Email"].ToString()!.Trim(); 
            txtDiem.Text=r["DiemTichLuy"].ToString();
            cboFormStatus.Text = r["TrangThai"].ToString();
            formPanel.Visibility=Visibility.Visible; 
        }
        void btnXoa_Click(object s, RoutedEventArgs e)
        {
            var r=(DataRowView)dgData.SelectedItem;
            if(r==null)return;
            string ma = r["MaKH"].ToString()!.Trim();
            if(MessageBox.Show("Xóa khách hàng này?","Xác nhận",MessageBoxButton.YesNo)==MessageBoxResult.Yes)
            {
                try{
                    string cu = $"Tên: {r["HoTen"]}, SDT: {r["SoDienThoai"]}";
                    bus.Xoa(ma);
                    new LichSuBUS().GhiNhanChinhSua(MainWindow.CurrentNV, "KhachHang", "DELETE", ma, cu, "");
                    LoadData();
                }catch(Exception ex){MessageBox.Show(ex.Message);}
            }
        }
        void btnLuu_Click(object s, RoutedEventArgs e)
        {
            try{
                string ma = txtMa.Text.Trim();
                string ten = txtTen.Text.Trim();
                string sdt = txtSDT.Text.Trim();
                string email = txtEmail.Text.Trim();
                string dc = txtDiaChi.Text.Trim();
                int diem = int.TryParse(txtDiem.Text, out int d) ? d : 0;
                string tt = (cboFormStatus.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Hoạt động";

                if (string.IsNullOrWhiteSpace(ten) || string.IsNullOrWhiteSpace(sdt) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(dc))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ tất cả các thông tin (Tên, SĐT, Email, Địa chỉ)!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                
                if(isEdit) {
                    var r=(DataRowView)dgData.SelectedItem;
                    string cu = $"Tên: {r["HoTen"]}, SDT: {r["SoDienThoai"]}, Email: {r["Email"]}, Điểm: {r["DiemTichLuy"]}, TT: {r["TrangThai"]}";
                    string moi = $"Tên: {ten}, SDT: {sdt}, Email: {txtEmail.Text}, Điểm: {diem}, TT: {tt}";
                    bus.CapNhat(ma, ten, sdt, txtDiaChi.Text.Trim(), txtEmail.Text.Trim(), diem, tt);
                    new LichSuBUS().GhiNhanChinhSua(MainWindow.CurrentNV, "KhachHang", "UPDATE", ma, cu, moi);
                } else {
                    string moi = $"Tên: {ten}, SDT: {sdt}, Email: {txtEmail.Text}, Điểm: {diem}, TT: {tt}";
                    bus.Them(ma, ten, sdt, txtDiaChi.Text.Trim(), txtEmail.Text.Trim(), diem, tt);
                    new LichSuBUS().GhiNhanChinhSua(MainWindow.CurrentNV, "KhachHang", "INSERT", ma, "", moi);
                }
                LoadData();
                formPanel.Visibility=Visibility.Collapsed;
            }catch(Exception ex){MessageBox.Show(ex.Message);}
        }
        void btnHuy_Click(object s, RoutedEventArgs e) => formPanel.Visibility=Visibility.Collapsed;
    }
}
