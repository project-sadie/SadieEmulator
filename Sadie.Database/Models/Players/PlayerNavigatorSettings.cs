namespace Sadie.Database.Models.Players;

public class PlayerNavigatorSettings
{
    public Player Player { get; set; }
    public int WindowX { get; set; }
    public int WindowY { get; set; }
    public int WindowWidth { get; set; }
    public int WindowHeight { get; set; }
    public bool OpenSearches { get; set; }
}