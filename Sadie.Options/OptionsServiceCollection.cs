﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sadie.Options.Models;
using Sadie.Options.Validation;

namespace Sadie.Options;

public static class OptionsServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        serviceCollection.AddOptions();

        serviceCollection.Configure<DatabaseOptions>(options => config.GetSection("DatabaseOptions").Bind(options));
        serviceCollection.Configure<NetworkOptions>(options => config.GetSection("NetworkOptions").Bind(options));
        serviceCollection.Configure<NetworkPacketOptions>(options => config.GetSection("NetworkOptions:PacketOptions").Bind(options));

        serviceCollection.AddSingleton<IValidateOptions<DatabaseOptions>, DatabaseOptionsValidator>();
        serviceCollection.AddSingleton<IValidateOptions<NetworkOptions>, NetworkOptionsValidator>();
        serviceCollection.AddSingleton<IValidateOptions<NetworkPacketOptions>, NetworkPacketOptionsValidator>();
    }
}
