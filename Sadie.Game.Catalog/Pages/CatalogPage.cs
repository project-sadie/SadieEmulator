using Sadie.Game.Catalog.Items;

namespace Sadie.Game.Catalog.Pages;

public class CatalogPage
{
    public int Id { get; }
    public string Name { get; }
    public string Caption { get; }
    public string Layout { get; }
    public int RoleId { get; }
    public int ParentId { get; }
    public int OrderId { get; }
    public int Icon { get; }
    public bool Enabled { get; }
    public bool Visible { get; }
    public string HeaderImage { get; }
    public string TeaserImage { get; }
    public string SpecialImage { get; }
    public string PrimaryText { get; }
    public string SecondaryText { get; }
    public string DetailsText { get; }
    public string TeaserText { get; }
    public List<CatalogPage> Pages { get; }
    public List<CatalogItem> Items { get; }

    public CatalogPage(
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
        List<CatalogPage> pages, 
        List<CatalogItem> items)
    {
        Id = id;
        Name = name;
        Caption = caption;
        Layout = layout;
        RoleId = roleId;
        ParentId = parentId;
        OrderId = orderId;
        Icon = icon;
        Enabled = enabled;
        Visible = visible;
        HeaderImage = headerImage;
        TeaserImage = teaserImage;
        SpecialImage = specialImage;
        PrimaryText = primaryText;
        SecondaryText = secondaryText;
        DetailsText = detailsText;
        TeaserText = teaserText;
        Pages = pages;
        Items = items;
    }
}