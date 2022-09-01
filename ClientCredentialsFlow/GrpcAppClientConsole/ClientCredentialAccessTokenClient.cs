using IdentityModel.Client;
using Microsoft.Extensions.Configuration;

namespace GrpcAppClientConsole;

public class ClientCredentialAccessTokenClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ClientCredentialAccessTokenClient(
        IConfiguration configuration,
        HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
    }


    public async Task<string> GetAccessToken(string api_name, string api_scope, string secret)
    {
        try
        {
            var disco = await HttpClientDiscoveryExtensions.GetDiscoveryDocumentAsync(
                _httpClient,
                _configuration["OpenIDConnectSettings:Authority"]);

            if (disco.IsError)
            {
                Console.WriteLine($"disco error Status code: {disco.IsError}, Error: {disco.Error}");
                throw new ApplicationException($"Status code: {disco.IsError}, Error: {disco.Error}");
            }

            var tokenResponse = await HttpClientTokenRequestExtensions.RequestClientCredentialsTokenAsync(_httpClient, new ClientCredentialsTokenRequest
            {
                Scope = api_scope,
                ClientSecret = secret,
                Address = disco.TokenEndpoint,
                ClientId = api_name
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine($"tokenResponse.IsError Status code: {tokenResponse.IsError}, Error: {tokenResponse.Error}");
                throw new ApplicationException($"Status code: {tokenResponse.IsError}, Error: {tokenResponse.Error}");
            }

            return tokenResponse.AccessToken;
            
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception {e}");
            throw new ApplicationException($"Exception {e}");
        }
    }
}
