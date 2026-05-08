using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.GameCentre;

[PacketId(ServerPacketId.GameCentreConfig)]
public class PlayerGameAchievementsListWriter : AbstractPacketWriter;