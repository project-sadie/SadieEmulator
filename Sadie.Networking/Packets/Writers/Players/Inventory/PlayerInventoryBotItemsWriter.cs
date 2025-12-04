using Sadie.API;
using Sadie.API.DTOs.Player;
using Sadie.API.Interfaces.Networking;
using Sadie.Core.Enums.Game.Players;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Packets.Writers.Players.Inventory;

[PacketId(ServerPacketId.PlayerInventoryBotItems)]
public class PlayerInventoryBotItemsWriter : AbstractPacketWriter
{
    public required ICollection<PlayerBotDto> Bots { get; init; }

    public override void OnSerialize(INetworkPacketWriter writer)
    {
        writer.WriteInteger(Bots.Count);

        foreach (var bot in Bots)
        {
            writer.WriteInteger(bot.Id);
            writer.WriteString(bot.Username);
            writer.WriteString(bot.Motto);
            writer.WriteString(bot.Gender == PlayerAvatarGender.Male ? "m" : "f");
            writer.WriteString(bot.FigureCode);
        }
    }
}