namespace Sadie.Game.Players.Club;

public class PlayerClubOffer(
    int id,
    string name,
    int days,
    int costCredits,
    int costPoints,
    int costPointsType,
    bool vip)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
    public int Days { get; } = days;
    public int CostCredits { get; } = costCredits;
    public int CostPoints { get; } = costPoints;
    public int CostPointsType { get; } = costPointsType;
    public bool Vip { get; } = vip;
}