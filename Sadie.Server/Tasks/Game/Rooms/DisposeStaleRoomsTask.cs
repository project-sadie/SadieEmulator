using Microsoft.Extensions.Configuration;
using Sadie.API.Interfaces.Game.Rooms;

namespace SadieEmulator.Tasks.Game.Rooms
{
    public class DisposeStaleRoomsTask(IRoomRepository roomRepository,
        IConfiguration configuration) : IServerTask
    {
        public TimeSpan PeriodicInterval => TimeSpan.FromSeconds(20);
        public DateTime LastExecuted { get; set; }

        public async Task ExecuteAsync()
        {
            var keepAliveMinutes = configuration.GetValue("RoomOptions:KeepAliveMinutes", 5);
            
            var staleRooms = roomRepository
                .GetAllRooms()
                .Where(x => 
                    x.UserRepository.NoUsersSince != null && 
                    (DateTime.Now - x.UserRepository.NoUsersSince.Value).TotalMinutes >= keepAliveMinutes)
                .OrderBy(x => x.UserRepository.NoUsersSince)
                .Take(100)
                .ToList();
            
            foreach (var room in staleRooms)
            {
                roomRepository.TryRemove(room.Room.Id, out _);
                await room.DisposeAsync();
            }
        }
    }
}