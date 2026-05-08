using System.ComponentModel.DataAnnotations;

namespace Sadie.Db.Models.Players;

public class PlayerSsoToken
{
    public int Id { get; init; }
    public long PlayerId { get; init; }
    [MaxLength(200)] public string? Token { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset ExpiresAt { get; init; }
    public DateTimeOffset? UsedAt { get; set; }
}