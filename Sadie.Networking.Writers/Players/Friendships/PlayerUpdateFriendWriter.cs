using Sadie.API;
using Sadie.API.Networking;
using Sadie.Enums.Unsorted;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players.Friendships;

[PacketId(ServerPacketId.PlayerUpdateFriend)]
public class PlayerUpdateFriendWriter : AbstractPacketWriter
{
    public required List<IPlayerFriendshipUpdate> Updates { get; init; }

    public override void OnSerialize(INetworkPacketWriter writer)
    {
        writer.WriteInteger(0);
        writer.WriteInteger(Updates.Count);

        foreach (var update in Updates)
        {
            writer.WriteInteger(update.Type);

            if (update.Type == -1)
            {
                writer.WriteLong(update.Friend!.Id);
            }
            else
            {
                writer.WriteLong(update.Friend.Id);
                writer.WriteString(update.Friend.Username);
                writer.WriteInteger(update.Friend.Gender == AvatarGender.Male ? 0 : 1);
                writer.WriteBool(update.FriendOnline);
                writer.WriteBool(update.FriendInRoom);
                writer.WriteString(update.Friend.FigureCode);
                writer.WriteInteger(0);
                writer.WriteString(update.Friend.Motto);
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