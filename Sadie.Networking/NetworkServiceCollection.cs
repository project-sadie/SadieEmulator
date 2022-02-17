using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sadie.Networking.Client;

namespace Sadie.Networking;

public class NetworkServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        serviceCollection.AddSingleton(new TcpListener(
            IPAddress.Parse(config.GetValue<string>("Networking:Host")), 
            config.GetValue<int>("Networking:Port"))  
        );
            
        serviceCollection.AddSingleton<INetworkClientFactory, NetworkClientFactory>();
        serviceCollection.AddSingleton<INetworkClientRepository, NetworkClientRepository>();
        serviceCollection.AddTransient<INetworkClient, NetworkClient>();
        serviceCollection.AddSingleton<INetworkListener, NetworkListener>();
    }
}