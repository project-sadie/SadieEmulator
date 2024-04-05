using System.Net;
using System.Security.Cryptography.X509Certificates;
using Fleck;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sadie.Networking.Client;

namespace Sadie.Networking;

public static class NetworkServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        var host = config["Networking:Host"] ?? IPAddress.Any.ToString();
        var port = int.Parse(config["Networking:Port"]);
            
        serviceCollection.AddSingleton<INetworkClientFactory, NetworkClientFactory>();
        serviceCollection.AddSingleton<INetworkClientRepository, NetworkClientRepository>();
            
        var certificateLocation = config["Networking:CertificateFile"];
            
        if (certificateLocation != null)
        {
            var certificate = new X509Certificate2(certificateLocation, "");
            serviceCollection.AddSingleton<X509Certificate2>(provider => certificate);
        }

        // Shut fleck up 
        FleckLog.LogAction = (x, y, z) => { };
        
        serviceCollection.AddSingleton<WebSocketServer>(provider => new WebSocketServer($"wss://{host}:{port}"));
        serviceCollection.AddTransient<INetworkClient, NetworkClient>();
            
        serviceCollection.AddTransient<INetworkClient, NetworkClient>();
        serviceCollection.AddSingleton<INetworkListener, NetworkListener>();

        var networkConstants = new NetworkingConstants();
        config.GetSection("Constants:Networking").Bind(networkConstants);
        serviceCollection.AddSingleton(networkConstants);
    }
}