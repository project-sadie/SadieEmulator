using System.ComponentModel.DataAnnotations.Schema;

namespace Sadie.Database.Models.Players;

public class PlayerNavigatorSettings
{
    public int Id { get; init; }
    public int PlayerId { get; init; }
    public Player? Player { get; init; }
    public int WindowX { get; set; }
    public int WindowY { get; set; }
    public int WindowWidth { get; set; }
    public int WindowHeight { get; set; }
    public bool OpenSearches { get; set; }
    [NotMapped] public int Unknown { get; init; } = 0;
}