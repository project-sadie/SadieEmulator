using Sadie.Database.Models.Catalog.Pages;
using Sadie.Shared.Unsorted;

namespace Sadie.Database.Models.Catalog.FrontPage;

public class CatalogFrontPageItemDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Image { get; set; }
    public CatalogFrontPageItemType Type { get; set; }
    public string ProductName { get; set; }
    public CatalogPageDto Page { get; set; }
}