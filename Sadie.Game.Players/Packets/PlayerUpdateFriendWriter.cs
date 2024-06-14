using Sadie.Game.Players.Friendships;
using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Game.Avatar;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Players.Packets;

[PacketId(ServerPacketId.PlayerUpdateFriend)]
public class PlayerUpdateFriendWriter : AbstractPacketWriter
{
    public required List<PlayerFriendshipUpdate> Updates { get; init; }

    public override void OnSerialize(NetworkPacketWriter writer)
    {
        writer.WriteInteger(0);
        writer.WriteInteger(Updates.Count);

        foreach (var update in Updates)
        {
            writer.WriteInteger(update.Type);

            if (update.Type == -1)
            {
                writer.WriteInteger(update.Friend!.Id);
            }
            else
            {
                writer.WriteInteger(update.Friend.Id);
                writer.WriteString(update.Friend.Username);
                writer.WriteInteger(update.Friend.AvatarData.Gender == AvatarGender.Male ? 0 : 1);
                writer.WriteBool(update.FriendOnline);
                writer.WriteBool(update.FriendInRoom);
                writer.WriteString(update.Friend.AvatarData.FigureCode);
                writer.WriteInteger(0);
                writer.WriteString(update.Friend.AvatarData.Motto);
                writer.WriteString("");
                writer.WriteString("");
                writer.WriteBool(false);
                writer.WriteBool(false);
                writer.WriteBool(false);
                writer.WriteInteger((int) update.Relation);
            }
        }
    }
}