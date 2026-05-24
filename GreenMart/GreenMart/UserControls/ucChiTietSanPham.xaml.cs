using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using BUS;

namespace GreenMart.UserControls
{
    public partial class ucChiTietSanPham : UserControl
    {
        private DataRowView _productData;
        private KhoBUS _busKho = new();

        public ucChiTietSanPham(DataRowView product)
        {
            InitializeComponent();
            _productData = product;
            BindData();
        }

        private void BindData()
        {
            if (_productData == null) return;

            txtHeaderName.Text = _productData["TenSP"].ToString();
            txtMa.Text = _productData["MaSP"].ToString();
            txtTen.Text = _productData["TenSP"].ToString();
            txtGia.Text = string.Format("{0:#,##0} đ", _productData["DonGia"]);
            txtDVT.Text = $"Đơn vị: {_productData["DonViTinh"]}";
            txtLoai.Text = _productData["TenLoai"].ToString();
            txtNCC.Text = _productData["TenNCC"].ToString();
            txtStatus.Text = _productData["TrangThai"].ToString();
            
            string moTa = _productData["MoTa"].ToString();
            txtMoTaFull.Text = string.IsNullOrEmpty(moTa) ? "Không có mô tả chi tiết cho sản phẩm này." : moTa;
            txtMoTaShort.Text = moTa.Length > 50 ? moTa.Substring(0, 47) + "..." : moTa;

            // Load tồn kho của sản phẩm này
            try
            {
                DataTable dtKho = _busKho.HienThiTatCaChiTiet(); // Có thể cần viết thêm hàm lọc theo MaSP ở BUS/DAL để tối ưu
                // Tạm thời lọc bằng code để nhanh
                DataView dv = dtKho.DefaultView;
                dv.RowFilter = $"MaSP = '{_productData["MaSP"]}'";
                dgKho.ItemsSource = dv;

                long tongTon = 0;
                foreach (DataRowView r in dv) tongTon += Convert.ToInt64(r["SoLuong"]);
                txtTongTon.Text = $"Tổng tồn: {tongTon}";
            }
            catch { }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            // Tìm MainWindow để quay lại trang Sản phẩm
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow is MainWindow mw)
            {
                mw.pageContent.Content = new ucSanPham();
                mw.txtPageTitle.Text = "Quản lý sản phẩm";
            }
        }
    }
}
