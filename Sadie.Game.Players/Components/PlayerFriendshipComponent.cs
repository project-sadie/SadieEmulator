namespace Sadie.Game.Players.Friendships;

public class PlayerFriendshipComponent
{
    private readonly int _playerId;

    public PlayerFriendshipComponent(int playerId, List<PlayerFriendship> friendships)
    {
        _playerId = playerId;
        Friendships = friendships;
    }

    public List<PlayerFriendship> Friendships { get; set; }

    public bool IsFriendsWith(int playerId)
    {
        return Friendships.Any(x => x.OriginId == playerId || x.TargetId == playerId);
    }

    public void AcceptIncomingRequest(int originId)
    {
        var request = Friendships.FirstOrDefault(x => x.TargetId == _playerId && x.OriginId == originId);

        if (request == null)
        {
            return;
        }
        
        request.Status = PlayerFriendshipStatus.Accepted;
    }

    public void DeclineIncomingRequest(int originId)
    {
        Friendships.RemoveAll(x => x.TargetId == _playerId && x.OriginId == originId);
    }

    public void OutgoingRequestAccepted(int targetId)
    {
        var request = Friendships.FirstOrDefault(x => x.OriginId == _playerId && x.TargetId == targetId);

        if (request == null)
        {
            return;
        }
        
        request.Status = PlayerFriendshipStatus.Accepted;
    }

    public void OutgoingRequestDeclined(int targetId)
    {
        Friendships.RemoveAll(x => x.OriginId == _playerId && x.TargetId == targetId);
    }

    public void RemoveFriend(int targetId)
    {
        Friendships.RemoveAll(x => x.OriginId == targetId || x.TargetId == targetId);
    }

    public void RemoveFriends(List<int> targetIds)
    {
        targetIds.ForEach(RemoveFriend);
    }
}