using Grpc.Net.Client;
using GrpcAzureAppServiceAppAuth;
using Microsoft.Extensions.Configuration;
using Grpc.Core;
using GrpcAppClientConsole;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json");

var configuration = builder.Build();

var clientCredentialAccessTokenClient 
    = new ClientCredentialAccessTokenClient(configuration, new HttpClient());

// 2. Get access token
var accessToken = await clientCredentialAccessTokenClient.GetAccessToken(
    "CC",
    "dataEventRecords",
    "cc_secret"
);

if (accessToken == null)
{
    Console.WriteLine("no auth result... ");
}
else
{
    Console.WriteLine(accessToken);

    var tokenValue = "Bearer " + accessToken;
    var metadata = new Metadata
    {
        { "Authorization", tokenValue }
    };

    var handler = new HttpClientHandler();

    var channel = GrpcChannel.ForAddress(
        configuration["ProtectedApiUrl"], 
        new GrpcChannelOptions
    {
        HttpClient = new HttpClient(handler)
        
    });

    CallOptions callOptions = new(metadata);

    var client = new Greeter.GreeterClient(channel);

    var reply = await client.SayHelloAsync(
        new HelloRequest { Name = "GreeterClient" }, callOptions);

    Console.WriteLine("Greeting: " + reply.Message);

    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
}