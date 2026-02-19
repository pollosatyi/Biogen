using System.Net.Http.Json;
using System.Text.Json;
using Biogen.BLL.LogicExtention;
using Biogen.Common.Entities;
using Biogen.Infrastructure.GigaChat;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Biogen.Infrastructure;

public class NeuralNetworkClient : INeuralNetworkClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NeuralNetworkClient> _logger;
    private readonly NeuralNetworkApiSettings _settings;

    public NeuralNetworkClient(
        HttpClient httpClient,
        ILogger<NeuralNetworkClient> logger,
        IOptions<NeuralNetworkApiSettings> settings)
    {
        _httpClient = httpClient;
        _logger = logger;
        _settings = settings.Value;
    }

    public async Task<ImageDetectionOutcome> DetectImageContentAsync(
        byte[] imageData,
        string prompt,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Отправка изображения ({Size} байт) в GigaChat", imageData.Length);

        var fileId = await UploadImageAsync(imageData, cancellationToken);

        var request = new GigaChatRequest
        {
            Model = _settings.ModelName,
            Messages = new List<GigaChatMessage>
            {
                new GigaChatMessage
                {
                    Role = "user",
                    Content = prompt,
                    Attachments = new List<string> { fileId }
                }
            }
        };

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.PostAsJsonAsync("/api/v1/chat/completions", request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("GigaChat вернул {StatusCode}. Тело ответа: {Body}",
                    (int)response.StatusCode, errorBody);
            }

            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка HTTP-запроса к GigaChat");
            throw;
        }

        GigaChatResponse gigaChatResponse;
        try
        {
            gigaChatResponse = await response.Content.ReadFromJsonAsync<GigaChatResponse>(
                cancellationToken: cancellationToken)
                ?? throw new InvalidOperationException("GigaChat вернул null-ответ");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка десериализации ответа GigaChat");
            throw;
        }

        var content = gigaChatResponse.Choices.FirstOrDefault()?.Message.Content
            ?? throw new InvalidOperationException("GigaChat не вернул контент");

        _logger.LogInformation("Ответ GigaChat получен, парсинг результата");

        try
        {
            return JsonSerializer.Deserialize<ImageDetectionOutcome>(content)
                ?? throw new InvalidOperationException("Не удалось десериализовать ответ GigaChat в ImageDetectionOutcome");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка парсинга JSON из ответа GigaChat. Содержимое: {Content}", content);
            throw;
        }
    }

    private async Task<string> UploadImageAsync(byte[] imageData, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Загрузка изображения в GigaChat Files API");

        var imageContent = new ByteArrayContent(imageData);
        imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

        using var content = new MultipartFormDataContent();
        content.Add(imageContent, "file", "image.jpg");
        content.Add(new StringContent("general"), "purpose");

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.PostAsync("/api/v1/files", content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Ошибка загрузки файла {StatusCode}. Тело: {Body}",
                    (int)response.StatusCode, errorBody);
            }

            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка загрузки изображения в GigaChat Files API");
            throw;
        }

        var fileResponse = await response.Content.ReadFromJsonAsync<GigaChatFileResponse>(
            cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("GigaChat Files API вернул null");

        _logger.LogInformation("Изображение загружено, file_id: {FileId}", fileResponse.Id);

        return fileResponse.Id;
    }
}
