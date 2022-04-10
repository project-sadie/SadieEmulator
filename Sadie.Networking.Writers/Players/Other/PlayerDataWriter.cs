using Sadie.Game.Players;
using Sadie.Shared.Game.Avatar;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Other;

public class PlayerDataWriter : NetworkPacketWriter
{
    public PlayerDataWriter(IPlayerData playerData)
    {
        WriteShort(ServerPacketId.PlayerData);
        WriteInt(playerData.Id);
        WriteString(playerData.Username);
        WriteString(playerData.FigureCode);
        WriteString(playerData.Gender == AvatarGender.Male ? "M" : "F");
        WriteString(playerData.Motto);
        WriteString(playerData.Username);
        WriteBoolean(false);
        WriteInt(playerData.RespectsReceived);
        WriteInt(playerData.RespectPoints);
        WriteInt(playerData.RespectPointsPet);
        WriteBoolean(false);
        WriteString(playerData.LastOnline.ToString());
        WriteBoolean(false);
        WriteBoolean(false);
    }
}