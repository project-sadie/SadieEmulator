using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Catalog;

public class CatalogModeWriter : NetworkPacketWriter
{
    public CatalogModeWriter(int type)
    {
        WriteShort(ServerPacketId.CatalogMode);
        WriteInteger(type);
    }
}