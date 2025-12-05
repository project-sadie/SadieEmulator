using Sadie.API.DTOs.Players;
using Sadie.API.Interfaces.Networking;
using Sadie.Core.Enums.Game.Players;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Packets.Writers.Players.Wardrobe;

[PacketId(ServerPacketId.PlayerWardrobe)]
public class PlayerWardrobeWriter : AbstractPacketWriter
{
    public required int State { get; init; }
    public required ICollection<PlayerWardrobeItemDto> Outfits { get; init; }

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
                writer.WriteString(outfit.Gender == PlayerAvatarGender.Male ? "M" : "F");
            }
        });
    }
}