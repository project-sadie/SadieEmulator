namespace Sadie.Enums.Game.Rooms.Furniture;

public enum WiredTriggerCode
{
    AvatarSaysSomething = 0,
    AvatarWalksOnFurniture = 1,
    AvatarWalksOffFurniture = 2,
    ExecuteOnce = 3,
    ToggleFurniture = 4,
    ExecutePeriodically = 6,
    AvatarEntersRoom = 7,
    GameStarts = 8,
    GameEnds = 9,
    ScoreAchieved = 10,
    Collision = 11,
    ExecutePeriodicallyLong = 12,
    BotReachedFurniture = 13,
    BotReachedAvatar = 14,
}