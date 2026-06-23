using Microsoft.EntityFrameworkCore;
using CoreBankingWebApplication.Models; 

namespace CoreBankingWebApplication.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Customer> Customers { get; set; }

        // Pass the configuration directly from the constructor instead overriding OnConfiguring method
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public ApplicationDbContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "Server=localhost;Database=CoreBankingDB;Trusted_Connection=True;TrustServerCertificate=True;";
            optionsBuilder.UseSqlServer(connectionString);
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
      
        }
        
    }
}
