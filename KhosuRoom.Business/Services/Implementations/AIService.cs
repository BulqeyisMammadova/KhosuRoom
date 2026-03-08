using KhosuRoom.Business.Services.Abstractions;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text;
using System.Text.Json;

namespace KhosuRoom.Business.Services.Implementations;

internal class AIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;

    public AIService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient();
        _apiKey = configuration["AI:ApiKey"] ?? "";
        _model = configuration["AI:Model"] ?? "gemini-2.5-flash";
    }

    public async Task<List<string>> GenerateSimilarTasksAsync(string assignmentTitle, string? assignmentDescription)
    {
        var prompt = $"""
            Sən yaradıcı bir müəllimsən. Aşağıdakı tapşırığa əsasən oxşar məzmunda, lakin daha maraqlı və fərqli 1 yeni tapşırıq yaz.
            Tapşırıq çox uzun və ya çox qısa olmamalıdır (normal ölçüdə olsun).
            Tapşırığın əsas məğzini və tələbləri qısaca izah et, lakin detallara çox girmə.

            Tapşırıq başlığı: {assignmentTitle}
            {(string.IsNullOrWhiteSpace(assignmentDescription) ? "" : $"Tapşırıq izahı: {assignmentDescription}")}

            Diqqət: Nömrələmə, başqa heç bir məlumat, salamlaşma və ya əlavə söz yazma. 
            Birbaşa olaraq yalnız tapşırığın qısa mətnini qaytar.
            """;

        var url = "https://openrouter.ai/api/v1/chat/completions";

        var requestBody = new
        {
            model = _model,
            messages = new[]
            {
                new { role = "user", content = prompt }
            },
            temperature = 0.7,
            max_tokens = 1500
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

        var response = await _httpClient.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
                throw new InvalidOperationException("OpenRouter API limiti dolub (429). Bir az gözləyib yenidən cəhd edin.");

            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                throw new InvalidOperationException("OpenRouter API key-i yanlışdır. appsettings.json-dakı AI:ApiKey-i yoxlayın.");

            throw new InvalidOperationException($"AI sorğusu uğursuz oldu ({(int)response.StatusCode}). Bir az sonra yenidən cəhd edin.");
        }

        var responseJson = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseJson);

        var text = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? "";

        // Markdown ulduzlarını təmizləyirik (bəzən qalın yazmaq üçün istifadə edir)
        text = text.Replace("**", "");

        var tasks = new List<string>();
        
        // 1 tapşırıq istədiyimiz üçün heç bir parçalamaya ehtiyac yoxdur.
        // Nə qədər uzun, sətir və ya abzas olur olsun tam və bütöv halda 1 task kimi göstəriləcək.
        if (!string.IsNullOrWhiteSpace(text))
        {
            tasks.Add(text.Trim());
        }

        return tasks;
    }
}
