using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sadie.Options.Options;
using Sadie.Options.Validation;

namespace Sadie.Options;

public static class OptionsServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        serviceCollection.AddOptions();

        serviceCollection.Configure<NetworkOptions>(options => config.GetSection("NetworkOptions").Bind(options));
        serviceCollection.Configure<NetworkPacketOptions>(options => config.GetSection("NetworkOptions:PacketOptions").Bind(options));
        serviceCollection.Configure<EncryptionOptions>(options => config.GetSection("Encryption").Bind(options));

        serviceCollection.AddSingleton<IValidateOptions<NetworkOptions>, NetworkOptionsValidator>();
        serviceCollection.AddSingleton<IValidateOptions<NetworkPacketOptions>, NetworkPacketOptionsValidator>();
    }
}
