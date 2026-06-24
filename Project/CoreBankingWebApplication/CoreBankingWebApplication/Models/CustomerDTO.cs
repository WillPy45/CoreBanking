namespace CoreBankingWebApplication.Models
{
    public class CustomerDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }

            
    }

    public static class CustomerDTOExtensions
    {
        // Cambiado a 'ToEntity' o 'MapTo' para que sea más claro lo que hace
        public static Customer ToEntity(this CustomerDTO dto)
        {
            return new Customer
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Address = dto.Address
            };
        }

        public static void UpdateEntity(this CustomerDTO dto, Customer customerEntity)
        {
            customerEntity.FirstName = dto.FirstName;
            customerEntity.LastName = dto.LastName;
            customerEntity.Address = dto.Address;
        }
    }

}
