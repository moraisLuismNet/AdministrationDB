using System.ComponentModel.DataAnnotations;

namespace AdministrationDataBaseData.Models
{
    public class Observation
    {
        [Key]
        public int Id { get; set; }
        public string? Description { get; set; }

        // Foreign keys
        public int PathologyId { get; set; }
        public Pathology Pathology { get; set; }

        public int MassagesCustomerId { get; set; }
        public MassagesCustomer MassagesCustomer { get; set; }
    }
}
