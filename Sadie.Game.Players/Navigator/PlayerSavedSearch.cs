namespace Sadie.Game.Players.Navigator;

public class PlayerSavedSearch
{
    public PlayerSavedSearch(long id, string search, string filter)
    {
        Id = id;
        Search = search;
        Filter = filter;
        Unknown1 = "";
    }

    public long Id { get; }
    public string Search { get; }
    public string Filter { get; }
    public string Unknown1 { get; }
}