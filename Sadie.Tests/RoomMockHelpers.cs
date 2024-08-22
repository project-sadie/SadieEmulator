using Sadie.Database.Models.Players;
using Sadie.Database.Models.Rooms;

namespace Sadie.Tests;

public class RoomMockHelpers
{
    protected static Room MockRoomWithName(string name)
    {
        return new Room
        {
            Name = name,
            Description = ""
        };
    }
    
    protected static Room MockRoomWithTag(string tag)
    {
        return new Room
        {
            Name = "",
            Description = "",
            Tags = new List<RoomTag>
            {
                new()
                {
                    Name = tag
                }
            }
        };
    }
    
    protected static Room MockRoomWithOwner(string username)
    {
        return new Room
        {
            Name = "",
            Description = "",
            Owner = new Player
            {
                Username = username,
                Email = ""
            }
        };
    }
}