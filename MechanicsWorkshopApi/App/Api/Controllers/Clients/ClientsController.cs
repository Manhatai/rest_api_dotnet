using MechanicsWorkshopApi.Infra.Data;
using MechanicsWorkshopApi.Infra.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; // Contains base classes for ASP.NET Core MVC controllers (??? - todo).
using Microsoft.EntityFrameworkCore;
using Serilog; // Contains the EFC classes for DB operations.

namespace MechanicsWorkshopApi.app.api.Controllers
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
                Log.Information("List of clients returned successfully [200]");
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
                    Log.Information($"Client with id {id} not found! [404]");
                    return NotFound("Client not found!");
                }
                Log.Information($"Client with ID {id} returned successfully [200]");
                return Ok(client);
            }

        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Clients>> AddClient(Clients client)
        {
            if (client == null || client.FirstName == null || client.LastName == null || client.Email == null || client.Phone == null)
            {
                Log.Information($"One or more input data empty while adding new client [400]");
                return BadRequest("Data cannot be empty!"); // 400
            }

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            Log.Information($"New car with ID {client.ID} created successfully [201]");
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
                    Log.Information($"Client with ID {id} not found [404]");
                    return NotFound("Client not found!"); // 404
                }

                dbClient.FirstName = updatedClient.FirstName;
                dbClient.LastName = updatedClient.LastName;
                dbClient.Email = updatedClient.Email;
                dbClient.Phone = updatedClient.Phone;

                await _context.SaveChangesAsync();

                Log.Information($"Client with ID {dbClient.ID} updated successfully [200]");
                return Ok(dbClient);
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
                Log.Information($"Client with ID {id} has a booking assigned [404]");
                return Conflict("Can not delete client with a booking assigned! Please delete the booking first.");
            }

            var dbClient = await _context.Clients.FindAsync(id);
            if (dbClient is null)
            {
                Log.Information($"Client with ID {id} not found [404]");
                return NotFound("Client not found!");
            }

            _context.Clients.Remove(dbClient);
            await _context.SaveChangesAsync();

            Log.Information($"Client with ID {id} deleted successfully [203]");
            return NoContent(); // 203
        }
    }
}
