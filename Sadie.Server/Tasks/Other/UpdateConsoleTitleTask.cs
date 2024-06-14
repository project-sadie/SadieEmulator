using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Shared;

namespace SadieEmulator.Tasks.Other;

public class UpdateConsoleTitleTask(
    PlayerRepository playerRepository, 
    RoomRepository roomRepository) : IServerTask
{
    public TimeSpan PeriodicInterval => TimeSpan.FromSeconds(1);
    public DateTime LastExecuted { get; set; }

    public Task ExecuteAsync()
    {
        var usersOnline = playerRepository.Count();
        var roomCount = roomRepository.Count;
        
        Console.Title = $"Sadie {GlobalState.Version} - Started: {GlobalState.Started.ToString("HH:mm:ss")} - Players: {usersOnline} - Rooms: {roomCount}";
        
        return Task.CompletedTask;
    }
}