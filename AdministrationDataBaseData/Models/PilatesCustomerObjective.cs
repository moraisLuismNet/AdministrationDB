namespace AdministrationDataBaseData.Models
{
    public class PilatesCustomerObjective
    {
        public int PilatesCustomerId { get; set; }
        public PilatesCustomer PilatesCustomer { get; set; }

        public int ObjectiveId { get; set; }
        public Objective Objective { get; set; }
    }

}
