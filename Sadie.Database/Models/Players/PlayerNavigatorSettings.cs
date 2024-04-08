namespace Sadie.Database.Models.Players;

public class PlayerNavigatorSettings
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public Player Player { get; set; }
    public int WindowX { get; set; }
    public int WindowY { get; set; }
    public int WindowWidth { get; set; }
    public int WindowHeight { get; set; }
    public bool OpenSearches { get; set; }
    public int Mode { get; set; }
}