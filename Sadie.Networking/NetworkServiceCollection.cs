using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sadie.Networking.Client;
using Sadie.Networking.WebSockets;

namespace Sadie.Networking;

public class NetworkServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        var host = config["Networking:Host"] ?? IPAddress.Any.ToString();
        var port = int.Parse(config["Networking:Port"]);
            
        serviceCollection.AddSingleton<INetworkClientFactory, NetworkClientFactory>();
        serviceCollection.AddSingleton<INetworkClientRepository, NetworkClientRepository>();

        var useNitroWebSockets = config.GetValue<bool>("Networking:UseWebSockets");

        if (useNitroWebSockets)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add($"http://{host}:{port}/");
            
            serviceCollection.AddSingleton(listener);
            serviceCollection.AddTransient<INetworkClient, WsNetworkNetworkClient>();
            serviceCollection.AddSingleton<INetworkListener, WsNetworkListener>();
        }
        else
        {
            serviceCollection.AddSingleton(new TcpListener(
                IPAddress.Parse(host), port
            ));
            
            serviceCollection.AddTransient<INetworkClient, NetworkClient>();
            serviceCollection.AddSingleton<INetworkListener, NetworkListener>();
        }

        var networkConstants = new NetworkingConstants();
        config.GetSection("Constants:Networking").Bind(networkConstants);
        serviceCollection.AddSingleton(networkConstants);
    }
}