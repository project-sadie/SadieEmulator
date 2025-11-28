using System.ComponentModel;
using Sadie.Core.Enums.Game.Players;
using Sadie.Core.Enums.Miscellaneous;

namespace Sadie.Db.Models.Players;

public class PlayerBot
{
    public int Id { get; init; }
    public required long PlayerId { get; init; }
    public required int? RoomId { get; set; }
    public required string Username { get; init; }
    public required string FigureCode { get; init; }
    public required string Motto { get; init; }
    public required PlayerAvatarGender Gender { get; init; }
    [DefaultValue(0)] public ChatBubble ChatBubbleId { get; init; }
    public required DateTime CreatedAt { get; init; }
}