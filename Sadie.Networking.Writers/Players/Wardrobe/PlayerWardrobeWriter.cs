using Sadie.Database.Models.Players;
using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Game.Avatar;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Players.Wardrobe;

[PacketId(ServerPacketId.PlayerWardrobe)]
public class PlayerWardrobeWriter : AbstractPacketWriter
{
    public required int State { get; init; }
    public required ICollection<PlayerWardrobeItem> Outfits { get; init; }

    public override void OnConfigureRules()
    {
        Override(GetType().GetProperty(nameof(Outfits))!, writer =>
        {
            writer.WriteInteger(Outfits.Count);

            var i = 0;
        
            foreach (var outfit in Outfits)
            {
                i++;
            
                writer.WriteInteger(i);
                writer.WriteString(outfit.FigureCode);
                writer.WriteString(outfit.Gender == AvatarGender.Male ? "M" : "F");
            }
        });
    }
}