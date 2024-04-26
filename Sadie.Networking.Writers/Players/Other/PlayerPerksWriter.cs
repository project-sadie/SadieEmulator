using Sadie.Game.Players;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Other;

public class PlayerPerksWriter : AbstractPacketWriter
{
    public PlayerPerksWriter(List<PlayerPerk> perks)
    {
        WriteShort(ServerPacketId.PlayerPerks);
        WriteInteger(perks.Count);

        foreach (var perk in perks)
        {
            WriteString(perk.Code);
            WriteString(perk.ErrorMessage);
            WriteBool(perk.Allowed);
        }
    }
}