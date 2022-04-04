using System.Diagnostics;
using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Shared.Extensions;

namespace SadieEmulator.Tasks.Other;

public class UpdateStatusTask : IServerTask
{
    public string Name => "UpdateStatusTask";
    public TimeSpan PeriodicInterval => TimeSpan.FromSeconds(1);
    public DateTime LastExecuted { get; set; }

    private readonly IPlayerRepository _playerRepository;
    private readonly IRoomRepository _roomRepository;

    public UpdateStatusTask(IPlayerRepository playerRepository, IRoomRepository roomRepository)
    {
        _playerRepository = playerRepository;
        _roomRepository = roomRepository;
    }
    
    public Task ExecuteAsync()
    {
        var usersOnline = _playerRepository.Count();
        var roomCount = _roomRepository.Count();
        
        Console.Title = $"Sadie {Server.Version} - Players Online: {usersOnline} - Rooms Loaded: {roomCount}";
        
        return Task.CompletedTask;
    }
}