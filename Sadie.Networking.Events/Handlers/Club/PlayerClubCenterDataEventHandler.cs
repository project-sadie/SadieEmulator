using Sadie.Networking.Client;
using Sadie.Shared.Attributes;
using Sadie.Networking.Writers.Players.Other;

namespace Sadie.Networking.Events.Handlers.Club;

[PacketId(EventHandlerId.HabboClubCenter)]
public class PlayerClubCenterDataEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        var subscription = client.Player?.Subscriptions.FirstOrDefault(x => x.Subscription.Name == "HABBO_CLUB");
        
        if (subscription == null)
        {
            return;
        }
        
        await client.WriteToStreamAsync(new PlayerClubCenterDataWriter
        {
            StreakInDays = 0,
            JoinDateString = subscription.CreatedAt.ToString("dd/MM/yyyy"),
            KickbackPercentageString = 0.1.ToString(),
            TotalCreditsMissed = 0,
            TotalCreditsRewarded = 0,
            TotalCreditsSpent = 0,
            CreditRewardForStreakBonus = 0,
            CreditRewardForMonthlySpent = 0,
            TimeUntilPayday = 0
        });
    }
}