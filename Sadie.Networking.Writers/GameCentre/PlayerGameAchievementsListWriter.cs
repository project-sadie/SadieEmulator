using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.GameCentre;

[PacketId(ServerPacketId.GameCentreConfig)]
public class PlayerGameAchievementsListWriter : AbstractPacketWriter;