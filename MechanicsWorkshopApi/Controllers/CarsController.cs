using MechanicsWorkshopApi.Data;
using MechanicsWorkshopApi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace MechanicsWorkshopApi.Controllers
{
    [Route("workshop/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase 
    {
        private readonly DataContext _context;
        private readonly ILogger<CarsController> _logger;

        public CarsController(DataContext context) // Constructor
        {
            _context = context;
        }

        

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<Cars>> GetAllCars()
        {
            var cars = await _context.Cars.ToListAsync();
            {
                Log.Information("List of cars returned successfully [200]");
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
                    Log.Information($"Car with id {id} not found! [404]");
                    return NotFound("Car not found!");
                }

                Log.Information($"Car with ID {id} returned successfully [200]");
                return Ok(car);
            }

        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Cars>> AddCar(Cars car)
        {
            if (car == null || car.Brand == null || car.Model == null || car.Year == null || car.Malfunction == null)
            {
                Log.Information($"One or more input data empty while adding new car [400]");
                return BadRequest("Data cannot be empty!"); // 400
            }

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            Log.Information($"New car with ID {car.ID} created successfully [201]");
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
                    Log.Information($"Car with ID {id} not found [404]");
                    return NotFound("Car not found!");
                }

                dbCar.Brand = updatedCar.Brand;
                dbCar.Model = updatedCar.Model;
                dbCar.Year = updatedCar.Year;
                dbCar.Malfunction = updatedCar.Malfunction;

                await _context.SaveChangesAsync();

                Log.Information($"Car with ID {dbCar.ID} updated successfully [200]");
                return Ok(dbCar);
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
                Log.Information($"Car with ID {id} has a booking assigned, can not delete [404]");
                return Conflict("Can not delete car with a booking assigned! Please delete the booking first.");
            }

            var dbCar = await _context.Cars.FindAsync(id);
            if (dbCar is null)
            {
                Log.Information($"Car with ID {id} not found [404]");
                return NotFound("Car not found!");
            }

             _context.Cars.Remove(dbCar);
             await _context.SaveChangesAsync();
            Log.Information($"Car with ID {id} deleted successfully [203]");
            return NoContent();
            
        }
    }
}
