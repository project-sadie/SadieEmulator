using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Catalog;

public class CatalogRecyclerLogicWriter : AbstractPacketWriter
{
    public CatalogRecyclerLogicWriter()
    {
        WriteShort(ServerPacketId.CatalogRecyclerLogic);
        WriteInteger(0); // TODO: prize size
    }   
}