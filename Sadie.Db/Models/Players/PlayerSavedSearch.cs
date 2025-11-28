using System.ComponentModel.DataAnnotations.Schema;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Db.Models.Players;

public class PlayerSavedSearch
{
    [PacketData] public int Id { get; init; }
    [PacketData] public string? Search { get; init; }
    [PacketData] public string? Filter { get; init; }
    [NotMapped] [PacketData] public string Localization => "";
    public long PlayerId { get; init; }
}