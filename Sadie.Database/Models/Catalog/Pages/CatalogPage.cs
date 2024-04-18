using System.ComponentModel.DataAnnotations;
using Sadie.Database.Models.Catalog.Items;

namespace Sadie.Database.Models.Catalog.Pages;

public class CatalogPage
{
    [Key]
    public int Id { get; init; }
    public string Name { get; init; }
    public string Caption { get; init; }
    public string Layout { get; init; }
    public int? RoleId { get; init; }
    public int CatalogPageId { get; init; }
    public int OrderId { get; init; }
    public int IconId { get; init; }
    public bool Enabled { get; init; }
    public bool Visible { get; init; }
    public string HeaderImage { get; init; }
    public string TeaserImage { get; init; }
    public string SpecialImage { get; init; }
    public string? PrimaryText { get; init; }
    public string? SecondaryText { get; init; }
    public string? DetailsText { get; init; }
    public string? TeaserText { get; init; }
    public ICollection<CatalogPage> Pages { get; init; } = [];
    public ICollection<CatalogItem> Items { get; init; } = [];
}