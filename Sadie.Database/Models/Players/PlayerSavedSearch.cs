using System.ComponentModel.DataAnnotations.Schema;
using Sadie.Shared.Attributes;

namespace Sadie.Database.Models.Players;

public class PlayerSavedSearch
{
    [PacketData] public int Id { get; init; }
    [PacketData] public string? Search { get; init; }
    [PacketData] public string? Filter { get; init; }
    [NotMapped] [PacketData] public string Localization => "";
    public int PlayerId { get; init; }
}