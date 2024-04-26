using System.ComponentModel.DataAnnotations.Schema;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Database.Models.Players;

public class PlayerNavigatorSettings
{
    [NotPacketData] public int Id { get; set; }
    [NotPacketData] public int PlayerId { get; set; }
    [NotPacketData] public Player? Player { get; set; }
    public int WindowX { get; set; }
    public int WindowY { get; set; }
    public int WindowWidth { get; set; }
    public int WindowHeight { get; set; }
    public bool OpenSearches { get; set; }
    [NotMapped] public int Unknown { get; set; } = 0;
}