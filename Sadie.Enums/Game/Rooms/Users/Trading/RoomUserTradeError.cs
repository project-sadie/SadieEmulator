namespace Sadie.Enums.Game.Rooms.Users.Trading;

public enum RoomUserTradeError
{
    TradingNotAllowed = 1,
    SelfTradingDisabled = 2,
    TargetTradingDisabled = 4,
    RoomTradingNotAllowed = 6,
    SelfAlreadyTrading = 7,
    TargetAlreadyTrading = 8
}