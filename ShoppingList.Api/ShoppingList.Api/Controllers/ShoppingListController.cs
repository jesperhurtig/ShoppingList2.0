using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;  

namespace ShoppingList.Api.Controllers
{
    [Route("api/lists")]
    [ApiController]
    public class ShoppingListsController : ControllerBase
    {
        private readonly ShoppingListContext _context;

        public ShoppingListsController(ShoppingListContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ListEntity>>> GetLists()
        {
            return await _context.ShoppingLists.Include(l => l.Items).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<ListEntity>> CreateList([FromBody] ListEntity list)
        {
            if (string.IsNullOrWhiteSpace(list.Name))
            {
                return BadRequest("Listan måste ha ett namn.");
            }

            var newList = new ListEntity
            {
                Id = Guid.NewGuid(),
                Name = list.Name,
                Date = DateTime.UtcNow
            };

            _context.ShoppingLists.Add(newList);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetListById), new { id = newList.Id }, newList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ListEntity>> GetListById(Guid id)
        {
            var list = await _context.ShoppingLists.Include(l => l.Items).FirstOrDefaultAsync(l => l.Id == id);

            if (list == null)
            {
                return NotFound();
            }

            return Ok(list);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteList(Guid id)
        {
            var list = await _context.ShoppingLists.FindAsync(id);
            if (list == null) return NotFound();

            _context.ShoppingLists.Remove(list);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}