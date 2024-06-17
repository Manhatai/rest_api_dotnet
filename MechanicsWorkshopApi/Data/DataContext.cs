using MechanicsWorkshopApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace MechanicsWorkshopApi.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Clients> Clients { get; set; }
        public DbSet<Cars> Cars { get; set; }
    }
}
