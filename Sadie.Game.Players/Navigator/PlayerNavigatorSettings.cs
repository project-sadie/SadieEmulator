namespace Sadie.Game.Players.Navigator;

public class PlayerNavigatorSettings
{
    public int WindowX { get; }
    public int WindowY { get; }
    public int WindowWidth { get; }
    public int WindowHeight { get; }
    public bool OpenSearches { get; }
    public int Unknown1 { get; }
    
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