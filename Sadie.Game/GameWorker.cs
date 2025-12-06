using System.Diagnostics;
using System.Net.WebSockets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sadie.API.Interfaces.Game.Rooms;

namespace Sadie.Game
{
    public class GameWorker(IRoomRepository roomRepository, ILogger<GameWorker> logger) : IHostedService
    {
        private CancellationTokenSource? _cts;
        private Thread? _thread;
        private readonly SemaphoreSlim _semaphore = new(Environment.ProcessorCount);

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cts = new CancellationTokenSource();
            _thread = new Thread(() =>
            {
                GameLoopAsync(_cts.Token).GetAwaiter().GetResult();
            })
            {
                IsBackground = true
            };
            
            _thread.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cts?.Cancel();
            return Task.CompletedTask;
        }

        private async Task GameLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var sw = Stopwatch.StartNew();
                var roomTasks = new List<Task>();

                foreach (var room in roomRepository.GetAllRooms())
                {
                    await _semaphore.WaitAsync(token);

                    var roomTask = Task.Run(async () =>
                    {
                        try
                        {
                            await room.BotRepository.RunPeriodicCheckAsync();
                            await room.UserRepository.RunPeriodicCheckAsync();

                            foreach (var user in room.UserRepository.GetAll())
                            {
                                var obj = user.NetworkObject;

                                if (obj.Outbox.Count == 0)
                                {
                                    continue;
                                }

                                await obj.WebSocket.SendAsync
                                (
                                    obj
                                        .Outbox
                                        .SelectMany(x => x.GetAllBytes())
                                        .ToArray(),
                                    WebSocketMessageType.Binary,
                                    true,
                                    token
                                );

                                obj.Outbox.Clear();
                            }
                        }
                        finally
                        {
                            _semaphore.Release();
                        }
                    }, token);

                    roomTasks.Add(roomTask);
                }

                await Task.WhenAll(roomTasks);
                sw.Stop();

                var elapsed = sw.ElapsedMilliseconds;
                var delay = 500 - (int)elapsed;

                if (delay < 0)
                {
                    logger.LogWarning("Game loop tick is lagging by {ms}ms", -delay);
                }
                else if (delay > 0)
                {
                    Thread.Sleep(delay);
                }
            }
        }
    }
}
