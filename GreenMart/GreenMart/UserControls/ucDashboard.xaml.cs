using System.Windows.Controls;
using BUS;

namespace GreenMart.UserControls
{
    public class ChartBar { public double Height { get; set; } public string Label { get; set; } = ""; public string Tooltip { get; set; } = ""; public bool IsPeak { get; set; } public double LabelY { get; set; } }
    public class TopSP { public string TenSP { get; set; } = ""; public int DaBan { get; set; } public double BarWidth { get; set; } }
    public class TopKH { public string HoTen { get; set; } = ""; public int SoDon { get; set; } public decimal TongChi { get; set; } }
    public class LowStock { public string TenSP { get; set; } = ""; public int TonKho { get; set; } public string DonViTinh { get; set; } = ""; }

    public partial class ucDashboard : UserControl
    {
        System.Data.DataTable _dtChart = new System.Data.DataTable();
        decimal _maxVal = 1;

        public ucDashboard()
        {
            InitializeComponent();
            txtSubTitle.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy", new System.Globalization.CultureInfo("vi-VN"));
            LoadData();
        }

        void LoadData()
        {
            try
            {
                var bus = new ThongKeBUS();
                
                // 1. KPI Cards - Tổng quan
                var dt = bus.LayTongQuan();
                if (dt.Rows.Count > 0)
                {
                    var r = dt.Rows[0];
                    txtDoanhThu.Text = string.Format("{0:#,##0} ₫", r["DoanhThu"]);
                    txtSoDon.Text = string.Format("{0:#,##0}", r["SoDon"]);
                    txtSoKhach.Text = string.Format("{0:#,##0}", r["SoKhach"]);
                    txtGiaTriKho.Text = string.Format("{0:#,##0} ₫", r["GiaTriKho"]);
                    txtSapHet.Text = $"{r["SapHet"]} sản phẩm sắp hết hàng";
                    txtDonTB.Text = $"TB: {string.Format("{0:#,##0}", r["DonTrungBinh"])} ₫/đơn";
                    txtDTTang.Text = $"{r["TangTruong"]}% so với tháng trước";
                }

                // 2. Wave Chart - Biểu đồ lượn sóng
                _dtChart = bus.LayBieuDo();
                _maxVal = 1;
                decimal totalWeek = 0;
                foreach (System.Data.DataRow r in _dtChart.Rows)
                {
                    decimal v = Convert.ToDecimal(r["Tien"]);
                    if (v > _maxVal) _maxVal = v;
                    totalWeek += v;
                }
                txtTongTuan.Text = string.Format("{0:#,##0} ₫", totalWeek);

                var bars = new System.Collections.Generic.List<ChartBar>();
                for (int i = 0; i < _dtChart.Rows.Count; i++)
                {
                    decimal val = Convert.ToDecimal(_dtChart.Rows[i]["Tien"]);
                    double h = ((double)val / (double)_maxVal * 150);
                    bars.Add(new ChartBar 
                    { 
                        Label = _dtChart.Rows[i]["Ngay"].ToString()!, 
                        Tooltip = val > 0 ? string.Format("{0:#,##0} ₫", val) : "0 ₫",
                        LabelY = -h
                    });
                }
                chartPoints.ItemsSource = bars;
                UpdateWaveChart();

                // 3. Top SP - Sản phẩm bán chạy
                var dtTop = bus.LayTopSP();
                int topMax = 1;
                foreach (System.Data.DataRow r in dtTop.Rows)
                    if (Convert.ToInt32(r["DaBan"]) > topMax) topMax = Convert.ToInt32(r["DaBan"]);
                
                var tops = new System.Collections.Generic.List<TopSP>();
                foreach (System.Data.DataRow r in dtTop.Rows)
                    tops.Add(new TopSP { TenSP = r["TenSP"].ToString()!, DaBan = Convert.ToInt32(r["DaBan"]), BarWidth = (double)Convert.ToInt32(r["DaBan"]) / topMax * 200 });
                topSPList.ItemsSource = tops;

                // 4. Top KH - Khách hàng tiêu biểu
                var dtKH = bus.LayTopKhachHang();
                var khs = new System.Collections.Generic.List<TopKH>();
                foreach (System.Data.DataRow r in dtKH.Rows)
                    khs.Add(new TopKH { HoTen = r["HoTen"].ToString()!, SoDon = Convert.ToInt32(r["SoDon"]), TongChi = Convert.ToDecimal(r["TongChi"]) });
                topKHList.ItemsSource = khs;

                // 5. Low Stock - Cảnh báo tồn kho
                var dtLow = bus.LayDanhSachSapHet();
                var lows = new System.Collections.Generic.List<LowStock>();
                foreach (System.Data.DataRow r in dtLow.Rows)
                    lows.Add(new LowStock { TenSP = r["TenSP"].ToString()!, TonKho = Convert.ToInt32(r["TonKho"]), DonViTinh = r["DonViTinh"].ToString()! });
                lowStockList.ItemsSource = lows;

                // 6. Recent HD - Giao dịch gần đây
                dgHoaDon.ItemsSource = bus.LayHDGanDay().DefaultView;
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex.Message); }
        }
        private void chartPoints_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            UpdateWaveChart();
        }

        void UpdateWaveChart()
        {
            try
            {
                if (_dtChart == null || _dtChart.Rows.Count == 0) return;

                double width = chartPoints.ActualWidth;
                if (width <= 0) return;

                int count = _dtChart.Rows.Count;
                double colWidth = width / count;

                var points = new System.Collections.Generic.List<System.Windows.Point>();
                for (int i = 0; i < count; i++)
                {
                    decimal val = Convert.ToDecimal(_dtChart.Rows[i]["Tien"]);
                    double h = ((double)val / (double)_maxVal * 150);
                    // Căn giữa theo cột của UniformGrid
                    double x = (i + 0.5) * colWidth;
                    points.Add(new System.Windows.Point(x, -h));
                }

                if (points.Count > 1)
                {
                    var segments = new System.Windows.Media.PathSegmentCollection();
                    for (int i = 0; i < points.Count - 1; i++)
                    {
                        var p1 = points[i];
                        var p2 = points[i + 1];
                        var cp1 = new System.Windows.Point((p1.X + p2.X) / 2, p1.Y);
                        var cp2 = new System.Windows.Point((p1.X + p2.X) / 2, p2.Y);
                        segments.Add(new System.Windows.Media.BezierSegment(cp1, cp2, p2, true));
                    }

                    var lineFigure = new System.Windows.Media.PathFigure(points[0], segments, false);

                    // Area fill
                    var fillFigure = new System.Windows.Media.PathFigure(new System.Windows.Point(points[0].X, 0), new System.Windows.Media.PathSegmentCollection(), true);
                    fillFigure.Segments.Add(new System.Windows.Media.LineSegment(points[0], false));
                    foreach (var s in segments) fillFigure.Segments.Add(s);
                    fillFigure.Segments.Add(new System.Windows.Media.LineSegment(new System.Windows.Point(points.Last().X, 0), false));

                    wavePath.Data = new System.Windows.Media.PathGeometry(new[] { lineFigure, fillFigure });
                }
            }
            catch { }
        }
    }
}
