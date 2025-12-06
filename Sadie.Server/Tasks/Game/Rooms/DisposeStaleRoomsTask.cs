using Microsoft.Extensions.Configuration;
using Sadie.API.Interfaces.Game.Rooms;

namespace SadieEmulator.Tasks.Game.Rooms
{
    public class DisposeStaleRoomsTask(IRoomRepository roomRepository,
        IConfiguration configuration) : IServerTask
    {
        public TimeSpan PeriodicInterval => TimeSpan.FromSeconds(10);
        public DateTime LastExecuted { get; set; }

        private readonly SemaphoreSlim _semaphore = new(5);

        public async Task ExecuteAsync()
        {
            var keepAlive = configuration.GetValue("RoomOptions:KeepAliveSeconds", 60);

            var staleRooms = roomRepository
                .GetAllRooms()
                .Where(x =>
                    x.UserRepository.NoUsersSince != null &&
                    (DateTime.UtcNow - x.UserRepository.NoUsersSince.Value).TotalSeconds >= keepAlive)
                .OrderBy(x => x.UserRepository.NoUsersSince)
                .Take(100)
                .ToList();

            var tasks = new List<Task>();

            foreach (var room in staleRooms)
            {
                await _semaphore.WaitAsync();

                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        roomRepository.TryRemove(room.Room.Id, out _);
                        await room.DisposeAsync();
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                }));
            }

            await Task.WhenAll(tasks);
        }
    }
}