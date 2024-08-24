using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Dtos;

namespace Sadie.Networking.Writers.Players.Other;

[PacketId(ServerPacketId.PlayerPerks)]
public class PlayerPerksWriter : AbstractPacketWriter
{
    public required List<PerkData> Perks { get; init; }

    public override void OnConfigureRules()
    {
        Override(GetType().GetProperty(nameof(Perks))!, writer =>
        {
            writer.WriteInteger(Perks.Count);

            foreach (var item in Perks)
            {
                writer.WriteString(item.Code);
                writer.WriteString(item.ErrorMessage);
                writer.WriteBool(item.Allowed);
            }
        });
    }
}