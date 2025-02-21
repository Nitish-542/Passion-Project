using CRM.Data;
using CRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BartenderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BartenderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/bartender
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BartenderDto>>> GetBartenders()
        {
            var bartenders = await _context.Bartenders
                .Select(b => new BartenderDto
                {
                    BartenderId = b.BartenderId,
                    FirstName = b.FirstName,
                    LastName = b.LastName,
                    DrinksPosted = b.DrinksPosted,
                    Email = b.Email,
                    LastPosted = b.LastPosted
                }).ToListAsync();

            return Ok(bartenders);
        }

        // GET: api/bartender/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<BartenderDto>> GetBartender(int id)
        {
            var bartender = await _context.Bartenders
                .Where(b => b.BartenderId == id)
                .Select(b => new BartenderDto
                {
                    BartenderId = b.BartenderId,
                    FirstName = b.FirstName,
                    LastName = b.LastName,
                    DrinksPosted = b.DrinksPosted,
                    Email = b.Email,
                    LastPosted = b.LastPosted
                }).FirstOrDefaultAsync();

            if (bartender == null)
            {
                return NotFound();
            }

            return Ok(bartender);
        }

        // POST: api/bartender
        [HttpPost]
        public async Task<ActionResult<BartenderDto>> CreateBartender(BartenderDto bartenderDto)
        {
            var bartender = new Bartender
            {
                FirstName = bartenderDto.FirstName,
                LastName = bartenderDto.LastName,
                DrinksPosted = bartenderDto.DrinksPosted,
                Email = bartenderDto.Email,
                LastPosted = bartenderDto.LastPosted
            };

            _context.Bartenders.Add(bartender);
            await _context.SaveChangesAsync();

            bartenderDto.BartenderId = bartender.BartenderId;

            return CreatedAtAction(nameof(GetBartender), new { id = bartenderDto.BartenderId }, bartenderDto);
        }

        // PUT: api/bartender/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBartender(int id, BartenderDto bartenderDto)
        {
            if (id != bartenderDto.BartenderId)
            {
                return BadRequest();
            }

            var bartender = await _context.Bartenders.FindAsync(id);
            if (bartender == null)
            {
                return NotFound();
            }

            bartender.FirstName = bartenderDto.FirstName;
            bartender.LastName = bartenderDto.LastName;
            bartender.DrinksPosted = bartenderDto.DrinksPosted;
            bartender.Email = bartenderDto.Email;
            bartender.LastPosted = bartenderDto.LastPosted;

            _context.Entry(bartender).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/bartender/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBartender(int id)
        {
            var bartender = await _context.Bartenders.FindAsync(id);
            if (bartender == null)
            {
                return NotFound();
            }

            _context.Bartenders.Remove(bartender);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
