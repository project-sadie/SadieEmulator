namespace Sadie.Game.Players.Navigator;

public class PlayerSavedSearch
{
    public PlayerSavedSearch(int id, string search, string filter)
    {
        Id = id;
        Search = search;
        Filter = filter;
        Unknown1 = "";
    }

    public int Id { get; }
    public string Search { get; }
    public string Filter { get; }
    public string Unknown1 { get; }
}