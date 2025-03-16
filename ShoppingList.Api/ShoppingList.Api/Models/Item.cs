namespace ShoppingList.Api.Models;

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsPicked { get; set; } = false;
    public bool IsPurchased { get; set; } = false;

    public Guid ShoppingListId { get; set; } 
    public ListEntity ListEntity { get; set; } 
}