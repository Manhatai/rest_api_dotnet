using MechanicsWorkshopApi.Data;
using MechanicsWorkshopApi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MechanicsWorkshopApi.Controllers
{
    [Route("workshop/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private readonly DataContext _context;

        public CarsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<Cars>> GetAllCars()
        {
            var cars = await _context.Cars.ToListAsync();
            {
                return Ok(cars);
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Cars>> GetCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            {
                if (car is null)
                {
                    return NotFound("Car not found!");
                }
                return Ok(car);
            }

        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Cars>> AddCar(Cars car)
        {
            if (car == null || car.Brand == null || car.Model == null || car.Year == null || car.Malfunction == null)
            {
                return BadRequest("Data cannot be empty!"); // 400
            }

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            return Created($"/workshop/clients/{car.ID}", car);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<Cars>> UpdateCar(int id, Cars updatedCar)
        {
            var dbCar = await _context.Cars.FindAsync(id);
            {
                if (dbCar is null)
                {
                    return NotFound("Car not found!");
                }

                dbCar.Brand = updatedCar.Brand;
                dbCar.Model = updatedCar.Model;
                dbCar.Year = updatedCar.Year;
                dbCar.Malfunction = updatedCar.Malfunction;

                await _context.SaveChangesAsync();

                return Ok(updatedCar);
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteCar(int id)
        {
            // See if there is a booking that contains a client with this id
            var dbBooking = await _context.Bookings.FirstOrDefaultAsync(b => b.CarID == id); // LINQ lambda expression
            if (dbBooking != null)
            {
                return Conflict("Cannot delete car with a booking assigned! Please delete the booking first.");
            }

            var dbCar = await _context.Cars.FindAsync(id);
            if (dbCar is null)
            {
                return NotFound("Car not found!");
            }

             _context.Cars.Remove(dbCar);
             await _context.SaveChangesAsync();
             return NoContent();
            
        }
    }
}
