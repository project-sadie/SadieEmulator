namespace Sadie.Database.Models;

public class Permission
{
    public int Id { get; init; }
    public string Name { get; init; }
    public ICollection<Role> Roles { get; init; } = [];
}