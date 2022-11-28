using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;
    
namespace Sadie.Networking.Writers.Catalog;

public class CatalogRecyclerLogicWriter : NetworkPacketWriter
{
    public CatalogRecyclerLogicWriter()
    {
        WriteShort(ServerPacketId.CatalogRecyclerLogic);
        WriteInteger(0); // TODO: prize size
    }   
}