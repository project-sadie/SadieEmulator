using Microsoft.Extensions.DependencyInjection;

namespace Sadie.Networking.Encryption;

public static class NetworkEncryptionServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<HabboEncryption>();
    }
}
