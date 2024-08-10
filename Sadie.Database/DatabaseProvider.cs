using Microsoft.Extensions.DependencyInjection;

namespace Sadie.Database;

public class DatabaseProvider(IServiceProvider serviceProvider)
{
    public SadieContext GetContextInstance()
    {
        return serviceProvider.GetRequiredService<SadieContext>();
    }
}