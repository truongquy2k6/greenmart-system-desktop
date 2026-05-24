using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BUS;

namespace GreenMart.UserControls
{
    public partial class ucLichSuTruyCap : UserControl
    {
        LichSuBUS bus = new();
        DataTable allData = new DataTable();
        int currentPage = 1, pageSize = 15, totalPages = 1;

        public ucLichSuTruyCap()
        {
            InitializeComponent();
            // Mặc định lọc 30 ngày gần nhất
            dtTu.SelectedDate = DateTime.Now.AddDays(-30);
            dtDen.SelectedDate = DateTime.Now;
            LoadData();
        }

        void LoadData()
        {
            allData = bus.LayTruyCap();
            currentPage = 1;
            UpdateStats();
            UpdateGrid();
        }

        void UpdateStats()
        {
            if (allData == null || allData.Rows.Count == 0)
            {
                txtTongLuot.Text = "0";
                txtHomNay.Text = "0";
                txtNhanVienKhacNhau.Text = "0";
                txtDangOnline.Text = "0";
                return;
            }

            txtTongLuot.Text = allData.Rows.Count.ToString("N0");

            // Hôm nay
            string today = DateTime.Today.ToString("yyyy-MM-dd");
            int homnay = allData.AsEnumerable()
                .Count(r => r["ThoiGianDangNhap"] != DBNull.Value &&
                            Convert.ToDateTime(r["ThoiGianDangNhap"]).Date == DateTime.Today);
            txtHomNay.Text = homnay.ToString();

            // Nhân viên khác nhau
            int nvCount = allData.AsEnumerable()
                .Select(r => r["MaNV"].ToString())
                .Distinct().Count();
            txtNhanVienKhacNhau.Text = nvCount.ToString();

            // Đang online (chưa đăng xuất)
            int online = allData.AsEnumerable()
                .Count(r => r["ThoiGianDangXuat"] == DBNull.Value || r["ThoiGianDangXuat"].ToString() == "");
            txtDangOnline.Text = online.ToString();
        }

        void UpdateGrid()
        {
            if (allData == null || allData.Rows.Count == 0)
            {
                dg.ItemsSource = null;
                txtPage.Text = "0 / 0";
                return;
            }

            var query = allData.AsEnumerable().AsEnumerable();

            // Lọc từ ngày - đến ngày
            if (dtTu.SelectedDate.HasValue)
            {
                var tu = dtTu.SelectedDate.Value.Date;
                query = query.Where(r => r["ThoiGianDangNhap"] != DBNull.Value &&
                                        Convert.ToDateTime(r["ThoiGianDangNhap"]).Date >= tu);
            }
            if (dtDen.SelectedDate.HasValue)
            {
                var den = dtDen.SelectedDate.Value.Date;
                query = query.Where(r => r["ThoiGianDangNhap"] != DBNull.Value &&
                                        Convert.ToDateTime(r["ThoiGianDangNhap"]).Date <= den);
            }

            // Lọc keyword
            string kw = txtSearch.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(kw))
            {
                query = query.Where(r =>
                    r["HoTen"].ToString()!.ToLower().Contains(kw) ||
                    r["MaNV"].ToString()!.ToLower().Contains(kw) ||
                    r["DiaChiIP"].ToString()!.ToLower().Contains(kw) ||
                    r["ThietBi"].ToString()!.ToLower().Contains(kw));
            }

            var filtered = query.ToList();
            if (!filtered.Any())
            {
                dg.ItemsSource = null;
                txtPage.Text = "0 / 0";
                return;
            }

            totalPages = (int)Math.Ceiling((double)filtered.Count / pageSize);
            if (currentPage > totalPages) currentPage = totalPages;
            if (currentPage < 1) currentPage = 1;
            txtPage.Text = $"{currentPage} / {totalPages}";

            var paginated = filtered.Skip((currentPage - 1) * pageSize).Take(pageSize);
            dg.ItemsSource = paginated.CopyToDataTable().DefaultView;
        }

        void btnTim_Click(object s, RoutedEventArgs e) { currentPage = 1; UpdateGrid(); }
        void txtSearch_KeyDown(object s, KeyEventArgs e) { if (e.Key == Key.Enter) { currentPage = 1; UpdateGrid(); } }
        void btnPrev_Click(object s, RoutedEventArgs e) { if (currentPage > 1) { currentPage--; UpdateGrid(); } }
        void btnNext_Click(object s, RoutedEventArgs e) { if (currentPage < totalPages) { currentPage++; UpdateGrid(); } }

        void btnReset_Click(object s, RoutedEventArgs e)
        {
            dtTu.SelectedDate = DateTime.Now.AddDays(-30);
            dtDen.SelectedDate = DateTime.Now;
            txtSearch.Text = "";
            currentPage = 1;
            LoadData();
        }
    }
}
