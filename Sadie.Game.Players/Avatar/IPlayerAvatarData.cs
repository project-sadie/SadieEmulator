namespace Sadie.Game.Players;

public interface IPlayerAvatarData
{
    string FigureCode { get; }
    string Motto { get; }
    PlayerAvatarGender Gender { get; }
    long LastRoomLoaded { get; set; }
}