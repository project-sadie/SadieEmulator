using Sadie.Shared.Unsorted.Game.Avatar;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players;

public class PlayerChangedAppearanceWriter : AbstractPacketWriter
{
    public PlayerChangedAppearanceWriter(string figureCode, AvatarGender gender)
    {
        WriteShort(ServerPacketId.PlayerChangedAppearance);
        WriteString(figureCode);
        WriteString(gender == AvatarGender.Male ? "M" : "F");
    }
}