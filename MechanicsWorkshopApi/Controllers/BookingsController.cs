using MechanicsWorkshopApi.Data;
using MechanicsWorkshopApi.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MechanicsWorkshopApi.Controllers
{
    [Route("workshop/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly DataContext _context;

        public BookingsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Bookings>>> GetAllBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Car)    // These statements tell the framework to load related
                .Include(b => b.Client) // Car and Client entities when querying Bookings
                .ToListAsync();
            {
                return Ok(bookings);
            }
            
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Entities.Bookings>> GetBooking(int id)
        {
            var booking = await _context.Bookings
              .Include(b => b.Car)
              .Include(b => b.Client)
              .FirstOrDefaultAsync(b => b.ID == id); // FindAsync doesnt support Include,
            {                                        // so I need to use this instead
                if (booking is null)
                {
                    return NotFound("Booking not found!");
                }
                return Ok(booking);
            }

        }

        [HttpPost]
        public async Task<ActionResult<List<Entities.Bookings>>> AddBooking(Entities.Bookings booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return Ok(booking);
        }

        [HttpPut]
        public async Task<ActionResult<List<Entities.Bookings>>> UpdateBooking(Entities.Bookings updatedBooking)
        {
            var dbBooking = await _context.Bookings.FindAsync(updatedBooking.ID);
            {
                if (dbBooking is null)
                {
                    return NotFound("Booking not found!");
                }

                dbBooking.Date = updatedBooking.Date;
                dbBooking.Hour = updatedBooking.Hour;

                await _context.SaveChangesAsync();

                return Ok(updatedBooking);
            }
        }

        [HttpDelete]
        public async Task<ActionResult<List<Entities.Bookings>>> DeleteBooking(int id)
        {
            var dbBooking = await _context.Bookings.FindAsync(id);
            {
                if (dbBooking is null)
                {
                    return NotFound("Booking not found!");
                }

                _context.Bookings.Remove(dbBooking);
                await _context.SaveChangesAsync();

                return NoContent();
            }
        }
    }
}