namespace Blazor.BFF.OpenIddict.Client.Services;

public interface IAntiforgeryHttpClientFactory
{
    Task<HttpClient> CreateClientAsync(string clientName = "authorizedClient");
}
