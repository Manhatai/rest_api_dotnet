using MechanicsWorkshopApi.Data;
using MechanicsWorkshopApi.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MechanicsWorkshopApi.Controllers
{
    [Route("workshop/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {

        private readonly DataContext _context;

        public ClientsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Clients>>> GetAllClients()
        {
            var clients = await _context.Clients.ToListAsync();
            {
                return Ok(clients);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Clients>> GetClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            {
                if (client is null)
                {
                    return NotFound("Client not found!");
                }
                return Ok(client);
            }

        }

        [HttpPost]
        public async Task<ActionResult<List<Clients>>> AddClient(Clients client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return Ok(client);
        }

        [HttpPut]
        public async Task<ActionResult<List<Clients>>> UpdateClient(Clients updatedClient)
        {
            var dbClient = await _context.Clients.FindAsync(updatedClient.ID);
            {
                if (dbClient is null)
                {
                    return NotFound("Client not found!");
                }

                dbClient.FirstName = updatedClient.FirstName;
                dbClient.LastName = updatedClient.LastName;
                dbClient.Email = updatedClient.Email;
                dbClient.Phone = updatedClient.Phone;

                await _context.SaveChangesAsync();

                return Ok(updatedClient);
            }
        }

        [HttpDelete]
        public async Task<ActionResult<List<Clients>>> DeleteClient(int id)
        {
            var dbClient = await _context.Clients.FindAsync(id);
            {
                if (dbClient is null)
                {
                    return NotFound("Client not found!");
                }

                _context.Clients.Remove(dbClient);
                await _context.SaveChangesAsync();

                return NoContent();
            }
        }
    }
}
