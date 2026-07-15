namespace CoreBankingWebApplication.Models.Entities
{
    /// <summary>
    /// A base class that contains commons entity properties
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Gets/Sets the primary key of the entity
        /// </summary>
        public int Id { get; set; }


        /// <summary>
        /// Gets/Sets the create time in UTC
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>
        /// Gets/Sets the last update time in UTC
        /// </summary>
        public DateTime LastUpdatedUtc { get; set; }



    }
}

// See more later INTECEPTORS Entity Framework to implements a system that automaticaly add commons values 