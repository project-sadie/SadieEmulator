using System.ComponentModel.DataAnnotations;
using Sadie.Database.Models.Catalog.Items;

namespace Sadie.Database.Models.Catalog.Pages;

public class CatalogPage
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Caption { get; set; }
    public string Layout { get; set; }
    public int? RoleId { get; set; }
    public int CatalogPageId { get; set; }
    public int OrderId { get; set; }
    public int IconId { get; set; }
    public bool Enabled { get; set; }
    public bool Visible { get; set; }
    public string HeaderImage { get; set; }
    public string TeaserImage { get; set; }
    public string SpecialImage { get; set; }
    public string? PrimaryText { get; set; }
    public string? SecondaryText { get; set; }
    public string? DetailsText { get; set; }
    public string? TeaserText { get; set; }
    public ICollection<CatalogPage> Pages { get; set; } = [];
    public ICollection<CatalogItem> Items { get; set; } = [];
}