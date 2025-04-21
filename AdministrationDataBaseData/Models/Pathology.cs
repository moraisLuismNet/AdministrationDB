using System.ComponentModel.DataAnnotations;

namespace AdministrationDataBaseData.Models
{
    public class Pathology
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        // Many-to-many relationship with MassagesCustomer
        public ICollection<MassagesCustomerPathology> MassagesCustomerPathologies { get; set; } = new List<MassagesCustomerPathology>();

        // One-to-many relationship with Observation
        public ICollection<Observation> Observations { get; set; } = new List<Observation>();

    }
}
