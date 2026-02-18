namespace Biogen.Common.Entities;

public class NeuralNetworkApiSettings
{
    public const string SectionName = "NeuralNetworkApi";

    public string BaseUrl { get; set; } = "https://gigachat.devices.sberbank.ru/";

    public string ModelName { get; set; } = "GigaChat-Pro";

    public string TokenEndpoint { get; set; } = "https://ngw.devices.sberbank.ru:9443/api/v2/oauth";

    public string AuthorizationKey { get; set; } =
        "MDE5YzcwMWYtYmY5Ni03OGQ1LWJlZmMtNDM3Zjg0YTJjZDU3OmE4OTgzNzhjLWVjNDgtNDU4My1hMDUyLTU3MzAxOGE5MDA0MQ==";

    public string Scope { get; set; } = "GIGACHAT_API_PERS";

    public string RqUID { get; set; } = "0b393d77-522a-4397-8c50-8fc94c346d15";

    public int TokenRefreshBufferMinutes { get; set; } = 5;
}