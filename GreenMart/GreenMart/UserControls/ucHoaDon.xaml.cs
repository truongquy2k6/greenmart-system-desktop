using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using BUS;

namespace GreenMart.UserControls
{
    public partial class ucHoaDon : UserControl
    {
        HoaDonBUS bus=new();
        DataTable allData = new DataTable();
        int currentPage = 1, pageSize = 12, totalPages = 1;

        public ucHoaDon(){InitializeComponent();dpTu.SelectedDate=DateTime.Now.AddMonths(-1);dpDen.SelectedDate=DateTime.Now;LoadData();}
        
        void LoadData()
        {
            DateTime tu = dpTu.SelectedDate ?? DateTime.Now.AddMonths(-1);
            DateTime den = dpDen.SelectedDate ?? DateTime.Now;
            // Đảm bảo ngày kết thúc lấy đến hết ngày (23:59:59)
            DateTime denFix = new DateTime(den.Year, den.Month, den.Day, 23, 59, 59);
            
            allData = bus.TimKiem(tu, denFix, txtSearch.Text.Trim());
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
        
        void btnTim_Click(object s,RoutedEventArgs e)=>LoadData();
        void dg_SelectionChanged(object s,SelectionChangedEventArgs e)
        {
            var r=(DataRowView?)dg.SelectedItem;
            if(r==null)
            {
                dgCT.ItemsSource=null;
                txtSelectedMaHD.Text = "Hóa đơn: ---";
                txtSelectedKhach.Text = "Khách hàng: ---";
                txtSelectedNgay.Text = "Ngày lập: ---";
                txtSelectedNhanVien.Text = "Nhân viên: ---";
                txtSelectedSubtotal.Text = "0 đ";
                txtSelectedDiscount.Text = "0 đ";
                txtSelectedTong.Text = "0 đ";
                txtSelectedPTTT.Text = "Phương thức: ---";
                return;
            }
            string maHD = r["MaHD"].ToString()!.Trim();
            dgCT.ItemsSource=bus.LayChiTiet(maHD).DefaultView;
            
            decimal tong = Convert.ToDecimal(r["TongTien"]);
            decimal giam = Convert.ToDecimal(r["GiamGia"]);
            decimal sub = tong + giam;

            txtSelectedMaHD.Text = $"Hóa đơn: {maHD}";
            txtSelectedKhach.Text = r["TenKhachHang"].ToString();
            txtSelectedNgay.Text = $"Ngày lập: {Convert.ToDateTime(r["NgayLap"]):dd/MM/yyyy HH:mm}";
            txtSelectedNhanVien.Text = $"Nhân viên: {r["TenNhanVien"]}";
            
            txtSelectedSubtotal.Text = string.Format("{0:#,##0} đ", sub);
            txtSelectedDiscount.Text = string.Format("-{0:#,##0} đ", giam);
            txtSelectedTong.Text = string.Format("{0:#,##0} đ", tong);
            txtSelectedPTTT.Text = $"Phương thức: {r["PhuongThucThanhToan"]}";
        }

        void btnXuatExcel_Click(object s, RoutedEventArgs e)
        {
            try {
                if (allData.Rows.Count == 0) { MessageBox.Show("Không có dữ liệu để xuất!"); return; }
                var dialog = new Microsoft.Win32.SaveFileDialog { Filter = "CSV UTF-8 (Comma delimited)|*.csv", FileName = $"HoaDon_{DateTime.Now:yyyyMMdd_HHmm}.csv" };
                if (dialog.ShowDialog() == true) {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("\uFEFF");
                    sb.AppendLine("MaHD,NgayLap,TenKhachHang,TongTien,TrangThai");
                    foreach (DataRow r in allData.Rows)
                        sb.AppendLine($"{r["MaHD"]},\"{Convert.ToDateTime(r["NgayLap"]):dd/MM/yyyy}\",\"{r["TenKhachHang"]}\",{r["TongTien"]},\"{r["TrangThai"]}\"");
                    File.WriteAllText(dialog.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show("Xuất báo cáo thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            } catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        void btnInHD_Click(object s, RoutedEventArgs e)
        {
            var r = (DataRowView?)((Button)s).DataContext;
            if (r == null) return;
            string maHD = r["MaHD"].ToString()!.Trim();
            var dt = bus.LayChiTiet(maHD);
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("===========================================");
            sb.AppendLine("           GREEN MART SUPERMARKET          ");
            sb.AppendLine("===========================================");
            sb.AppendLine($"Mã Hóa Đơn: {maHD}");
            sb.AppendLine($"Ngày lập: {Convert.ToDateTime(r["NgayLap"]):dd/MM/yyyy HH:mm}");
            sb.AppendLine($"Khách hàng: {r["TenKhachHang"]}");
            sb.AppendLine("-------------------------------------------");
            sb.AppendLine(string.Format("{0,-20} | {1,-4} | {2,10}", "Sản phẩm", "SL", "Thành tiền"));
            sb.AppendLine("-------------------------------------------");
            foreach(DataRow dr in dt.Rows)
            {
                string tenSP = dr["TenSP"].ToString()!;
                if(tenSP.Length > 20) tenSP = tenSP.Substring(0, 17) + "...";
                sb.AppendLine(string.Format("{0,-20} | {1,-4} | {2,10:N0}", tenSP, dr["SoLuong"], dr["ThanhTien"]));
            }
            sb.AppendLine("-------------------------------------------");
            sb.AppendLine($"TỔNG TIỀN: {Convert.ToDecimal(r["TongTien"]):#,##0} đ");
            sb.AppendLine("===========================================");
            sb.AppendLine("         CẢM ƠN QUÝ KHÁCH HẸN GẶP LẠI      ");
            sb.AppendLine("===========================================");
            
            var dialog = new Microsoft.Win32.SaveFileDialog { Filter = "Text File|*.txt", FileName = $"HoaDon_{maHD}.txt" };
            if (dialog.ShowDialog() == true) {
                File.WriteAllText(dialog.FileName, sb.ToString(), Encoding.UTF8);
                MessageBox.Show("Đã in hóa đơn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        void btnHuy_Click(object s, RoutedEventArgs e)
        {
            var r = (DataRowView?)((Button)s).DataContext;
            if (r == null) return;
            string maHD = r["MaHD"].ToString()!.Trim();
            if (MessageBox.Show($"Bạn có chắc chắn muốn hủy hóa đơn {maHD}?", "Xác nhận hủy", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    bus.Huy(maHD);
                    MessageBox.Show("Đã hủy hóa đơn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }
    }
}
