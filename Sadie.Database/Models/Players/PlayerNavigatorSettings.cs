using System.ComponentModel.DataAnnotations.Schema;
using Sadie.Shared.Attributes;

namespace Sadie.Database.Models.Players;

public class PlayerNavigatorSettings
{
    [PacketData] public int Id { get; init; }
    [PacketData] public int PlayerId { get; init; }
    public Player? Player { get; init; }
    [PacketData] public int WindowX { get; set; }
    [PacketData] public int WindowY { get; set; }
    [PacketData] public int WindowWidth { get; set; }
    [PacketData] public int WindowHeight { get; set; }
    [PacketData] public bool OpenSearches { get; set; }
    [NotMapped] public int Unknown { get; init; } = 0;
}