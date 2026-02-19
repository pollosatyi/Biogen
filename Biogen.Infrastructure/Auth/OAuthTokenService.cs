using System.Net.Http.Headers;
using System.Net.Http.Json;
using Biogen.Common.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Biogen.Infrastructure.Auth;

public interface IOAuthTokenService
{
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}

public class OAuthTokenService : IOAuthTokenService, IDisposable
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly NeuralNetworkApiSettings _settings;
    private readonly ILogger<OAuthTokenService> _logger;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private string? _cachedToken;
    private DateTimeOffset _tokenExpiry = DateTimeOffset.MinValue;

    public OAuthTokenService(
        IHttpClientFactory httpClientFactory,
        IOptions<NeuralNetworkApiSettings> settings,
        ILogger<OAuthTokenService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        if (_cachedToken is not null && DateTimeOffset.UtcNow < _tokenExpiry)
            return _cachedToken;

        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            // Повторная проверка после захвата семафора
            if (_cachedToken is not null && DateTimeOffset.UtcNow < _tokenExpiry)
                return _cachedToken;

            _logger.LogInformation("Обновление OAuth-токена GigaChat");

            var tokenResponse = await RequestTokenAsync(cancellationToken);
            _cachedToken = tokenResponse.AccessToken;
            _tokenExpiry = DateTimeOffset.FromUnixTimeMilliseconds(tokenResponse.ExpiresAt)
                .AddMinutes(-_settings.TokenRefreshBufferMinutes);

            return _cachedToken;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<OAuthTokenResponse> RequestTokenAsync(CancellationToken cancellationToken)
    {
        using var client = _httpClientFactory.CreateClient("OAuthTokenClient");

        using var request = new HttpRequestMessage(HttpMethod.Post, _settings.TokenEndpoint);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.Add("RqUID", _settings.RqUID);
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", _settings.AuthorizationKey);

        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["scope"] = _settings.Scope
        });

        var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<OAuthTokenResponse>(cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("OAuth-ответ от GigaChat был null");
    }

    public void Dispose() => _semaphore.Dispose();
}
