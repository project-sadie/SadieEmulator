namespace Sadie.Game.Players.Navigator;

public class PlayerNavigatorSettings
{
    public int WindowX { get; set; }
    public int WindowY { get; set; }
    public int WindowWidth { get; set; }
    public int WindowHeight { get; set; }
    public bool OpenSearches { get; set; }
    public int Unknown1 { get; set; }

    public PlayerNavigatorSettings(int windowX, int windowY, int windowWidth, int windowHeight, bool openSearches, int unknown1)
    {
        WindowX = windowX;
        WindowY = windowY;
        WindowWidth = windowWidth;
        WindowHeight = windowHeight;
        OpenSearches = openSearches;
        Unknown1 = unknown1;
    }
}