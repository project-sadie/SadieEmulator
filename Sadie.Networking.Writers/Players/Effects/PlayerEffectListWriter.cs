using Sadie.Game.Players.Effects;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Effects;

public class PlayerEffectListWriter : AbstractPacketWriter
{
    public PlayerEffectListWriter(List<PlayerEffect> effects)
    {
        WriteShort(ServerPacketId.PlayerEffectList);
        WriteInteger(effects.Count);

        foreach (var effect in effects)
        {
            WriteInteger(effect.Id);
            WriteInteger(0);
            WriteInteger(effect.Duration);
            WriteInteger(-1);
            WriteInteger(0); // ??
            WriteBool(effect.Duration == -1);
        }
    }
}