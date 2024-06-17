using System.ComponentModel.DataAnnotations;

namespace MechanicsWorkshopApi.Entities
{
    public class Clients
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required, MaxLength(12)]
        public string Phone { get; set; } = string.Empty;
    }
}
