using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sadie.Networking.Client;
using Sadie.Networking.Validators;
using NetworkOptions = Sadie.Networking.Options.NetworkOptions;
using NetworkPacketOptions = Sadie.Networking.Options.NetworkPacketOptions;

namespace Sadie.Networking;

public static class NetworkServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        serviceCollection.AddSingleton<INetworkClientFactory, NetworkClientFactory>();
        serviceCollection.AddSingleton<INetworkClientRepository, NetworkClientRepository>();

        serviceCollection.AddTransient<INetworkClient, NetworkClient>();

        serviceCollection.AddTransient<INetworkClient, NetworkClient>();
        serviceCollection.AddSingleton<INetworkListener, NetworkListener>();
        
        serviceCollection.Configure<NetworkOptions>(options => config.GetSection("NetworkOptions").Bind(options));
        serviceCollection.Configure<NetworkPacketOptions>(options => config.GetSection("NetworkOptions:PacketOptions").Bind(options));

        serviceCollection.AddSingleton<IValidateOptions<NetworkOptions>, NetworkOptionsValidator>();
        serviceCollection.AddSingleton<IValidateOptions<NetworkPacketOptions>, NetworkPacketOptionsValidator>();
    }
}