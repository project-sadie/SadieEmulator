namespace Sadie.Game.Players.Navigator;

public class PlayerSavedSearch(long id, string search, string filter)
{
    public long Id { get; } = id;
    public string Search { get; } = search;
    public string Filter { get; } = filter;
    public string Unknown1 { get; } = "";
}