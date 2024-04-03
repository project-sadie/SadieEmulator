using Sadie.Game.Catalog.Pages;

namespace Sadie.Game.Catalog.FrontPage;

public class CatalogFrontPageItem(
    int id,
    string title,
    string image,
    CatalogFrontPageItemType type,
    string productName,
    CatalogPage? page)
{
    public int Id { get; } = id;
    public string Title { get; } = title;
    public string Image { get; } = image;
    public CatalogFrontPageItemType Type { get; } = type;
    public string ProductName { get; } = productName;
    public CatalogPage Page { get; } = page;
}