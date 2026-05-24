using System.Threading;
using System.Windows;
using System.IO;
using System.Text.Json;

namespace GreenMart
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Phải khai báo biến static để Mutex không bị Garbage Collector dọn dẹp
        private static Mutex _mutex = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            const string appName = "GreenMart_Mutex_12345";
            bool createdNew;

            // Kiểm tra xem Mutex này đã tồn tại trong hệ thống chưa
            _mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                // Nếu Mutex đã tồn tại, nghĩa là app đang chạy
                MessageBox.Show("Ứng dụng GreenMart đang chạy rồi bạn ơi!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Đóng phiên bản mới định mở này lại
                Application.Current.Shutdown();
                return;
            }

            // --- KHỞI TẠO CÁC DỊCH VỤ BÊN THỨ 3 ---
            try
            {
                if (File.Exists("appsettings.json"))
                {
                    string jsonString = File.ReadAllText("appsettings.json");
                    using JsonDocument doc = JsonDocument.Parse(jsonString);
                    var root = doc.RootElement.GetProperty("Cloudinary");
                    string cloudName = root.GetProperty("CloudName").GetString() ?? "";
                    string apiKey = root.GetProperty("ApiKey").GetString() ?? "";
                    string apiSecret = root.GetProperty("ApiSecret").GetString() ?? "";
                    
                    if (cloudName != "your_cloud_name")
                    {
                        GreenMart.Services.CloudinaryHelper.Initialize(cloudName, apiKey, apiSecret);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải cấu hình Cloudinary từ appsettings.json: " + ex.Message, "Lỗi cấu hình", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Giải phóng Mutex khi thoát app
            if (_mutex != null)
            {
                _mutex.ReleaseMutex();
                _mutex.Dispose();
            }
            base.OnExit(e);
        }
    }
}
