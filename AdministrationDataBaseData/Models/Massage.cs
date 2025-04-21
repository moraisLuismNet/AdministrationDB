using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AdministrationDataBaseData.Models
{
    public class Massage
    {
        [Key]
        public int Id { get; set; }
        public DateTime MassageDate { get; set; }
        public string OtherObservations { get; set; }
        public bool NotificationSent { get; set; }

        // Foreign key for Client
        [ForeignKey(nameof(MassagesCustomer))]
        public int IdMassagesCustomer { get; set; }
        public MassagesCustomer? MassagesCustomer { get; set; } // MassagesCustomer Relationship

    }
}
