using System.Diagnostics;
using System.Net.WebSockets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sadie.API.Interfaces.Game.Rooms;

namespace Sadie.Game
{
    public class GameWorker(
        IRoomRepository roomRepository,
        ILogger<GameWorker> logger)
        : IHostedService
    {
        private CancellationTokenSource? _cts;
        private Thread? _thread;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cts = new CancellationTokenSource();

            _thread = new Thread(() =>
            {
                GameLoopAsync(_cts.Token)
                    .GetAwaiter()
                    .GetResult();
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

                foreach (var room in roomRepository.GetAllRooms())
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

                        foreach (var p in obj.Outbox)
                        {
                            await obj.WebSocket.SendAsync
                            (
                                p.GetAllBytes(),
                                WebSocketMessageType.Binary,
                                true,
                                token
                            );
                        }

                        obj.Outbox.Clear();
                    }
                }

                sw.Stop();
                
                var elapsed = sw.ElapsedMilliseconds;
                var delay = 500 - (int)elapsed;

                switch (delay)
                {
                    case < 0:
                        logger.LogWarning("Game loop tick is lagging by {ms}ms", -delay);
                        break;
                    case > 0:
                        Thread.Sleep(delay);
                        break;
                }
            }
        }
    }
}
