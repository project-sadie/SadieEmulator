using Sadie.API.Networking.Events.Dtos;

namespace Sadie.Networking.Events.Dtos;

public class GroupBadgeData : IGroupBadgeData
{
    public int GroupId { get; set; }
    public required string Badge { get; set; }
}