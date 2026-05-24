using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BUS;

namespace GreenMart.UserControls
{
    public partial class ucLichSuChinhSua : UserControl
    {
        LichSuBUS bus = new();
        DataTable allData = new DataTable();
        int currentPage = 1, pageSize = 10, totalPages = 1;

        public ucLichSuChinhSua() { InitializeComponent(); LoadData(); }

        void LoadData()
        {
            string? table = null;
            if (cboBang.SelectedIndex > 0) table = ((ComboBoxItem)cboBang.SelectedItem).Content.ToString();
            allData = bus.LayChinhSua(table); 
            currentPage = 1; 
            UpdateGrid();
        }

        void UpdateGrid()
        {
            DataTable displayData = allData;
            string kw = txtSearch.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(kw))
            {
                var filteredRows = allData.AsEnumerable().Where(r => 
                    r["HoTen"].ToString()!.ToLower().Contains(kw));
                if (filteredRows.Any()) displayData = filteredRows.CopyToDataTable();
                else { dg.ItemsSource = null; txtPage.Text = "0 / 0"; return; }
            }

            if (displayData.Rows.Count == 0) { dg.ItemsSource = null; txtPage.Text = "0 / 0"; return; }
            totalPages = (int)Math.Ceiling((double)displayData.Rows.Count / pageSize);
            if (currentPage > totalPages) currentPage = totalPages;
            if (currentPage < 1) currentPage = 1;
            txtPage.Text = $"{currentPage} / {totalPages}";
            var paginated = displayData.AsEnumerable().Skip((currentPage - 1) * pageSize).Take(pageSize);
            dg.ItemsSource = paginated.Any() ? paginated.CopyToDataTable().DefaultView : null;
        }

        void btnTim_Click(object s, RoutedEventArgs e) => LoadData();
        void cboBang_SC(object s, SelectionChangedEventArgs e) { if (this.IsLoaded) LoadData(); }
        void btnPrev_Click(object s, RoutedEventArgs e) { if (currentPage > 1) { currentPage--; UpdateGrid(); } }
        void btnNext_Click(object s, RoutedEventArgs e) { if (currentPage < totalPages) { currentPage++; UpdateGrid(); } }

        void btnDetail_Click(object s, RoutedEventArgs e)
        {
            var r = (DataRowView)((Button)s).DataContext;
            if (r == null) return;

            txtDetailSubtitle.Text = $"Bảng: {r["TenBang"]} | Mã bản ghi: {r["MaBanGhi"]}";
            
            // Format content for better readability
            string oldC = r["NoiDungCu"].ToString() ?? "";
            string newC = r["NoiDungMoi"].ToString() ?? "";
            
            txtOld.Text = oldC.Replace(", ", "\n");
            txtNew.Text = newC.Replace(", ", "\n");
            
            txtInfo.Text = $"Người thực hiện: {r["HoTen"]} | Thời gian: {Convert.ToDateTime(r["ThoiGian"]):dd/MM/yyyy HH:mm:ss}";
            
            detailPanel.Visibility = Visibility.Visible;
        }

        void btnCloseDetail_Click(object s, RoutedEventArgs e) => detailPanel.Visibility = Visibility.Collapsed;
    }
}
