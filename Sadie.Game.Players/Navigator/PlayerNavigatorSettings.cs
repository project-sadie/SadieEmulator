namespace Sadie.Game.Players.Navigator;

public class PlayerNavigatorSettings(
    int windowX,
    int windowY,
    int windowWidth,
    int windowHeight,
    bool openSearches,
    int unknown1)
{
    public int WindowX { get; set; } = windowX;
    public int WindowY { get; set; } = windowY;
    public int WindowWidth { get; set; } = windowWidth;
    public int WindowHeight { get; set; } = windowHeight;
    public bool OpenSearches { get; set; } = openSearches;
    public int Unknown1 { get; set; } = unknown1;
}