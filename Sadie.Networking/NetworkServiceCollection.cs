using Fleck;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sadie.Networking.Client;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Sadie.Networking;

public static class NetworkServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        var host = config["Networking:Host"] ?? IPAddress.Any.ToString();
        var port = int.Parse(config["Networking:Port"]);
        var useWss = bool.Parse(config["Networking:UseWss"]);

        serviceCollection.AddSingleton<INetworkClientFactory, NetworkClientFactory>();
        serviceCollection.AddSingleton<INetworkClientRepository, NetworkClientRepository>();

        // Shut fleck up 
        FleckLog.LogAction = (x, y, z) => { };

        var websocketLocation = useWss ? $"wss://{host}:{port}" : $"ws://{host}:{port}";
        var wss = new WebSocketServer(websocketLocation, useWss);
        var certificateLocation = config["Networking:CertificateFile"];

        if (useWss && !string.IsNullOrEmpty(certificateLocation))
        {
            wss.Certificate = new X509Certificate2(certificateLocation, "");
        }

        serviceCollection.AddSingleton<WebSocketServer>(provider => wss);
        serviceCollection.AddTransient<INetworkClient, NetworkClient>();

        serviceCollection.AddTransient<INetworkClient, NetworkClient>();
        serviceCollection.AddSingleton<INetworkListener, NetworkListener>();
    }
}