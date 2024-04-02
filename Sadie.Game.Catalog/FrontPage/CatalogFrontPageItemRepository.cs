namespace Sadie.Game.Catalog.FrontPage;

public class CatalogFrontPageItemRepository(CatalogFrontPageItemDao frontPageItemDao)
{
    public List<CatalogFrontPageItem> Items = [];

    public async Task LoadInitialDataAsync()
    {
        Items = await frontPageItemDao.GetAllAsync();
    }
}