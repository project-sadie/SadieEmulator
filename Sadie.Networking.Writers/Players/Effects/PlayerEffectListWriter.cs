using Sadie.API.Interfaces.Game.Players.Effects;
using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Players.Effects;

[PacketId(ServerPacketId.PlayerEffectList)]
public class PlayerEffectListWriter : AbstractPacketWriter
{
    public required List<IPlayerEffect> Effects { get; init; }

    public override void OnConfigureRules()
    {
        Override(GetType().GetProperty(nameof(Effects))!, writer =>
        {
            writer.WriteInteger(Effects.Count);

            foreach (var effect in Effects)
            {
                writer.WriteInteger(effect.Id);
                writer.WriteInteger(0);
                writer.WriteInteger(effect.Duration);
                writer.WriteInteger(-1);
                writer.WriteInteger(0);
                writer.WriteBool(effect.Duration == -1);
            }
        });
    }
}