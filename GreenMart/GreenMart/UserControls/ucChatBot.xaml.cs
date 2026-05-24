using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GreenMart.Services;
using DAL;

namespace GreenMart.UserControls
{
    // Removed ChatMessage class

    public partial class ucChatBot : UserControl
    {
        private GeminiService geminiService = new GeminiService();
        public Action OnCloseClicked { get; set; }

        public ucChatBot()
        {
            InitializeComponent();
            AddChatMessage("Chào sếp! Tôi là Trợ lý AI của GreenMart.\nTôi có thể giúp sếp xem thống kê doanh thu, kho hàng hoặc hướng dẫn cách sử dụng phần mềm. Sếp cần gì ạ?", false);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            OnCloseClicked?.Invoke();
        }

        private async void btnSend_Click(object sender, RoutedEventArgs e)
        {
            string msg = txtMessage.Text.Trim();
            if (string.IsNullOrEmpty(msg)) return;

            // Add user message
            AddChatMessage(msg, true);
            txtMessage.Text = "";
            txtMessage.IsEnabled = false;
            btnSend.IsEnabled = false;
            loadingIndicator.Visibility = Visibility.Visible;
            scrollChat.ScrollToEnd();

            // Prepare Context (Data Injection)
            string context = GetSystemContext();

            // Call API
            string reply = await geminiService.SendMessageAsync(msg, context);

            // Add AI response
            AddChatMessage(reply, false);
            loadingIndicator.Visibility = Visibility.Collapsed;
            txtMessage.IsEnabled = true;
            btnSend.IsEnabled = true;
            txtMessage.Focus();
            scrollChat.ScrollToEnd();
        }

        private void txtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnSend_Click(null, null);
            }
        }

        private string ExportTableToCsv(System.Data.DataTable dt, int maxRows = 50)
        {
            if (dt == null || dt.Rows.Count == 0) return "";
            var sb = new System.Text.StringBuilder();
            var columnNames = new System.Collections.Generic.List<string>();
            foreach (System.Data.DataColumn column in dt.Columns)
                columnNames.Add(column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));

            int limit = Math.Min(dt.Rows.Count, maxRows);
            for (int i = 0; i < limit; i++)
            {
                var fields = new System.Collections.Generic.List<string>();
                foreach (var item in dt.Rows[i].ItemArray)
                    fields.Add("\"" + item?.ToString()?.Replace("\"", "\"\"") + "\"");
                sb.AppendLine(string.Join(",", fields));
            }
            return sb.ToString();
        }

        private string GetSystemContext()
        {
            try
            {
                var busTK = new BUS.ThongKeBUS();
                var dt = busTK.LayTongQuan();
                
                string baseStats = "Hệ thống chưa có dữ liệu giao dịch.";
                if (dt.Rows.Count > 0)
                {
                    var r = dt.Rows[0];
                    string doanhThu = string.Format("{0:#,##0} VNĐ", r["DoanhThu"]);
                    string soDon = r["SoDon"].ToString() ?? "0";
                    string sapHet = r["SapHet"].ToString() ?? "0";

                    baseStats = $"[Ngày hiện tại: {DateTime.Now:dd/MM/yyyy}] - [Doanh thu hôm nay: {doanhThu}] - [Số đơn hàng: {soDon}] - [Sản phẩm sắp hết (tồn kho dưới mức an toàn): {sapHet}]";
                }

                // Nạp thêm dữ liệu từ các bảng (TRỪ bảng Nhân Viên)
                var busSP = new BUS.SanPhamBUS();
                var busKH = new BUS.KhachHangBUS();
                var busKM = new BUS.KhuyenMaiBUS();
                var busNCC = new BUS.NhaCungCapBUS();
                var busHD = new BUS.HoaDonBUS();
                var busKho = new BUS.KhoBUS();
                var busLSP = new BUS.LoaiSanPhamBUS();
                var busCH = new BUS.CuaHangBUS();
                var busNV = new BUS.NhanVienBUS();

                string dbContext = "\n\n--- TRÍCH XUẤT DỮ LIỆU CÁC BẢNG (Tối đa 20 dòng mỗi bảng) ---\n";
                dbContext += "\nBảng SẢN PHẨM:\n" + ExportTableToCsv(busSP.HienThi(), 20);
                dbContext += "\nBảng LOẠI SẢN PHẨM:\n" + ExportTableToCsv(busLSP.HienThi(), 20);
                dbContext += "\nBảng CỬA HÀNG:\n" + ExportTableToCsv(busCH.HienThi(), 20);
                dbContext += "\nBảng NHÂN VIÊN:\n" + ExportTableToCsv(busNV.HienThi(), 20);
                dbContext += "\nBảng KHÁCH HÀNG:\n" + ExportTableToCsv(busKH.HienThi(), 20);
                dbContext += "\nBảng KHUYẾN MÃI:\n" + ExportTableToCsv(busKM.HienThi(), 20);
                dbContext += "\nBảng NHÀ CUNG CẤP:\n" + ExportTableToCsv(busNCC.HienThi(), 20);
                dbContext += "\nBảng HÓA ĐƠN (dùng để tính doanh thu ngày/tháng):\n" + ExportTableToCsv(busHD.TimKiem(new DateTime(2000, 1, 1), DateTime.Now), 20);
                dbContext += "\nBảng KHO:\n" + ExportTableToCsv(busKho.HienThi(), 20);
                dbContext += "\nBảng CHI TIẾT KHO (chứa số lượng tồn kho theo sản phẩm):\n" + ExportTableToCsv(busKho.HienThiTatCaChiTiet(), 20);

                return baseStats + dbContext;
            }
            catch (Exception ex)
            {
                return $"Lỗi trích xuất dữ liệu: {ex.Message}";
            }
        }

        private void AddChatMessage(string text, bool isUser)
        {
            var container = new Grid { Margin = new Thickness(0, 5, 0, 10) };
            
            var border = new Border
            {
                Padding = new Thickness(12),
                CornerRadius = isUser ? new CornerRadius(12, 0, 12, 12) : new CornerRadius(0, 12, 12, 12),
                Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(isUser ? "#DCF8C6" : "#FFFFFF")),
                HorizontalAlignment = isUser ? HorizontalAlignment.Right : HorizontalAlignment.Left,
                Margin = isUser ? new Thickness(30, 0, 0, 0) : new Thickness(0, 0, 30, 0)
            };
            border.Effect = new System.Windows.Media.Effects.DropShadowEffect { BlurRadius = 5, ShadowDepth = 1, Opacity = 0.05 };

            var contentStack = new StackPanel();

            if (isUser)
            {
                contentStack.Children.Add(new TextBlock { Text = text, TextWrapping = TextWrapping.Wrap, Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1B5E20")), FontSize = 14 });
            }
            else
            {
                var parts = ParseMarkdownTables(text);
                foreach (var part in parts)
                {
                    if (part is string str)
                    {
                        if (string.IsNullOrWhiteSpace(str)) continue;
                        contentStack.Children.Add(new TextBlock { Text = str.Trim(), TextWrapping = TextWrapping.Wrap, Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2C3E50")), FontSize = 14, Margin = new Thickness(0,0,0,10) });
                    }
                    else if (part is DataTable dt)
                    {
                        var grid = new DataGrid
                        {
                            ItemsSource = dt.DefaultView,
                            AutoGenerateColumns = true,
                            IsReadOnly = true,
                            HeadersVisibility = DataGridHeadersVisibility.Column,
                            Margin = new Thickness(0, 5, 0, 15),
                            Background = System.Windows.Media.Brushes.White,
                            BorderBrush = System.Windows.Media.Brushes.LightGray,
                            BorderThickness = new Thickness(1),
                            MaxHeight = 250,
                            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
                        };
                        contentStack.Children.Add(grid);
                    }
                }
            }

            border.Child = contentStack;
            container.Children.Add(border);
            pnlChatMessages.Children.Add(container);
            scrollChat.ScrollToEnd();
        }

        private System.Collections.Generic.List<object> ParseMarkdownTables(string markdown)
        {
            var result = new System.Collections.Generic.List<object>();
            var lines = markdown.Split('\n');
            var currentText = new System.Text.StringBuilder();
            DataTable currentTable = null;
            bool inTable = false;

            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (trimmed.StartsWith("|") && trimmed.EndsWith("|"))
                {
                    if (!inTable)
                    {
                        inTable = true;
                        currentTable = new DataTable();
                        if (currentText.Length > 0)
                        {
                            result.Add(currentText.ToString());
                            currentText.Clear();
                        }
                        
                        var headers = trimmed.Trim('|').Split('|').Select(h => h.Trim()).ToList();
                        foreach (var h in headers) currentTable.Columns.Add(h);
                    }
                    else
                    {
                        if (trimmed.Contains("---")) continue;
                        var values = trimmed.Trim('|').Split('|').Select(v => v.Trim()).ToList();
                        if (values.Count == currentTable.Columns.Count)
                        {
                            currentTable.Rows.Add(values.ToArray());
                        }
                    }
                }
                else
                {
                    if (inTable)
                    {
                        inTable = false;
                        result.Add(currentTable);
                        currentTable = null;
                    }
                    currentText.AppendLine(line);
                }
            }

            if (inTable) result.Add(currentTable);
            if (currentText.Length > 0) result.Add(currentText.ToString());

            return result;
        }
    }
}
