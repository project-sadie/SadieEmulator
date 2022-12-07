using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Catalog;

public class CatalogModeWriter : NetworkPacketWriter
{
    public CatalogModeWriter(int mode)
    {
        WriteShort(ServerPacketId.CatalogMode);
        WriteInteger(mode);
    }
}