namespace Sadie.API.Networking.Events.Dtos;

public interface IGroupBadgeData
{
    int GroupId { get; set; }
    string Badge { get; set; }
}