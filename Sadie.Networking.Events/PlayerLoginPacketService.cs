using Sadie.API;
using Sadie.API.Interfaces.Game.Players;
using Sadie.Core.Enums.Game.Players;
using Sadie.Networking.Writers.Handshake;
using Sadie.Networking.Writers.Moderation;
using Sadie.Networking.Writers.Players;
using Sadie.Networking.Writers.Players.Clothing;
using Sadie.Networking.Writers.Players.Effects;
using Sadie.Networking.Writers.Players.Navigator;
using Sadie.Networking.Writers.Players.Other;
using Sadie.Networking.Writers.Players.Permission;
using Sadie.Networking.Writers.Players.Rooms;

namespace Sadie.Networking.Events;

public sealed class PlayerLoginPacketService
{
    public async Task SendAsync(INetworkObject client, IPlayerLogic playerLogic)
    {
        var player = playerLogic.Player;
        var data = player.Data;

        await client.WriteToStreamAsync(new NoobnessLevelWriter
        {
            Level = 1
        });

        if (data.HomeRoomId.HasValue)
        {
            await client.WriteToStreamAsync(new PlayerHomeRoomWriter
            {
                HomeRoom = data.HomeRoomId.Value,
                RoomIdToEnter = data.HomeRoomId.Value
            });
        }

        await client.WriteToStreamAsync(new PlayerEffectListWriter
        {
            Effects = []
        });

        await client.WriteToStreamAsync(new PlayerClothingListWriter
        {
            SetIds = [],
            FurnitureNames = []
        });

        await client.WriteToStreamAsync(new PlayerPermissionsWriter
        {
            Club = player.Subscriptions.Any(x => x.Subscription.Name == "HABBO_CLUB") ? 2 : 0,
            Rank = player.Roles.Count != 0 ? player.Roles.Max(x => x.Id) : 1,
            Ambassador = true
        });

        await client.WriteToStreamAsync(new PlayerNavigatorSettingsWriter
        {
            NavigatorSettings = player.NavigatorSettings!
        });

        await client.WriteToStreamAsync(new PlayerStatusWriter
        {
            IsOpen = true,
            IsShuttingDown = false,
            IsAuthentic = true
        });

        await client.WriteToStreamAsync(new PlayerNotificationSettingsWriter
        {
            ShowNotifications = player.GameSettings.ShowNotifications
        });

        await client.WriteToStreamAsync(new PlayerAchievementScoreWriter
        {
            AchievementScore = data.AchievementScore
        });

        if (playerLogic.HasPermission(PlayerPermissionName.Moderator))
        {
            await client.WriteToStreamAsync(new ModToolsWriter
            {
                Issues = [],
                MessageTemplates = [],
                RoomMessageTemplates = [],
                Unknown3 = 0,
                CallForHelpPermission = true,
                ChatLogsPermission = true,
                AlertPermission = true,
                KickPermission = true,
                BanPermission = true,
                RoomAlertPermission = true,
                RoomKickPermission = true
            });
        }
    }
}
