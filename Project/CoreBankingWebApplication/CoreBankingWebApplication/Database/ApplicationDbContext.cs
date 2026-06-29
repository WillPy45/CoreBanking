using Microsoft.EntityFrameworkCore;
using CoreBankingWebApplication.Models; 

namespace CoreBankingWebApplication.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<BalanceTransaction> BalanceTransactions { get; set; }
        public DbSet<Currency> Currencies { get; set; }
    
        public DbSet<CurrencyExchangeReserve> CurrencyExchangeReserves { get; set; }
        public DbSet<CurrencyExchange> CurrencyExchanges { get; set; }


        // Pass the configuration directly from the constructor instead overriding OnConfiguring method
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public ApplicationDbContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "server=127.0.0.1;uid=EFUser;pwd=EF123;database=CoreBankingDb";
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // ==== BalanceTransaction Entity ====
            // Configure relation for Sender BankAccount
            modelBuilder.Entity<BalanceTransaction>()
                .HasOne((entity) => entity.SenderBankAccount) // Navigation property
                .WithMany((referredEntity) =>                 // Reverse navigation property for the referred entity
                    referredEntity.SentTransactions)
                .HasForeignKey((entity) => entity.SenderBankAccountId); // Foreign Key

            // Configure relation for Receiver BankAccount
             modelBuilder.Entity<BalanceTransaction>()
                .HasOne((entity) => entity.ReceiverBankAccount) // Navigation property
                .WithMany((referredEntity) =>                 // Reverse navigation property for the referred entity
                    referredEntity.ReceivedTransactions)
                .HasForeignKey((entity) => entity.ReceiverBankAccountId); // Foreign Key

            // ========================================================================================

            // ==== Currency Entity ====
            modelBuilder.Entity<Currency>()
                .HasIndex((entity) => entity.Code)
                .IsUnique();
                

            // ==== BankAccount Entity ==== 
            modelBuilder.Entity<BankAccount>()
                .HasOne((entity) => entity.Customer)
                .WithMany((referredEntity) => referredEntity.BankAccounts)
                .HasForeignKey((entity) => entity.CustomerId);

            modelBuilder.Entity<BankAccount>()
                .HasOne((entity) => entity.Currency)
                .WithMany((referredEntity) => referredEntity.BankAccounts)
                .HasForeignKey((entity) => entity.CurrencyId);
            // ==== Customer Entity ====
            // It´s maybe well induced by EF

            // CurrencyExchangeReserve
            modelBuilder.Entity<CurrencyExchangeReserve>()
                .HasOne((entity) => entity.Currency)
                .WithOne((referredEntity) => referredEntity.ExchangeReserve)
                .HasForeignKey<CurrencyExchangeReserve>((e) => e.CurrencyId);

            // ==== CurrencyExchange Entity ====
            // Relation with Sender
            modelBuilder.Entity<CurrencyExchange>()
                

            // Relation with Receiver

            // Relation with CurrencyExchangeReserve

        }
        
    }
}
