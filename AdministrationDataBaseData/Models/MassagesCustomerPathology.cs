namespace AdministrationDataBaseData.Models
{
    public class MassagesCustomerPathology
    {
        // Foreign key for MassagesCustomer
        public int MassagesCustomerId { get; set; }
        public MassagesCustomer MassagesCustomer{ get; set; }

        // Foreign Key to Pathology
        public int PathologyId { get; set; }
        public Pathology Pathology { get; set; }

    }
}
