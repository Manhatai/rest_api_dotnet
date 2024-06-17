using MechanicsWorkshopApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace MechanicsWorkshopApi.Data
{
    public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
    {
        public DbSet<Clients> Clients { get; set; }
    }
}