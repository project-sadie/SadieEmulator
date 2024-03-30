namespace Sadie.Game.Navigator.Categories;

public class NavigatorCategory(
    int id,
    string name,
    string codeName,
    int orderId)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
    public string CodeName { get; } = codeName;
    public int OrderId { get; } = orderId;
}