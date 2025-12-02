using Sadie.API.Interfaces.Game.Rooms;

namespace SadieEmulator.Tasks.Game.Rooms
{
    public class ProcessRoomUnitsTask(IRoomRepository roomRepository) : IServerTask
    {
        public TimeSpan PeriodicInterval => TimeSpan.FromMilliseconds(500);
        public DateTime LastExecuted { get; set; }

        private int _isRunning;

        public async Task ExecuteAsync()
        {
            if (Interlocked.Exchange(ref _isRunning, 1) == 1)
            {
                return;
            }

            try
            {
                foreach (var room in roomRepository.GetAllRooms())
                {
                    await room.BotRepository.RunPeriodicCheckAsync();
                    await room.UserRepository.RunPeriodicCheckAsync();
                }
            }
            finally
            {
                _isRunning = 0;
            }
        }
    }
}