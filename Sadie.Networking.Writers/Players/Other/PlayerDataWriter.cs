using Sadie.Game.Players;
using Sadie.Shared.Unsorted.Game.Avatar;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Other;

public class PlayerDataWriter : NetworkPacketWriter
{
    public PlayerDataWriter(IPlayerData playerData)
    {
        WriteShort(ServerPacketId.PlayerData);
        WriteInteger(playerData.Id);
        WriteString(playerData.Username);
        WriteString(playerData.FigureCode);
        WriteString(playerData.Gender == AvatarGender.Male ? "M" : "F");
        WriteString(playerData.Motto);
        WriteString(playerData.Username);
        WriteBool(false);
        WriteInteger(playerData.RespectsReceived);
        WriteInteger(playerData.RespectPoints);
        WriteInteger(playerData.RespectPointsPet);
        WriteBool(false);
        WriteString(playerData.LastOnline.ToString());
        WriteBool(false);
        WriteBool(false);
    }
}