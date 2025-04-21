using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AdministrationDataBaseData.Models
{
    public class Session
    {
        [Key]
        public int Id { get; set; }
        public DateTime SessionDate { get; set; }
        public string? SessionObservations { get; set; }
        public bool NotificationSent { get; set; }

        // Foreign key for PilatesCustomer
        [ForeignKey(nameof(PilatesCustomer))]
        public int PilatesCustomerId { get; set; }
        public PilatesCustomer? PilatesCustomer { get; set; } // PilatesCustomer Relationship
    }
}
