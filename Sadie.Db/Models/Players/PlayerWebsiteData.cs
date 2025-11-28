namespace Sadie.Db.Models.Players;

public class PlayerWebsiteData
{
    public ulong Id { get; set; }
    public required string InitialIp { get; set; }
    public required string LastIp { get; set; }
    public DateTimeOffset LastLogin { get; set; }
    public required Player Player { get; set; }
}