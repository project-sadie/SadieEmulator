using Sadie.API.Game.Rooms.Bots;
using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Game.Avatar;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Rooms.Packets.Writers;

[PacketId(ServerPacketId.RoomUserData)]
public class RoomBotDataWriter : AbstractPacketWriter
{
    public required IRoomBot Bot { get; init; }

    public override void OnConfigureRules()
    {
        Override(GetType().GetProperty(nameof(Bot))!, writer =>
        {
            var botData = Bot.Bot;
            
            writer.WriteInteger(1);
            writer.WriteInteger(botData.Id);
            writer.WriteString(botData.Username);
            writer.WriteString(botData.Motto);
            writer.WriteString(botData.FigureCode);
            writer.WriteInteger(botData.Id);
            writer.WriteInteger(Bot.Point.X);
            writer.WriteInteger(Bot.Point.Y);
            writer.WriteString(Bot.PointZ + "");
            writer.WriteInteger((int) Bot.Direction);
            writer.WriteInteger(3);
            writer.WriteString(botData.Gender == AvatarGender.Male ? "M" : "F");
            writer.WriteInteger(Bot.Bot.PlayerId);
            writer.WriteString("");
            writer.WriteInteger(10);
            writer.WriteInteger(0);
            writer.WriteInteger(1);
            writer.WriteInteger(2);
            writer.WriteInteger(3);
            writer.WriteInteger(4);
            writer.WriteInteger(5);
            writer.WriteInteger(6);
            writer.WriteInteger(7);
            writer.WriteInteger(8);
            writer.WriteInteger(9);
            
        });
    }
}