using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace GreenMart.Services
{
    public class GeminiService
    {
        private string _apiKey = "";
        private readonly string _endpoint;

        public GeminiService()
        {
            string apiUrl = "https://greenmart-api.onrender.com/";
            try
            {
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                if (File.Exists(configPath))
                {
                    string jsonString = File.ReadAllText(configPath);
                    using JsonDocument doc = JsonDocument.Parse(jsonString);
                    if (doc.RootElement.TryGetProperty("ApiUrl", out var urlElem))
                    {
                        apiUrl = urlElem.GetString() ?? apiUrl;
                    }
                }
            }
            catch { }
            
            if (!apiUrl.EndsWith("/")) apiUrl += "/";
            _endpoint = $"{apiUrl}api/chatbot";
        }

        public async Task<string> SendMessageAsync(string userMessage, string contextData)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // Add header to tell the backend to use the Desktop Gemini API Key
                    client.DefaultRequestHeaders.Add("X-Client-Type", "Desktop");

                    var requestBody = new
                    {
                        system_instruction = new
                        {
                            parts = new[]
                            {
                                new
                                {
                                    text = @"Bạn là AI Trợ lý ảo độc quyền của hệ thống phần mềm quản lý cửa hàng GreenMart. 
Nhiệm vụ DUY NHẤT của bạn là:
1. Hướng dẫn sử dụng phần mềm GreenMart (cách bán hàng, xem tồn kho, thêm nhân viên...).
2. Báo cáo và thống kê dữ liệu cửa hàng dựa trên ngữ cảnh được cung cấp.

RÀNG BUỘC NGHIÊM NGẶT: 
- KHÔNG BAO GIỜ trả lời các câu hỏi ngoài lề (như kiến thức chung, toán học, lập trình, nấu ăn, lịch sử, v.v.).
- Nếu người dùng hỏi ngoài lề, hãy trả lời ngắn gọn: 'Tôi là trợ lý AI của GreenMart. Tôi chỉ hỗ trợ hướng dẫn sử dụng phần mềm và thống kê dữ liệu cửa hàng. Xin vui lòng đặt câu hỏi liên quan.'
- Luôn xưng hô là 'Tôi' và gọi người dùng là 'Sếp' hoặc 'Bạn'.
- Trả lời ngắn gọn, chuyên nghiệp, format markdown rõ ràng."
                                }
                            }
                        },
                        contents = new[]
                        {
                            new
                            {
                                  parts = new[]
                                  {
                                      new { text = $"[Dữ liệu hệ thống hiện tại (Hãy dùng để trả lời nếu cần thống kê)]: {contextData}\n\n[Câu hỏi của người dùng]: {userMessage}" }
                                  }
                            }
                        }
                    };

                    var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(_endpoint, jsonContent);
                    var responseString = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        using (JsonDocument doc = JsonDocument.Parse(responseString))
                        {
                            var text = doc.RootElement
                                .GetProperty("candidates")[0]
                                .GetProperty("content")
                                .GetProperty("parts")[0]
                                .GetProperty("text").GetString();
                            return text ?? "Không có câu trả lời.";
                        }
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        return "Bot đang nhận quá nhiều yêu cầu cùng lúc (vượt giới hạn API miễn phí). Vui lòng đợi khoảng 1 phút rồi thử lại nhé!";
                    }
                    else
                    {
                        return $"[Lỗi API]: {response.StatusCode}\nChi tiết: {responseString}";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"[Lỗi hệ thống]: {ex.Message}";
            }
        }
    }
}
