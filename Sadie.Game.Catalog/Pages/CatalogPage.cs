namespace Sadie.Game.Catalog;

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
        Pages = pages;
        Items = items;
    }
}