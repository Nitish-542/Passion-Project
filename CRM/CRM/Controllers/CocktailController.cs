using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CRM.Models;
using System.Linq;
using System.Threading.Tasks;
using CRM.Data;

namespace CRM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CocktailController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CocktailController(ApplicationDbContext context)
        {
            _context = context;
        }

        // CREATE: api/cocktail
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<CocktailDTO>> CreateCocktail(CocktailDTO cocktailDto)
        {
            var cocktail = new Cocktail
            {
                BartenderId = cocktailDto.BartenderId,
                DrinkName = cocktailDto.DrinkName,
                DrinkRecipe = cocktailDto.DrinkRecipe,
                LiqIns = cocktailDto.LiqIns,
                MixIns = cocktailDto.MixIns,
                CategoryId = cocktailDto.CategoryId,
                DatePosted = cocktailDto.DatePosted
            };

            _context.Cocktails.Add(cocktail);
            await _context.SaveChangesAsync();

            // Manually map the model to DTO with CategoryName
            var createdCocktailDto = new CocktailDTO
            {
                DrinkId = cocktail.DrinkId,
                BartenderId = cocktail.BartenderId,
                DrinkName = cocktail.DrinkName,
                DrinkRecipe = cocktail.DrinkRecipe,
                LiqIns = cocktail.LiqIns,
                MixIns = cocktail.MixIns,
                CategoryName = cocktail.Category?.CategoryName,  // Mapping CategoryName from the related Category
                DatePosted = cocktail.DatePosted
            };

            return CreatedAtAction("GetCocktail", new { id = cocktail.DrinkId }, createdCocktailDto);
        }

        // READ: api/cocktail/{id}
        [HttpGet("{id}")]
        [Route("get/{id}")]
        public async Task<ActionResult<CocktailDTO>> GetCocktail(int id)
        {
            var cocktail = await _context.Cocktails
                .Include(c => c.Category)  // Include Category for mapping CategoryName
                .Include(c => c.Bartender)
                .FirstOrDefaultAsync(c => c.DrinkId == id);

            if (cocktail == null)
            {
                return NotFound();
            }

            // Manually map the model to DTO with CategoryName
            var cocktailDto = new CocktailDTO
            {
                DrinkId = cocktail.DrinkId,
                BartenderId = cocktail.BartenderId,
                DrinkName = cocktail.DrinkName,
                DrinkRecipe = cocktail.DrinkRecipe,
                LiqIns = cocktail.LiqIns,
                MixIns = cocktail.MixIns,
                CategoryName = cocktail.Category?.CategoryName,  // Mapping CategoryName from the related Category
                DatePosted = cocktail.DatePosted
            };

            return cocktailDto;
        }

        // READ: api/cocktail
        [HttpGet]
        [Route("list")]
        public async Task<ActionResult<IEnumerable<CocktailDTO>>> GetCocktails()
        {
            var cocktails = await _context.Cocktails
                .Include(c => c.Category)  // Include Category for mapping CategoryName
                .Include(c => c.Bartender)
                .ToListAsync();

            // Manually map the models to DTOs with CategoryName
            var cocktailDtos = cocktails.Select(c => new CocktailDTO
            {
                DrinkId = c.DrinkId,
                BartenderId = c.BartenderId,
                DrinkName = c.DrinkName,
                DrinkRecipe = c.DrinkRecipe,
                LiqIns = c.LiqIns,
                MixIns = c.MixIns,
                CategoryName = c.Category?.CategoryName,  // Mapping CategoryName from the related Category
                DatePosted = c.DatePosted
            }).ToList();

            return cocktailDtos;
        }

        // UPDATE: api/cocktail/{id}
        [HttpPut("{id}")]
        [Route("update/{id}")]
        public async Task<IActionResult> UpdateCocktail(int id, CocktailDTO cocktailDto)
        {
            if (id != cocktailDto.DrinkId)
            {
                return BadRequest();
            }

            var cocktail = await _context.Cocktails.FindAsync(id);
            if (cocktail == null)
            {
                return NotFound();
            }

            // Manually map the DTO to the model
            cocktail.BartenderId = cocktailDto.BartenderId;
            cocktail.DrinkName = cocktailDto.DrinkName;
            cocktail.DrinkRecipe = cocktailDto.DrinkRecipe;
            cocktail.LiqIns = cocktailDto.LiqIns;
            cocktail.MixIns = cocktailDto.MixIns;
            cocktail.CategoryId = cocktailDto.CategoryId;
            cocktail.DatePosted = cocktailDto.DatePosted;

            _context.Entry(cocktail).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/cocktail/{id}
        [HttpDelete("{id}")]
        [Route("delete/{id}")]
        public async Task<IActionResult> DeleteCocktail(int id)
        {
            var cocktail = await _context.Cocktails.FindAsync(id);
            if (cocktail == null)
            {
                return NotFound();
            }

            _context.Cocktails.Remove(cocktail);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
