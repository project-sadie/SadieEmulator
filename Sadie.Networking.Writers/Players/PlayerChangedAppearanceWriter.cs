using Sadie.Game.Players.Avatar;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players;

public class PlayerChangedAppearanceWriter : NetworkPacketWriter
{
    public PlayerChangedAppearanceWriter(string figureCode, PlayerAvatarGender gender)
    {
        WriteShort(ServerPacketId.PlayerChangedAppearance);
        WriteString(figureCode);
        WriteString(gender == PlayerAvatarGender.Male ? "M" : "F");
    }
}