using Microsoft.EntityFrameworkCore;
using ShoppingList.Api.Models;

public class ShoppingListContext : DbContext
{
    public ShoppingListContext(DbContextOptions<ShoppingListContext> options) : base(options) { }

    public DbSet<ListEntity> ShoppingLists { get; set; }
    public DbSet<Item> ShoppingItems { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>()
            .HasOne(i => i.ListEntity)
            .WithMany(l => l.Items)
            .HasForeignKey(i => i.ShoppingListId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}