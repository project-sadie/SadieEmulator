using Sadie.Game.Players.DaosToDrop;

namespace Sadie.Game.Players.Club;

public class PlayerClubOfferRepository(PlayerClubOfferDao clubOfferDao)
{
    private List<PlayerClubOffer> _offers { get; set; } = [];

    public async Task LoadInitialDataAsync()
    {
        _offers = await clubOfferDao.GetAllAsync();
    }

    public IReadOnlyCollection<PlayerClubOffer> Offers => _offers;
}