using Sadie.Shared.Unsorted.Game;
using Sadie.Shared.Unsorted.Game.Avatar;

namespace Sadie.Database.Models.Players;

public class PlayerAvatarData
{
    public int Id { get; set; }
    public Player Player { get; set; }
    public string FigureCode { get; set; }
    public string Motto { get; set; }
    public AvatarGender Gender { get; set; }
    public ChatBubble ChatBubbleId { get; set; }
}