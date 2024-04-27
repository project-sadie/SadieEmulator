using Sadie.Game.Players.Effects;
using Sadie.Shared.Helpers;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Players.Effects;

[PacketId(ServerPacketId.PlayerEffectList)]
public class PlayerEffectListWriter : AbstractPacketWriter
{
    public required List<PlayerEffect> Effects { get; init; }

    public override void OnConfigureRules()
    {
        Override(PropertyHelper<PlayerEffectListWriter>.GetProperty(x => x.Effects), writer =>
        {
            writer.WriteInteger(Effects.Count);

            foreach (var effect in Effects)
            {
                writer.WriteInteger(effect.Id);
                writer.WriteInteger(0);
                writer.WriteInteger(effect.Duration);
                writer.WriteInteger(-1);
                writer.WriteInteger(0); // ??
                writer.WriteBool(effect.Duration == -1);
            }
        });
    }
}