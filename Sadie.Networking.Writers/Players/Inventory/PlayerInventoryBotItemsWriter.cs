using Sadie.API;
using Sadie.API.Networking;
using Sadie.Database.Models.Players;
using Sadie.Enums.Unsorted;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players.Inventory;

[PacketId(ServerPacketId.PlayerInventoryBotItems)]
public class PlayerInventoryBotItemsWriter : AbstractPacketWriter
{
    public required ICollection<PlayerBot> Bots { get; init; }

    public override void OnSerialize(INetworkPacketWriter writer)
    {
        writer.WriteInteger(Bots.Count);

        foreach (var bot in Bots)
        {
            writer.WriteInteger(bot.Id);
            writer.WriteString(bot.Username);
            writer.WriteString(bot.Motto);
            writer.WriteString(bot.Gender == AvatarGender.Male ? "m" : "f");
            writer.WriteString(bot.FigureCode);
        }
    }
}