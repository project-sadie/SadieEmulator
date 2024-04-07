namespace Sadie.Database.Models.Players;

public class PlayerBadge
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public int BadgeId { get; set; }
    public int Slot { get; set; }
}