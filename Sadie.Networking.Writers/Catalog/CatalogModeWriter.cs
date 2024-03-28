using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Catalog;

public class CatalogModeWriter : NetworkPacketWriter
{
    public CatalogModeWriter(int mode)
    {
        WriteShort(ServerPacketId.CatalogMode);
        WriteInteger(mode);
    }
}