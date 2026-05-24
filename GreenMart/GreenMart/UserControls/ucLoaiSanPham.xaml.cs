using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using BUS;

namespace GreenMart.UserControls
{
    public partial class ucLoaiSanPham : UserControl
    {
        LoaiSanPhamBUS bus = new();
        DataTable allData = new DataTable();
        int currentPage = 1, pageSize = 10, totalPages = 1;
        bool isEdit = false;

        public ucLoaiSanPham()
        {
            InitializeComponent();
            LoadData();
        }

        void btnTim_Click(object s, RoutedEventArgs e) 
        {
            currentPage = 1;
            LoadData();
        }

        void LoadData()
        {
            string kw = txtSearch.Text.Trim();
            allData = bus.TimKiem(kw, currentPage, pageSize);
            UpdateGrid();
        }

        void UpdateGrid()
        {
            if (allData.Rows.Count == 0) { dgData.ItemsSource = null; txtPage.Text = "0 / 0"; return; }
            
            int totalRows = Convert.ToInt32(allData.Rows[0]["TotalRows"]);
            totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
            
            if (currentPage > totalPages) currentPage = totalPages;
            if (currentPage < 1) currentPage = 1;
            txtPage.Text = $"{currentPage} / {totalPages}";
            
            dgData.ItemsSource = allData.DefaultView;
        }

        void btnPrev_Click(object s, RoutedEventArgs e) { if (currentPage > 1) { currentPage--; LoadData(); } }
        void btnNext_Click(object s, RoutedEventArgs e) { if (currentPage < totalPages) { currentPage++; LoadData(); } }

        void btnThem_Click(object s, RoutedEventArgs e)
        {
            isEdit = false;
            txtFormTitle.Text = "Thêm loại sản phẩm";
            txtMa.Text = bus.TaoMa();
            txtTen.Text = "";
            formPanel.Visibility = Visibility.Visible;
        }

        void btnSua_Click(object s, RoutedEventArgs e)
        {
            var row = (DataRowView)dgData.SelectedItem;
            if (row == null) return;
            isEdit = true;
            txtFormTitle.Text = "Sửa loại sản phẩm";
            txtMa.Text = row["MaLoai"].ToString()!.Trim();
            txtTen.Text = row["TenLoai"].ToString()!.Trim();
            formPanel.Visibility = Visibility.Visible;
        }

        void btnXoa_Click(object s, RoutedEventArgs e)
        {
            var row = (DataRowView)dgData.SelectedItem;
            if (row == null) return;
            string ma = row["MaLoai"].ToString()!.Trim();
            string ten = row["TenLoai"].ToString()!.Trim();

            if (MessageBox.Show($"Xác nhận xóa loại sản phẩm '{ten}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    bus.Xoa(ma);
                    new LichSuBUS().GhiNhanChinhSua(MainWindow.CurrentNV, "LoaiSP", "DELETE", ma, $"Tên loại: {ten}", "Đã xóa");
                    LoadData();
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("REFERENCE constraint") || ex.Message.Contains("FOREIGN KEY"))
                        MessageBox.Show("Không thể xóa Loại Sản Phẩm này vì đang có Sản Phẩm thuộc loại này.\nVui lòng xóa các Sản Phẩm liên quan trước!", "Lỗi xóa dữ liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    else
                        MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        void btnLuu_Click(object s, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtTen.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên loại sản phẩm!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string ma = txtMa.Text.Trim();
                string ten = txtTen.Text.Trim();

                if (isEdit)
                {
                    var row = (DataRowView)dgData.SelectedItem;
                    string oldTen = row["TenLoai"].ToString()!.Trim();
                    bus.CapNhat(ma, ten);
                    new LichSuBUS().GhiNhanChinhSua(MainWindow.CurrentNV, "LoaiSP", "UPDATE", ma, $"Tên cũ: {oldTen}", $"Tên mới: {ten}");
                }
                else
                {
                    bus.Them(ma, ten);
                    new LichSuBUS().GhiNhanChinhSua(MainWindow.CurrentNV, "LoaiSP", "INSERT", ma, "", $"Tên loại: {ten}");
                }
                LoadData();
                formPanel.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        void btnHuy_Click(object s, RoutedEventArgs e) => formPanel.Visibility = Visibility.Collapsed;
    }
}
