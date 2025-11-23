using System.Diagnostics;
using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Game.Rooms;

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
        var started = Process.GetCurrentProcess().StartTime;
        var assembly = typeof(Server).Assembly;
        var version = assembly.GetName().Version;
        
        Console.Title = $"Sadie {version} - Started: {started:HH:mm:ss} - Players: {usersOnline} - Rooms: {roomCount}";
        return Task.CompletedTask;
    }
}