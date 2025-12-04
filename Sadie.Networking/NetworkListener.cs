using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.Networking.Options;

namespace Sadie.Networking;

public class NetworkListener(
    ILogger<NetworkListener> logger,
    IOptions<NetworkOptions> options,
    INetworkClientFactory clientFactory,
    INetworkClientConnectionHandler connectionHandler)
    : IHostedService
{
    private readonly NetworkOptions _options = options.Value;
    private WebApplication? _app;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var builder = WebApplication.CreateBuilder();

        builder.WebHost.ConfigureKestrel(k =>
        {
            var ip = IPAddress.Parse(_options.Host ?? "127.0.0.1");

            if (_options.UseWss && !string.IsNullOrWhiteSpace(_options.CertificateFile))
            {
                k.Listen(ip, _options.Port, o => o.UseHttps(_options.CertificateFile));
            }
            else
            {
                k.Listen(ip, _options.Port);
            }
        });

        var app = builder.Build();

        app.UseWebSockets();

        app.Map("/", async ctx =>
        {
            if (!ctx.WebSockets.IsWebSocketRequest)
            {
                ctx.Response.StatusCode = 400;
                return;
            }

            var socket = await ctx.WebSockets.AcceptWebSocketAsync();
            var guid = Guid.NewGuid();
            var ip = ctx.Connection.RemoteIpAddress ?? IPAddress.None;

            var client = clientFactory.CreateClient(ip, guid, socket);

            await connectionHandler.HandleClientAsync(client, ctx.RequestAborted);
        });

        _app = app;

        return app.StartAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _app?.StopAsync(cancellationToken) ?? Task.CompletedTask;
    }
}
