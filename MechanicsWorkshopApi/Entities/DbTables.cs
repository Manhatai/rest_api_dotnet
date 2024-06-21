using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MechanicsWorkshopApi.Entities
{
    public class Clients
    {
        [Key]
        public int ID { get; set; }

        [Required, MaxLength(20)]
        public string FirstName { get; set; } = string.Empty; // 'public' means its accessible from outside the class
                                                              // 'get' is called when this accessor is used to return a value from the property
        [Required, MaxLength(20)]                             // 'set' is used when assigning a value to the property                  
        public string LastName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        [EmailAddress] // Ensures the proper email address format
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

        [Required, MaxLength(100)] // Changed from 50 MaxLength
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

        [ForeignKey("Client")] // Specifies the relationship
        public int ClientID { get; set; } // Integer representing the foreign key to Class entity
        public Clients? Client { get; set; } // Navigation property to Clients entity

        [ForeignKey("Car")]
        public int CarID { get; set; }
        public Cars? Car { get; set; }
    }

    public class User
    {
        [Key]
        public int ID { get; set; }

        [Required, MaxLength(30)]
        public string Login { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string PasswordHash { get; set; } = string.Empty;
    }

    public class UserRequest
    {

        [Required, MaxLength(30)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
