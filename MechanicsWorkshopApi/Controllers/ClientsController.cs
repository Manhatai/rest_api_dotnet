using MechanicsWorkshopApi.Data; // Contains the DataContext class.
using MechanicsWorkshopApi.Entities; // Contains the DB tables.
using Microsoft.AspNetCore.Mvc; // Contains base classes for ASP.NET Core MVC controllers (??? - todo).
using Microsoft.EntityFrameworkCore; // Contains the EFC classes for DB operations.

namespace MechanicsWorkshopApi.Controllers
{
    [Route("workshop/[controller]")] // Sets the base route for the controller. 'Clients' in this case.
    [ApiController] // Marks the class as an API controller.
    public class ClientsController : ControllerBase
    {

        private readonly DataContext _context; // A private variable that holds the 'DataContext' instance.

        public ClientsController(DataContext context) // Accepts the DataContext instance injected by dependancy 
        {                                             // injection system. Used to interact with the DB.
            _context = context;
        }

        [HttpGet] // Indicates the type of request, GET in this case.
        // Async performs potentially long-running operations without blocking the calling thread. Allows for await.
        public async Task<ActionResult<List<Entities.Clients>>> GetAllClients() // Retrives all clients from the DB
        {                                                                       // in the form of a list.
            var clients = await _context.Clients.ToListAsync(); // Used to get all cients from the Clients table 
            {                                                   // by using EFC.
                return Ok(clients); // 200
            }
        }

        [HttpGet("{id}")] // Get request with an ID parameter
        public async Task<ActionResult<Entities.Clients>> GetClient(int id)
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
        public async Task<ActionResult<List<Entities.Clients>>> AddClient(Entities.Clients client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return Created($"/workshop/clients/{client.ID}", client); // 201
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<List<Entities.Clients>>> UpdateClient(int id, Entities.Clients updatedClient)
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
        public async Task<ActionResult<List<Entities.Clients>>> DeleteClient(int id)
        {
            var dbClient = await _context.Clients.FindAsync(id);
            {
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
}
