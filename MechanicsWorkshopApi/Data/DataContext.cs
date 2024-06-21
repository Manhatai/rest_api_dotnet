// So called Entity Framework Core which is a ORM (allows interacting with DB using .NET objects)
using MechanicsWorkshopApi.Entities; // Contains definitions for Clients, Cars and Bookings
using Microsoft.EntityFrameworkCore; // EFC import

namespace MechanicsWorkshopApi.Data // Namespaces = a way to organize and group related classes
{
    public class DataContext : DbContext // DataContext inherits from DbContext which is the primary way to interact with DB,
    {                                    // so it becomes a context class allowing for data querying
        public DataContext(DbContextOptions<DataContext> options) : base(options) { } // A constructor that contains configuration options for the class.
                                                                                      // 'Base(options)' call passes these options to classes constructor.
        public DbSet<Clients> Clients { get; set; } // Collections representing X in each table
        public DbSet<Cars> Cars { get; set; }
        public DbSet<Bookings> Bookings { get; set; }

        public DbSet<User> User { get; set; }

    }
}