namespace CoreBankingWebApplication.Models.Entities
{
    /// <summary>
    /// Represents a technical profile
    /// </summary>
    /// <remarks>
    /// A profile is useful to manage diverse context,
    /// for example, what happens if the Profile is an Admin, an more.
    /// Divided the concerns in properties is useful to manage from the general to the core domain
    /// </remarks>
    public class Profile : BaseEntity
    {
        /// <summary>
        /// Gets/Sets the user name
        /// </summary>
        /// <remarks>
        /// This can be a ficticial user name, has not rules like a legal full name
        /// </remarks>
        public required string UserName { get; set; }

        public required string Email { get; set; }

        /// <summary>
        /// Gets/Sets the hashed password
        /// </summary>
        /// <remarks>
        /// Hashed password depends of external services,
        /// is useful to be sure what kind of hash technique is used for specific purpose
        /// </remarks>
        public required string PasswordHash { get; set; }


        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }

        /// <summary>
        /// Gets/Sets the role profile
        /// </summary>
        /// <remarks>
        /// Useful for role restricted behaviors
        /// </remarks>
        public RoleType Role { get; set; }
    }

    /// <summary>
    /// The possible profile roles
    /// </summary>
    public enum RoleType
    {
        Admin,
        Operator,
        Customer,
        Visitor
    }
}
