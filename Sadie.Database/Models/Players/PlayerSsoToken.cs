namespace Sadie.Database.Models.Players;

public class PlayerSsoToken
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public string Token { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? UsedAt { get; set; }
}