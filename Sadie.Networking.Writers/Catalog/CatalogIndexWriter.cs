using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Catalog;

public class CatalogIndexWriter : NetworkPacketWriter
{
    public CatalogIndexWriter(int type)
    {
        WriteShort(ServerPacketId.CatalogIndex);
        WriteInteger(type);
    }
}