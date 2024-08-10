namespace Sadie.Shared;

public static class GlobalState
{
    public static readonly Version Version = AppDomain.CurrentDomain.GetAssemblies()
        .SingleOrDefault(assembly => assembly.GetName().Name == "Sadie.Server")!
        .GetName()
        .Version!;
    
    public static readonly Random Random = new((int)DateTime.Now.Ticks);
}