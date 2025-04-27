using Microsoft.Extensions.Options;
using Sadie.Database.Models.Catalog.FrontPage;
using Sadie.Database.Models.Catalog.Items;
using Sadie.Database.Models.Catalog.Pages;
using Sadie.Database.Models.Furniture;
using Sadie.Database.Models.Players;
using Sadie.Enums.Game.Furniture;
using Sadie.Enums.Unsorted;
using Sadie.Networking.Events.Dtos;
using Sadie.Networking.Options;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization;
using Sadie.Networking.Writers;
using Sadie.Networking.Writers.Catalog;
using Sadie.Networking.Writers.Players.Navigator;
using Sadie.Networking.Writers.Players.Other;
using Sadie.Networking.Writers.Rooms.Users;

namespace Sadie.Tests.Networking;

public class NetworkPacketWriterSerializerTests
{
    private IOptions<NetworkPacketOptions> _options;
    
    [SetUp]
    public void SetUp()
    {
        _options = Options.Create(new NetworkPacketOptions
        {
            BufferByteSize = 4024,
            FrameLengthByteCount = 4,
            NotifyMissingPacket = false
        });
    }
    
    [Test]
    public void Serialize_RoomUserWhisperWriter_ReadsCorrectly()
    {
        var testWriter = new RoomUserChatWriter
        {
            SenderId = 3921,
            Message = "hello-world",
            EmotionId = 53,
            ChatBubbleId = 0,
            Urls = ["https://sadie.pw"],
            MessageLength = 4234
        };
        
        var writer = NetworkPacketWriterSerializer.Serialize(testWriter);
        var decoder = new NetworkPacketDecoder(_options);
        var packets = decoder.DecodePacketsFromBytes(writer.GetAllBytes());

        Assert.That(packets, Has.Count.EqualTo(1));

        var packet = packets.First();
        
        Assert.Multiple(() =>
        {
            Assert.That(packet.ReadInt(), Is.EqualTo(3921));
            Assert.That(packet.ReadString(), Is.EqualTo("hello-world"));
            Assert.That(packet.ReadInt(), Is.EqualTo(53));
            Assert.That(packet.ReadInt(), Is.EqualTo(0));
            Assert.That(packet.ReadInt(), Is.EqualTo(1));
            Assert.That(packet.ReadString(), Is.EqualTo("https://sadie.pw"));
            Assert.That(packet.ReadInt(), Is.EqualTo(4234));
        });
    }

    [Test]
    public void Serialize_NestedDataWithAttribute_ReadsCorrectly()
    {
        var writer = NetworkPacketWriterSerializer.Serialize(new PlayerPerksWriter
        {
            Perks = [new PerkData("codeTest", "Some message test", false)]
        });
        
        var decoder = new NetworkPacketDecoder(_options);
        var packets = decoder.DecodePacketsFromBytes(writer.GetAllBytes());

        Assert.That(packets, Has.Count.EqualTo(1));

        var packet = packets.First();
        
        Assert.Multiple(() =>
        {
            Assert.That(packet.ReadInt(), Is.EqualTo(1));
            Assert.That(packet.ReadString(), Is.EqualTo("codeTest"));
            Assert.That(packet.ReadString(), Is.EqualTo("Some message test"));
            Assert.That(packet.ReadBool(), Is.EqualTo(false));
        });
    }

    [Test]
    public void Serialize_NestedDataWithoutAttribute_ReadsCorrectly()
    {
        var writer = NetworkPacketWriterSerializer.Serialize(new PlayerNavigatorSettingsWriter
        {
            NavigatorSettings = new PlayerNavigatorSettings
            {
                Id = 652,
                PlayerId = 23,
                Player = null,
                WindowX = 75,
                WindowY = 34,
                WindowWidth = 45,
                WindowHeight = 213,
                OpenSearches = false,
                ResultsMode = 32
            }
        });
        
        var decoder = new NetworkPacketDecoder(_options);
        var packets = decoder.DecodePacketsFromBytes(writer.GetAllBytes());

        Assert.That(packets, Has.Count.EqualTo(1));

        var packet = packets.First();
        
        Assert.Multiple(() =>
        {
            Assert.That(packet.PacketId, Is.EqualTo(ServerPacketId.NavigatorSettings));
            Assert.That(packet.ReadInt(), Is.EqualTo(75));
            Assert.That(packet.ReadInt(), Is.EqualTo(34));
            Assert.That(packet.ReadInt(), Is.EqualTo(45));
            Assert.That(packet.ReadInt(), Is.EqualTo(213));
            Assert.That(packet.ReadBool(), Is.EqualTo(false));
            Assert.That(packet.ReadInt(), Is.EqualTo(32));
        });
    }

    [Test]
    public void Serialize_WithOnSerializeOverride_ReadsCorrectly()
    {
        var writer = NetworkPacketWriterSerializer.Serialize(new CatalogPageWriter
        {
            PageId = 392,
            CatalogMode = "edom",
            PageLayout = "frontpage4",
            Images = ["image1", "image2"],
            Texts = ["text1", "text2", "text3"],
            Items = [
                new CatalogItem
                {
                    Id = 423,
                    Name = "item1",
                    CostCredits = 1923,
                    CostPoints = 23219,
                    CostPointsType = 2,
                    FurnitureItems = [new FurnitureItem
                        {
                            Type = FurnitureItemType.Bot,
                            InteractionType = "interaction9",
                            AssetId = 39193,
                            Name = "",
                            AssetName = ""
                        }
                    ],
                    RequiresClubMembership = false,
                    MetaData = "meta",
                    Amount = 324,
                    SellLimit = 29
                }
            ],
            Unknown = 29,
            AcceptSeasonCurrencyAsCredits = false,
            FrontPageItems = [
                new CatalogFrontPageItem
                {
                    Id = 23324,
                    Title = "titles",
                    Image = "image992",
                    TypeId = CatalogFrontPageItemType.PageName,
                    ProductName = "product92934",
                    CatalogPage = new CatalogPage
                    {
                        Id = 234,
                        Name = "page-name",
                        Caption = null,
                        Layout = null,
                        RoleId = null,
                        CatalogPageId = null,
                        OrderId = 0,
                        IconId = 0,
                        Enabled = false,
                        Visible = true,
                        ImagesJson = ["image1", "image2"],
                        TextsJson = ["text1", "text2", "text3"],
                        Pages = [],
                        Items = []
                    }
                }
            ]
        });
        
        var decoder = new NetworkPacketDecoder(_options);
        var packets = decoder.DecodePacketsFromBytes(writer.GetAllBytes());

        Assert.That(packets, Has.Count.EqualTo(1));

        var packet = packets.First();
        
        Assert.Multiple(() =>
        {
            Assert.That(packet.PacketId, Is.EqualTo(ServerPacketId.CatalogPage));
            Assert.That(packet.ReadInt(), Is.EqualTo(392));
            Assert.That(packet.ReadString(), Is.EqualTo("edom"));
            Assert.That(packet.ReadString(), Is.EqualTo("frontpage4"));
            Assert.That(packet.ReadInt(), Is.EqualTo(2));
            Assert.That(packet.ReadString(), Is.EqualTo("image1"));
            Assert.That(packet.ReadString(), Is.EqualTo("image2"));
            Assert.That(packet.ReadInt(), Is.EqualTo(3));
            Assert.That(packet.ReadString(), Is.EqualTo("text1"));
            Assert.That(packet.ReadString(), Is.EqualTo("text2"));
            Assert.That(packet.ReadString(), Is.EqualTo("text3"));
            Assert.That(packet.ReadInt(), Is.EqualTo(1));
            Assert.That(packet.ReadInt(), Is.EqualTo(423));
            Assert.That(packet.ReadString(), Is.EqualTo("item1"));
            Assert.That(packet.ReadBool(), Is.EqualTo(false));
            Assert.That(packet.ReadInt(), Is.EqualTo(1923));
            Assert.That(packet.ReadInt(), Is.EqualTo(23219));
            Assert.That(packet.ReadInt(), Is.EqualTo(2));
            Assert.That(packet.ReadBool(), Is.EqualTo(false));
            Assert.That(packet.ReadInt(), Is.EqualTo(1));
            Assert.That(packet.ReadString(), Is.EqualTo("r"));
            Assert.That(packet.ReadInt(), Is.EqualTo(39193));
            Assert.That(packet.ReadString(), Is.EqualTo("meta"));
            Assert.That(packet.ReadInt(), Is.EqualTo(324));
            Assert.That(packet.ReadBool(), Is.EqualTo(false));
            Assert.That(packet.ReadInt(), Is.EqualTo(0));
            Assert.That(packet.ReadBool(), Is.EqualTo(false));
            Assert.That(packet.ReadBool(), Is.EqualTo(false));
            Assert.That(packet.ReadString(), Is.EqualTo("item1.png"));
            Assert.That(packet.ReadInt(), Is.EqualTo(29));
            Assert.That(packet.ReadBool(), Is.EqualTo(false));
            Assert.That(packet.ReadInt(), Is.EqualTo(1));
            Assert.That(packet.ReadInt(), Is.EqualTo(23324));
            Assert.That(packet.ReadString(), Is.EqualTo("titles"));
            Assert.That(packet.ReadString(), Is.EqualTo("image992"));
            Assert.That(packet.ReadInt(), Is.EqualTo(0));
            Assert.That(packet.ReadString(), Is.EqualTo("page-name"));
            Assert.That(packet.ReadInt(), Is.EqualTo(-1));
        });
    }

    [Test]
    public void Serialize_WithOnConfigureRulesOverride_ReadsCorrectly()
    {
        
    }
}