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
        WriteLong(playerData.Id);
        WriteString(playerData.Username);
        WriteString(playerData.FigureCode);
        WriteString(playerData.Gender == AvatarGender.Male ? "M" : "F");
        WriteString(playerData.Motto);
        WriteString(playerData.Username);
        WriteBoolean(false);
        WriteLong(playerData.RespectsReceived);
        WriteLong(playerData.RespectPoints);
        WriteLong(playerData.RespectPointsPet);
        WriteBoolean(false);
        WriteString(playerData.LastOnline.ToString());
        WriteBoolean(false);
        WriteBoolean(false);
    }
}