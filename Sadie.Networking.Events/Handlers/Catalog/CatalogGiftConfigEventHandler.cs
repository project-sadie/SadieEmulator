﻿using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Catalog;

namespace Sadie.Networking.Events.Handlers.Catalog;

[PacketId(EventHandlerId.CatalogGiftConfig)]
public class CatalogGiftConfigEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        await client.WriteToStreamAsync(new CatalogGiftWrappingConfigWriter
        {
            Enabled = true,
            Price = 3,
            GiftWrappers = [],
            BoxTypes = [],
            RibbonTypes = [],
            GiftFurniture = [],
        });
    }
}