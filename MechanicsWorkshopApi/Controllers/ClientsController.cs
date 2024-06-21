using MechanicsWorkshopApi.Data; // Contains the DataContext class.
using MechanicsWorkshopApi.Entities; // Contains the DB tables.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; // Contains base classes for ASP.NET Core MVC controllers (??? - todo).
using Microsoft.EntityFrameworkCore; // Contains the EFC classes for DB operations.

namespace MechanicsWorkshopApi.Controllers
{
    [Route("workshop/[controller]")] // Sets the base route for the controller. 'Clients' in this case.
    [ApiController] // Marks the class as an API controller.
    public class ClientsController : ControllerBase
    {

        private readonly DataContext _context; // A private variable that holds the 'DataContext' instance.

        public ClientsController(DataContext context) // Accepts the DataContext instance injected by dependancy injection system.
        {
            _context = context; // Used to interact with the DB.
        }

        [HttpGet] // Indicates the type of request, GET in this case.
        [Authorize]
        public async Task<ActionResult<Clients>> GetAllClients()
        {
            var clients = await _context.Clients.ToListAsync(); // Used to get all cients from the Clients table by using EFC.
            {
                return Ok(clients); // 200
            }
        }

        [HttpGet("{id}")] // Get request with an ID parameter
        [Authorize]
        public async Task<ActionResult<Clients>> GetClient(int id)
        {
            var client = await _context.Clients.FindAsync(id); // Finds client by id
            {
                if (client is null)
                {
                    return NotFound("Client not found!");
                } 
                return Ok(client);
            }

        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Clients>> AddClient(Clients client)
        {
            if (client == null || client.FirstName == null || client.LastName == null || client.Email == null || client.Phone == null)
            {
                return BadRequest("Data cannot be empty!"); // 400
            }

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return Created($"/workshop/clients/{client.ID}", client); // 201
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<Clients>> UpdateClient(int id, Clients updatedClient)
        {
            var dbClient = await _context.Clients.FindAsync(id);
            {
                if (dbClient is null)
                {
                    return NotFound("Client not found!"); // 404
                }

                dbClient.FirstName = updatedClient.FirstName;
                dbClient.LastName = updatedClient.LastName;
                dbClient.Email = updatedClient.Email;
                dbClient.Phone = updatedClient.Phone;

                await _context.SaveChangesAsync();

                return Ok(updatedClient);
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteClient(int id)
        {
            // See if there is a booking that contains a client with this id
            var dbBooking = await _context.Bookings.FirstOrDefaultAsync(b => b.ClientID == id); // LINQ lambda expression
            if (dbBooking != null)
            {
                return Conflict("Cannot delete client with a booking assigned! Please delete the booking first.");
            }

            var dbClient = await _context.Clients.FindAsync(id);     
            if (dbClient is null)
            {
                return NotFound("Client not found!");
            }

            _context.Clients.Remove(dbClient);
            await _context.SaveChangesAsync();

            return NoContent(); // 204
        }
    }
}
