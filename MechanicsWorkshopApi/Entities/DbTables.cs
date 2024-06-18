using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MechanicsWorkshopApi.Entities
{
    public class Clients
    {
        [Key]
        public int ID { get; set; }

        [Required, MaxLength(20)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string LastName { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(12)]
        public string Phone { get; set; } = string.Empty;
    }

    public class Cars
    {
        [Key]
        public int ID { get; set; }

        [Required, MaxLength(20)]
        public string Brand { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Model { get; set; } = string.Empty;

        [Required, MaxLength(4)]
        public string Year { get; set; } = string.Empty;

        [Required, MaxLength(100)] // Changed from 50 maxl
        public string Malfunction { get; set; } = string.Empty;
    }

    public class Bookings
    {
        [Key]
        public int ID { get; set; }

        [Required, MaxLength(20)]
        public string Date { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Hour { get; set; } = string.Empty;

        [ForeignKey("Client")]
        public int ClientID { get; set; }
        public Clients Client { get; set; }

        [ForeignKey("Car")]
        public int CarID { get; set; }
        public Cars Car { get; set; }
    }
}
