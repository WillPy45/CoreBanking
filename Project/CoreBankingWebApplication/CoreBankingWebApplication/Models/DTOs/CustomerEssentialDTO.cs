using CoreBankingWebApplication.Models.Entities;


namespace CoreBankingWebApplication.Models.DTOs
{
    public class CustomerEssentialDTO
    {
        public int CustomerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }

        public static CustomerEssentialDTO FromEntity(Customer customer)
        {
            return new CustomerEssentialDTO()
            {
                CustomerId = customer.CustomerId,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Address = customer.Address
            };
        }
    }
    
}
