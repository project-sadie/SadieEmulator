using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Catalog;

public class CatalogModeWriter : AbstractPacketWriter
{
    public CatalogModeWriter(int mode)
    {
        WriteShort(ServerPacketId.CatalogMode);
        WriteInteger(mode);
    }
}