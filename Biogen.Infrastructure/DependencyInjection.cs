using Biogen.BLL.LogicExtention;
using Biogen.Common.Entities;
using Biogen.DAl.Repository;
using Biogen.Dal.Repository.Contracts;
using Biogen.Infrastructure.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Biogen.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<NeuralNetworkApiSettings>(
            configuration.GetSection(NeuralNetworkApiSettings.SectionName));

        // HttpClient для запроса токена (без auth handler, чтобы не зациклиться)
        // Сертификат Сбербанка подписан российским УЦ — отключаем проверку цепочки
        services.AddHttpClient("OAuthTokenClient")
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            });

        // OAuthTokenService — singleton для кеширования токена
        services.AddSingleton<IOAuthTokenService>(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var options = sp.GetRequiredService<IOptions<NeuralNetworkApiSettings>>();
            var logger = sp.GetRequiredService<ILogger<OAuthTokenService>>();
            return new OAuthTokenService(factory, options, logger);
        });

        // DelegatingHandler для автоподстановки Bearer-токена
        services.AddTransient<OAuthDelegatingHandler>();

        // Typed HttpClient для NeuralNetworkClient с auth handler
        services.AddHttpClient<INeuralNetworkClient, NeuralNetworkClient>((sp, client) =>
            {
                var settings = sp.GetRequiredService<IOptions<NeuralNetworkApiSettings>>().Value;
                client.BaseAddress = new Uri(settings.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(120);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            })
            .AddHttpMessageHandler<OAuthDelegatingHandler>();

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(
            configuration.GetConnectionString("DefaultConnection"));
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<Context>(options =>
            options.UseNpgsql(dataSource));

        services.AddScoped<IRepository, Repository>();

        return services;
    }
}