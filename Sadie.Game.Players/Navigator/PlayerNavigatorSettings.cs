namespace Sadie.Game.Players.Navigator;

public class PlayerNavigatorSettings(
    int windowX,
    int windowY,
    int windowWidth,
    int windowHeight,
    bool openSearches,
    int mode)
{
    public int WindowX { get; set; } = windowX;
    public int WindowY { get; set; } = windowY;
    public int WindowWidth { get; set; } = windowWidth;
    public int WindowHeight { get; set; } = windowHeight;
    public bool OpenSearches { get; set; } = openSearches;
    public int Mode { get; set; } = mode;
}