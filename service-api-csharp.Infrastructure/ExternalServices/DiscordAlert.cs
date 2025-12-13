namespace service_api_csharp.Infrastructure.ExternalServices;

public class DiscordAlert
{
    private readonly HttpClient _client;
    
    public DiscordAlert(IHttpClientFactory client)
    {
        _client = client.CreateClient("DiscordAlert");  
    }
}