using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WeatherFrontend.Services
{
    public class AIChatService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl = "https://localhost:7125/api/"; // Adjust to your backend URL

        public AIChatService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetChatResponse(string userPrompt)
        {
            try
            {
                Console.WriteLine($"Sending prompt to backend: {userPrompt}");
                var jsonContent = new StringContent(
                    userPrompt, // Send the prompt as a raw string
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync($"{_baseApiUrl}AIChat", jsonContent);
                Console.WriteLine($"Response status: {response.StatusCode}");
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response content: {responseContent}");
                var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                return jsonResponse.GetProperty("response").GetString() ?? "No response";
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP error details: {ex.Message} - Status: {ex.StatusCode}");
                return $"An error occurred: {ex.Message}";
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON deserialization error: {ex.Message}");
                return $"An error occurred: {ex.Message}";
            }
        }
    }
}