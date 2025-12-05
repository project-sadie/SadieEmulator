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
                var roomTasks = new List<Task>();
                var semaphore = new SemaphoreSlim(Environment.ProcessorCount / 2);
        
                foreach (var room in roomRepository.GetAllRooms())
                {
                    await semaphore.WaitAsync();

                    var roomTask = Task.Run(async () =>
                    {
                        try
                        {
                            await room.BotRepository.RunPeriodicCheckAsync();
                            await room.UserRepository.RunPeriodicCheckAsync();
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    });

                    roomTasks.Add(roomTask);
                }

                await Task.WhenAll(roomTasks);
            }
            finally
            {
                _isRunning = 0;
            }
        }
    }
}