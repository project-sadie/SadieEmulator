using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Players;

public class PlayerChangedAppearanceParser
{
    public string Gender { get; private set; }
    public string FigureCode { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        Gender = reader.ReadString();
        FigureCode = reader.ReadString();
    }
}