using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GreenMart.Services
{
    public class CloudinaryHelper
    {
        private static Cloudinary _cloudinary;

        /// <summary>
        /// Khởi tạo cấu hình Cloudinary bằng API Key của bạn
        /// Thay thế bằng thông tin thật lấy từ Dashboard Cloudinary của bạn.
        /// </summary>
        public static void Initialize(string cloudName, string apiKey, string apiSecret)
        {
            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true;
        }

        /// <summary>
        /// Upload hình ảnh lên Cloudinary
        /// </summary>
        /// <param name="filePath">Đường dẫn file ảnh trên máy tính</param>
        /// <returns>URL ảnh đã được upload thành công (hoặc null nếu lỗi)</returns>
        public static async Task<string> UploadImageAsync(string filePath)
        {
            if (_cloudinary == null)
            {
                throw new Exception("Vui lòng gọi CloudinaryHelper.Initialize() trước khi upload!");
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Không tìm thấy file ảnh!", filePath);
            }

            try
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(filePath),
                    Folder = "GreenMart", // Thư mục bạn vừa tạo trên Cloudinary
                    UseFilename = true,
                    UniqueFilename = true,
                    Overwrite = false,
                    // Tối ưu hóa dung lượng ngay khi upload
                    Transformation = new Transformation()
                        .Width(1280).Height(1280).Crop("limit") // Giới hạn độ phân giải tối đa HD (1280px)
                        .FetchFormat("auto") // Tự động chọn định dạng nhẹ nhất (vd: WebP)
                        .Quality("auto") // Nén thông minh tự động (giảm 50-80% dung lượng mà không vỡ hạt)
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    throw new Exception(uploadResult.Error.Message);
                }

                // Trả về URL bảo mật (HTTPS) của ảnh
                return uploadResult.SecureUrl.ToString();
            }
            catch (Exception ex)
            {
                // Có thể ghi log lỗi ở đây
                System.Diagnostics.Debug.WriteLine($"Lỗi upload Cloudinary: {ex.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// Xóa ảnh trên Cloudinary dựa vào URL
        /// </summary>
        public static async Task<bool> DeleteImageAsync(string imageUrl)
        {
            if (_cloudinary == null || string.IsNullOrEmpty(imageUrl)) return false;

            try
            {
                // URL thường có dạng: https://res.cloudinary.com/.../upload/v123456/GreenMart/abc.jpg
                Uri uri = new Uri(imageUrl);
                string path = uri.AbsolutePath;
                
                string[] parts = path.Split('/');
                int uploadIndex = Array.IndexOf(parts, "upload");
                if (uploadIndex == -1 || uploadIndex + 2 >= parts.Length) return false;

                int startIndex = uploadIndex + 1;
                // Bỏ qua version (thường bắt đầu bằng 'v' kèm số)
                if (parts[startIndex].StartsWith("v") && parts[startIndex].Length > 1 && char.IsDigit(parts[startIndex][1]))
                {
                    startIndex++;
                }

                string publicIdWithExtension = string.Join("/", parts, startIndex, parts.Length - startIndex);
                int lastDotIndex = publicIdWithExtension.LastIndexOf('.');
                string publicId = lastDotIndex > -1 ? publicIdWithExtension.Substring(0, lastDotIndex) : publicIdWithExtension;

                var deletionParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deletionParams);

                return result.Result == "ok";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi xóa ảnh Cloudinary: {ex.Message}");
                return false;
            }
        }
    }
}
