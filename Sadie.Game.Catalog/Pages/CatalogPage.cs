using Sadie.Game.Catalog.Items;

namespace Sadie.Game.Catalog.Pages;

public class CatalogPage(
    int id,
    string name,
    string caption,
    string layout,
    int roleId,
    int parentId,
    int orderId,
    int icon,
    bool enabled,
    bool visible,
    string headerImage,
    string teaserImage,
    string specialImage,
    string primaryText,
    string secondaryText,
    string detailsText,
    string teaserText,
    List<CatalogItem> items)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
    public string Caption { get; } = caption;
    public string Layout { get; } = layout;
    public int RoleId { get; } = roleId;
    public int ParentId { get; } = parentId;
    public int OrderId { get; } = orderId;
    public int Icon { get; } = icon;
    public bool Enabled { get; } = enabled;
    public bool Visible { get; } = visible;
    public string HeaderImage { get; } = headerImage;
    public string TeaserImage { get; } = teaserImage;
    public string SpecialImage { get; } = specialImage;
    public string PrimaryText { get; } = primaryText;
    public string SecondaryText { get; } = secondaryText;
    public string DetailsText { get; } = detailsText;
    public string TeaserText { get; } = teaserText;
    public List<CatalogPage> Pages { get; } = new();
    public List<CatalogItem> Items { get; } = items;
}