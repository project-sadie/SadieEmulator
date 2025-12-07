using Sadie.API.DTOs.Rooms;
using Sadie.Core.Enums.Game.Rooms;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Navigator;

public class OfficialRoomEntryData
{
    [PacketData] public required int Index { get; init; }
    [PacketData] public required string PopupCaption { get; init; }
    [PacketData] public required string PopupDescription { get; init; }
    [PacketData] public required int ShowDetails { get; init; }
    [PacketData] public required string PictureText { get; init; }
    [PacketData] public required string PictureRef { get; init; }
    [PacketData] public required int FolderId { get; init; }
    [PacketData] public required int UserCount { get; init; }
    [PacketData] public required OfficialRoomEntryDataType Type { get; init; }
    [PacketData] public required string Unknown14 { get; init; }
    public RoomDto? GuestRoom { get; init; }
    public string? Tag { get; init; }
    public bool Open { get; init; }
}