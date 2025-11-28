using System.ComponentModel.DataAnnotations.Schema;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Db.Models.Players;

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