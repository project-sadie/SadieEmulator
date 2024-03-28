using Sadie.Game.Players.Wardrobe;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;
using Sadie.Shared.Unsorted.Game.Avatar;

namespace Sadie.Networking.Writers.Players.Wardrobe;

public class PlayerWardrobeWriter : NetworkPacketWriter
{
    public PlayerWardrobeWriter(int state, Dictionary<int, PlayerWardrobeItem> outfits)
    {
        WriteShort(ServerPacketId.PlayerWardrobe);
        WriteInteger(state);
        WriteInteger(outfits.Count);

        foreach (var outfit in outfits)
        {
            WriteInteger(outfit.Key);
            WriteString(outfit.Value.FigureCode);
            WriteString(outfit.Value.Gender == AvatarGender.Male ? "M" : "F");
        }
    }
}