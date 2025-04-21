using System.ComponentModel.DataAnnotations;

namespace AdministrationDataBaseData.Models
{
    public class MassagesCustomer
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surnames { get; set; }
        public string? DNI { get; set; }
        public string Phone { get; set; }
        public string? AccountNumber { get; set; }
        public string? Address { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Email { get; set; }
        public string? From { get; set; }
        public string? Work { get; set; }
        public string? PhysicalActivity { get; set; }
        public string? Pregnancies { get; set; }
        public string? Operations { get; set; }
        public string? OtherObservations { get; set; }
        public string? ReasonForVisit { get; set; }

        // One-to-many relationship with Massage (one client can have multiple massages)
        public ICollection<Massage> Massages { get; set; } = new List<Massage>();

        // Many-to-many relationship with Pathology
        public ICollection<MassagesCustomerPathology> MassagesCustomerPathologies { get; set; } = new List<MassagesCustomerPathology>();

        // One-to-many relationship with Observation (one customer can have multiple observations)
        public ICollection<Observation> Observations { get; set; } = new List<Observation>();
    }
}
