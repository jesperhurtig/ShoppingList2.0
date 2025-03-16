using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingList.Api.Models;

namespace ShoppingList.Api.Controllers
{
    [Route("api/items")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly ShoppingListContext _context;

        public ItemsController(ShoppingListContext context)
        {
            _context = context;
        }
        
        [HttpGet("{listId}")]
        public async Task<ActionResult<IEnumerable<Item>>> GetItemsByList(Guid listId)
        {
            return await _context.ShoppingItems
                .Where(i => i.ShoppingListId == listId)
                .ToListAsync();
        }

        [HttpPost("{listId}")]
        public async Task<ActionResult<Item>> AddItemToList(Guid listId, Item item)
        {
            item.ShoppingListId = listId;
            _context.ShoppingItems.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetItemsByList), new { listId = item.ShoppingListId }, item);
        }
        
        [HttpPut("{id}/toggle-picked")]
        public async Task<IActionResult> TogglePickedStatus(int id)
        {
            var item = await _context.ShoppingItems.FindAsync(id);
            if (item == null) return NotFound("Item hittades inte.");

            item.IsPicked = !item.IsPicked;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}/purchase")]
        public async Task<IActionResult> MarkItemAsPurchased(int id)
        {
            var item = await _context.ShoppingItems.FindAsync(id);
            if (item == null) return NotFound("Item hittades inte.");

            if (!item.IsPicked)
                return BadRequest("Item måste vara plockat innan det kan markeras som köpt.");

            item.IsPurchased = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _context.ShoppingItems.FindAsync(id);
            if (item == null) return NotFound("Item hittades inte.");

            if (!item.IsPicked)
                return BadRequest("Endast plockade items kan tas bort.");

            _context.ShoppingItems.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        
        [HttpDelete("batch")]
        public async Task<IActionResult> DeleteMultipleItems([FromBody] List<int> itemIds)
        {
            if (itemIds == null || !itemIds.Any())
                return BadRequest("Inga item-ID:n angivna.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var itemsToDelete = await _context.ShoppingItems
                    .Where(i => itemIds.Contains(i.Id) && i.IsPicked)
                    .ToListAsync();

                if (!itemsToDelete.Any())
                    return NotFound("Inga matchande plockade items hittades.");

                _context.ShoppingItems.RemoveRange(itemsToDelete);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return NoContent();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Ett fel inträffade vid borttagning av items.");
            }
        }
        
        [HttpDelete("lists/{listId}/purchased")]
        public async Task<IActionResult> DeletePurchasedItems(Guid listId)
        {
            var purchasedItems = await _context.ShoppingItems
                .Where(i => i.ShoppingListId == listId && i.IsPicked && i.IsPurchased)
                .ToListAsync();

            if (!purchasedItems.Any()) 
                return NotFound("Inga plockade och köpta items hittades.");

            _context.ShoppingItems.RemoveRange(purchasedItems);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
