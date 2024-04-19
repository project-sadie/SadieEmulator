using Microsoft.Extensions.DependencyInjection;
using Sadie.Networking.Client;

namespace Sadie.Networking;

public static class NetworkServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<INetworkClientFactory, NetworkClientFactory>();
        serviceCollection.AddSingleton<INetworkClientRepository, NetworkClientRepository>();

        serviceCollection.AddTransient<INetworkClient, NetworkClient>();

        serviceCollection.AddTransient<INetworkClient, NetworkClient>();
        serviceCollection.AddSingleton<INetworkListener, NetworkListener>();
    }
}