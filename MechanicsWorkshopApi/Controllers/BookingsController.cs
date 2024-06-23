using MechanicsWorkshopApi.Data;
using MechanicsWorkshopApi.Entities;
using MechanicsWorkshopApi.Migrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

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
        [Authorize]
        public async Task<ActionResult<Bookings>> GetAllBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Car)    // These statements tell the framework to load related Car and Client entities when querying Bookings
                .Include(b => b.Client)
                .ToListAsync();
            {
                Log.Information("List of bookings returned successfully [200]");
                return Ok(bookings);
            }
            
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Bookings>> GetBooking(int id)
        {
            var booking = await _context.Bookings
              .Include(b => b.Car)
              .Include(b => b.Client)
              .FirstOrDefaultAsync(b => b.ID == id); // FindAsync doesnt support Include, so I need to use this instead
            {
                if (booking is null)
                {
                    Log.Information($"Booking with id {id} not found! [404]");
                    return NotFound("Booking not found!");
                }
                Log.Information($"Booking with ID {id} returned successfully [200]");
                return Ok(booking);
            }

        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Bookings>> AddBooking(Bookings booking)
        {
            booking.Client = await _context.Clients.FindAsync(booking.ClientID);
            booking.Car = await _context.Cars.FindAsync(booking.CarID);

            if (booking.Client == null || booking.Car == null)
            {
                Log.Information("Client or car doesn't exist. Please verify the provided IDs and ensure they are correct. [400]");
                return BadRequest("Client or car doesnt exist!");
            }

            if (booking.Date == null || booking.Hour == null)
            {
                Log.Information($"One or more input data empty while adding new booking [400]");
                return BadRequest("Data cannot be empty!"); // 400
            }

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            Log.Information($"New booking with ID {booking.ID} created successfully [201]");
            return Created($"/workshop/clients/{booking.ID}", booking);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<Bookings>> UpdateBooking(int id, Bookings updatedBooking)
        {
            var dbBooking = await _context.Bookings.FindAsync(id);
            {
                if (dbBooking is null)
                {
                    Log.Information($"Booking with ID {id} not found [404]");
                    return NotFound("Booking not found!");
                }

                dbBooking.Date = updatedBooking.Date;
                dbBooking.Hour = updatedBooking.Hour;
                dbBooking.ClientID = updatedBooking.ClientID;
                dbBooking.CarID = updatedBooking.CarID;

                await _context.SaveChangesAsync();

                Log.Information($"Booking with ID {dbBooking.ID} updated successfully [200]");
                return Ok(dbBooking);
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteBooking(int id)
        {
            var dbBooking = await _context.Bookings.FindAsync(id);
            {
                if (dbBooking is null)
                {
                    Log.Information($"Booking with ID {id} not found [404]");
                    return NotFound("Booking not found!");
                }

                _context.Bookings.Remove(dbBooking);
                await _context.SaveChangesAsync();

                Log.Information($"Booking with ID {id} deleted successfully [203]");
                return NoContent(); // 203
            }
        }
    }
}