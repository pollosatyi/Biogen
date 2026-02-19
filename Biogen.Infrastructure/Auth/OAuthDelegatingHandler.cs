using System.Net.Http.Headers;

namespace Biogen.Infrastructure.Auth;

public class OAuthDelegatingHandler : DelegatingHandler
{
    private readonly IOAuthTokenService _tokenService;

    public OAuthDelegatingHandler(IOAuthTokenService tokenService)
    {
        _tokenService = tokenService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await _tokenService.GetAccessTokenAsync(cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, cancellationToken);
    }
}
