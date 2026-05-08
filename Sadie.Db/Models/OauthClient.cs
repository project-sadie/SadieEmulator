using System.ComponentModel.DataAnnotations;

namespace Sadie.Db.Models;

public class OauthClient
{
    public int Id { get; init; }
    [MaxLength(256)] public required string Secret { get; init; }
    [MaxLength(120)] public required string Domain { get; init; }
}