using Sadie.Game.Players.Effects;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Effects;

public class PlayerEffectListWriter : NetworkPacketWriter
{
    public PlayerEffectListWriter(List<PlayerEffect> effects) : base(ServerPacketId.PlayerEffectList)
    {
        WriteInt(effects.Count);

        foreach (var effect in effects)
        {
            WriteInt(effect.Id);
            WriteInt(0);
            WriteInt(effect.Duration);
            WriteInt(-1);
            WriteInt(0); // ??
            WriteBoolean(effect.Duration == -1);
        }
    }
}