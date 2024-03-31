namespace Sadie.Game.Players.Club;

public class PlayerClubOfferFactory
{
    public PlayerClubOffer Create(
        int id,
        string name,
        int days,
        int costCredits,
        int costPoints,
        int costPointsType,
        bool vip)
    {
        return new PlayerClubOffer(
            id,
            name,
            days,
            costCredits,
            costPoints,
            costPointsType,
            vip);
    }
}