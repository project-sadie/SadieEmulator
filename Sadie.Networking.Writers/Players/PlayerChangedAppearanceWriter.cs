using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;
using Sadie.Shared.Unsorted.Game.Avatar;

namespace Sadie.Networking.Writers.Players;

public class PlayerChangedAppearanceWriter : NetworkPacketWriter
{
    public PlayerChangedAppearanceWriter(string figureCode, AvatarGender gender)
    {
        WriteShort(ServerPacketId.PlayerChangedAppearance);
        WriteString(figureCode);
        WriteString(gender == AvatarGender.Male ? "M" : "F");
    }
}