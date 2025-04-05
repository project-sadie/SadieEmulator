using System.ComponentModel.DataAnnotations.Schema;
using Sadie.Shared.Attributes;

namespace Sadie.Database.Models.Players;

public class PlayerNavigatorSettings
{
    public int Id { get; init; }
    public long PlayerId { get; init; }
    public Player? Player { get; init; }
    [PacketData] public int WindowX { get; set; }
    [PacketData] public int WindowY { get; set; }
    [PacketData] public int WindowWidth { get; set; }
    [PacketData] public int WindowHeight { get; set; }
    [PacketData] public bool OpenSearches { get; set; }
    [PacketData] [NotMapped] public int ResultsMode { get; set; }
}