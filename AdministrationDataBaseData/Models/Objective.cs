using System.ComponentModel.DataAnnotations;

namespace AdministrationDataBaseData.Models
{
    public class Objective
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        // Many-to-many relationship
        public ICollection<PilatesCustomerObjective> PilatesCustomerObjectives { get; set; } = new List<PilatesCustomerObjective>();

    }

}
