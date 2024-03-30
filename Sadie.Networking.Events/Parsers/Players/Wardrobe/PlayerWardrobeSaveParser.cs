using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Players.Wardrobe;

public class PlayerWardrobeSaveParser
{
    public int SlotId { get; private set; }
    public string FigureCode { get; private set; }
    public string Gender { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        SlotId = reader.ReadInteger();
        FigureCode = reader.ReadString();
        Gender = reader.ReadString();
    }
}