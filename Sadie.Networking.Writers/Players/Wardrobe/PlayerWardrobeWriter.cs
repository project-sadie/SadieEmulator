using Sadie.Game.Players.Wardrobe;
using Sadie.Shared.Game.Avatar;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Wardrobe;

public class PlayerWardrobeWriter : NetworkPacketWriter
{
    public PlayerWardrobeWriter(Dictionary<int, PlayerWardrobeItem> outfits)
    {
        WriteShort(ServerPacketId.PlayerWardrobe);
        WriteInteger(1);
        WriteInteger(outfits.Count);

        foreach (var outfit in outfits)
        {
            WriteInteger(outfit.Key);
            WriteString(outfit.Value.FigureCode);
            WriteString(outfit.Value.Gender == AvatarGender.Male ? "M" : "F");
        }
    }
}