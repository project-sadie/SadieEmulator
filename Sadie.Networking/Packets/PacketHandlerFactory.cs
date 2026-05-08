using System.Linq.Expressions;
using Sadie.API.Interfaces.Networking.Events.Handlers;

namespace Sadie.Networking.Packets;

public class PacketHandlerFactory
{
    private readonly IServiceProvider _provider;
    private readonly Dictionary<short, Func<INetworkPacketEventHandler>> _factoryMap = new();

    public PacketHandlerFactory(IServiceProvider provider, Dictionary<short, Type> handlerTypes)
    {
        _provider = provider;

        foreach (var kv in handlerTypes)
        {
            _factoryMap[kv.Key] = BuildFactory(kv.Value);
        }
    }

    private Func<INetworkPacketEventHandler> BuildFactory(Type eventType)
    {
        var ctor = eventType.GetConstructors().Single();

        var args = ctor.GetParameters()
            .Select(p => Expression.Convert(
                Expression.Call(
                    Expression.Constant(_provider),
                    typeof(IServiceProvider).GetMethod("GetService")!,
                    Expression.Constant(p.ParameterType)
                ),
                p.ParameterType))
            .ToArray();

        var newExp = Expression.New(ctor, args);
        var lambda = Expression.Lambda<Func<INetworkPacketEventHandler>>(newExp);

        return lambda.Compile();
    }

    public INetworkPacketEventHandler Create(short id)
    {
        return _factoryMap.TryGetValue(id, out var creator) ? 
            creator() : 
            null!;
    }
}