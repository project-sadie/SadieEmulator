using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Navigator;

public class SaveNavigatorSettingsEventParser : INetworkPacketEventParser
{
    public int WindowX { get; private set; }
    public int WindowY { get; private set; }
    public int WindowWidth { get; private set; }
    public int WindowHeight { get; private set; }
    public bool OpenSearches { get; private set; }
    public int Mode { get; private set; }
    
    public void Parse(INetworkPacketReader reader)
    {
        WindowX = reader.ReadInt();
        WindowY = reader.ReadInt();
        WindowWidth = reader.ReadInt();
        WindowHeight = reader.ReadInt();
        OpenSearches = reader.ReadBool();
        Mode = reader.ReadInt();
    }
}