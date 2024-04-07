namespace Sadie.Database.Models.Players;

public class PlayerSavedSearch
{
    public int Id { get; set; }
    public string Search { get; set; }
    public string Filter { get; set; }
    public int PlayerId { get; set; }
}