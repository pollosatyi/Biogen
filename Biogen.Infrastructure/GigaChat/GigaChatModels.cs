using System.Text.Json.Serialization;

namespace Biogen.Infrastructure.GigaChat;

public class GigaChatRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = "";

    [JsonPropertyName("messages")]
    public List<GigaChatMessage> Messages { get; set; } = new();
}

public class GigaChatMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = "";

    [JsonPropertyName("content")]
    public string Content { get; set; } = "";

    [JsonPropertyName("attachments")]
    public List<string>? Attachments { get; set; }
}

public class GigaChatResponse
{
    [JsonPropertyName("choices")]
    public List<GigaChatChoice> Choices { get; set; } = new();
}

public class GigaChatChoice
{
    [JsonPropertyName("message")]
    public GigaChatResponseMessage Message { get; set; } = new();
}

public class GigaChatResponseMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = "";

    [JsonPropertyName("content")]
    public string Content { get; set; } = "";
}

public class GigaChatFileResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";
}
