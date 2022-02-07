using Sadie.Game.Players.Effects;

namespace Sadie.Networking.Packets.Server.Players.Effects;

internal class PlayerEffectListWriter : NetworkPacketWriter
{
    internal PlayerEffectListWriter(List<PlayerEffect> effects) : base(ServerPacketId.PlayerEffectList)
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