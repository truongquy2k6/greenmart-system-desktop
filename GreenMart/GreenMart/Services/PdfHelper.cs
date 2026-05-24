using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace GreenMart.Services
{
    public static class PdfHelper
    {
        static PdfHelper()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public static void ExportInvoiceToPdf(string maHD, string thuNgan, string khachHang, DataTable chiTiet, decimal tongTien, decimal giamGia, decimal khachDua, decimal tienThua, DateTime ngayLap, string tenCuaHang = "GREEN MART", string diaChi = "123 Nguyễn Văn Linh, Q.7, TP.HCM", string sdt = "1900 1234")
        {
            try
            {
                var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"HoaDon_{maHD}.pdf");

                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        // Thiết lập kích thước khổ giấy in nhiệt 80mm, chiều dài tự động theo nội dung
                        page.ContinuousSize(80, Unit.Millimetre);
                        page.Margin(15);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(9).FontFamily(Fonts.Arial));

                        page.Content().Column(col =>
                        {
                            col.Spacing(3);

                            // --- HEADER ---
                            col.Item().AlignCenter().Text(tenCuaHang).FontSize(16).SemiBold();
                            col.Item().AlignCenter().Text(diaChi);
                            col.Item().AlignCenter().Text($"Hotline: {sdt}");
                            
                            col.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Black);

                            col.Item().AlignCenter().Text("HÓA ĐƠN BÁN LẺ").FontSize(12).SemiBold();
                            
                            // --- THÔNG TIN HÓA ĐƠN ---
                            col.Item().PaddingTop(3).Row(r => {
                                r.RelativeItem().Text($"Mã HĐ: {maHD}");
                                r.RelativeItem().AlignRight().Text($"{ngayLap:dd/MM/yyyy HH:mm}");
                            });
                            col.Item().Text($"Thu ngân: {thuNgan}");
                            col.Item().Text($"Khách hàng: {(string.IsNullOrEmpty(khachHang) ? "Khách vãng lai" : khachHang)}");

                            col.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Black);

                            // --- CHI TIẾT MẶT HÀNG ---
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(15); // SL
                                    columns.RelativeColumn();   // Tên SP
                                    columns.ConstantColumn(50); // Thành tiền
                                });

                                // Header bảng
                                table.Header(header =>
                                {
                                    header.Cell().Text("SL").SemiBold();
                                    header.Cell().Text("TÊN MẶT HÀNG").SemiBold();
                                    header.Cell().AlignRight().Text("T.TIỀN").SemiBold();
                                });

                                // Dòng chi tiết
                                foreach (DataRow row in chiTiet.Rows)
                                {
                                    string tenSP = row.Table.Columns.Contains("TenSP") ? row["TenSP"].ToString()! : "Sản phẩm";
                                    int sl = Convert.ToInt32(row["SoLuong"]);
                                    decimal donGia = Convert.ToDecimal(row["DonGia"]);
                                    decimal thanhTien = row.Table.Columns.Contains("ThanhTien") ? Convert.ToDecimal(row["ThanhTien"]) : (sl * donGia);

                                    table.Cell().PaddingTop(3).Text(sl.ToString());
                                    table.Cell().PaddingTop(3).Text(tenSP);
                                    table.Cell().PaddingTop(3).AlignRight().Text(thanhTien.ToString("#,##0"));
                                    
                                    // Hiện chi tiết đơn giá ở dòng dưới nếu số lượng > 1
                                    if (sl > 1) {
                                        table.Cell(); // Cột SL trống
                                        table.Cell().Text($"  {sl} x {donGia:#,##0}").FontSize(8).FontColor(Colors.Grey.Darken2);
                                        table.Cell(); // Cột T.Tiền trống
                                    }
                                }
                            });

                            col.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Black);

                            // --- TỔNG KẾT ---
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.ConstantColumn(70);
                                });

                                decimal tongCong = tongTien + giamGia;
                                
                                table.Cell().Text("Tổng cộng:");
                                table.Cell().AlignRight().Text($"{tongCong:#,##0}");

                                if (giamGia > 0)
                                {
                                    table.Cell().Text("Giảm giá:");
                                    table.Cell().AlignRight().Text($"-{giamGia:#,##0}");
                                }

                                table.Cell().PaddingTop(5).Text("KHÁCH PHẢI TRẢ:").SemiBold().FontSize(10);
                                table.Cell().PaddingTop(5).AlignRight().Text($"{tongTien:#,##0}").SemiBold().FontSize(11);

                                table.Cell().PaddingTop(5).Text("Khách đưa:");
                                table.Cell().PaddingTop(5).AlignRight().Text($"{khachDua:#,##0}");

                                table.Cell().Text("Tiền thừa:");
                                table.Cell().AlignRight().Text($"{tienThua:#,##0}");
                            });

                            col.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Black);

                            // --- FOOTER ---
                            col.Item().AlignCenter().Text("CẢM ƠN QUÝ KHÁCH & HẸN GẶP LẠI!").FontSize(8).SemiBold();
                            col.Item().AlignCenter().Text($"Hotline CSKH: {sdt}").FontSize(8);
                        });
                    });
                })
                .GeneratePdf(filePath);

                MessageBox.Show($"Đã xuất hóa đơn ra file PDF tại màn hình Desktop:\n{filePath}", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                // Mở file PDF ngay sau khi tạo
                var p = new Process();
                p.StartInfo = new ProcessStartInfo(filePath) { UseShellExecute = true };
                p.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi in hóa đơn: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
