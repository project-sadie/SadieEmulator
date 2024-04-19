using Sadie.Database.Models.Players;
using Sadie.Shared.Unsorted.Game.Avatar;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Wardrobe;

public class PlayerWardrobeWriter : NetworkPacketWriter
{
    public PlayerWardrobeWriter(int state, ICollection<PlayerWardrobeItem> outfits)
    {
        WriteShort(ServerPacketId.PlayerWardrobe);
        WriteInteger(state);
        WriteInteger(outfits.Count);

        var i = 0;
        
        foreach (var outfit in outfits)
        {
            i++;
            
            WriteInteger(i);
            WriteString(outfit.FigureCode);
            WriteString(outfit.Gender == AvatarGender.Male ? "M" : "F");
        }
    }
}