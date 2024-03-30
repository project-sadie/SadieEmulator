using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Shared;

namespace SadieEmulator.Tasks.Other;

public class UpdateConsoleTitleTask(
    IPlayerRepository playerRepository, 
    IRoomRepository roomRepository) : IServerTask
{
    public TimeSpan PeriodicInterval => TimeSpan.FromSeconds(1);
    public DateTime LastExecuted { get; set; }

    public Task ExecuteAsync()
    {
        var usersOnline = playerRepository.Count();
        var roomCount = roomRepository.Count;
        
        Console.Title = $"Sadie {GlobalState.Version} - Players Online: {usersOnline} - Rooms Loaded: {roomCount}";
        
        return Task.CompletedTask;
    }
}