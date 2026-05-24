using System.Threading;
using System.Windows;

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
