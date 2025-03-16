using ShoppingList.Api.Models;

public class ListEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; }

    public List<Item> Items { get; set; } = new();
}
