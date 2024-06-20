using MechanicsWorkshopApi.Data;
using MechanicsWorkshopApi.Entities;
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
        public async Task<ActionResult<List<Cars>>> GetAllCars()
        {
            var cars = await _context.Cars.ToListAsync();
            {
                return Ok(cars);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Entities.Cars>> GetCar(int id)
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
        public async Task<ActionResult<List<Entities.Cars>>> AddCar(Entities.Cars car)
        {
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            return Created($"/workshop/clients/{car.ID}", car);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<List<Entities.Cars>>> UpdateCar(int id, Entities.Cars updatedCar)
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
        public async Task<ActionResult<List<Entities.Cars>>> DeleteCar(int id)
        {
            var dbCar = await _context.Cars.FindAsync(id);
            {
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
}
