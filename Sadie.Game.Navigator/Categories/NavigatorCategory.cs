namespace Sadie.Game.Navigator.Categories;

public class NavigatorCategory
{
    public int Id { get; }
    public string Name { get; }
    public string CodeName { get; }
    public int OrderId { get; }
    
    public NavigatorCategory(int id,
        string name,
        string codeName,
        int orderId)
    {
        Id = id;
        Name = name;
        CodeName = codeName;
        OrderId = orderId;
    }
}