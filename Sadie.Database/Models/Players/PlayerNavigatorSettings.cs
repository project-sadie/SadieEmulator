using System.ComponentModel.DataAnnotations.Schema;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Database.Models.Players;

public class PlayerNavigatorSettings
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public Player? Player { get; set; }
    [PacketData] public int WindowX { get; set; }
    [PacketData] public int WindowY { get; set; }
    [PacketData] public int WindowWidth { get; set; }
    [PacketData] public int WindowHeight { get; set; }
    [PacketData] public bool OpenSearches { get; set; }
    [PacketData] [NotMapped] public int Unknown { get; set; } = 0;
}