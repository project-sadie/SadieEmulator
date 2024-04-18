﻿using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Database.Models.Rooms;

public class RoomLayout
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public string? HeightMap { get; init; }
    public int DoorX { get; init; }
    public int DoorY { get; init; }
    public double DoorZ { get; init; }
    public HDirection DoorDirection { get; init; }
}