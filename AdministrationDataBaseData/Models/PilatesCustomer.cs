using System.ComponentModel.DataAnnotations;

namespace AdministrationDataBaseData.Models
{
    public class PilatesCustomer
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
        public string? OtherObservations { get; set; }
        public bool? IllnessInjuryPathology { get; set; }
        public string? DiseaseInjuryPathologyObservations { get; set; }

        // One-to-many relationship with Session (one pilatesCustomer can have multiple sessions)
        public ICollection<Session> Sessions { get; set; } = new List<Session>();

        // Many-to-Many Relationship PilatesCustomer Objective
        public ICollection<PilatesCustomerObjective> PilatesCustomerObjectives { get; set; } = new List<PilatesCustomerObjective>();
    }

}
