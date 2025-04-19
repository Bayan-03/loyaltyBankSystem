using BankLoyaltySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BankLoyaltySystem.Data
{
    public class BankLoyaltyContext : DbContext
    {
        public BankLoyaltyContext(DbContextOptions<BankLoyaltyContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<CustomerFullDetail> CustomerFullDetails { get; set; }
        public virtual DbSet<Deposit> Deposits { get; set; }
        public virtual DbSet<Withdrawal> Withdrawals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.AccountBalance).HasDefaultValue(0.00m);
                entity.Property(e => e.BronzePoints).HasDefaultValue(0);
                entity.Property(e => e.SilverPoints).HasDefaultValue(0);
                entity.Property(e => e.GoldPoints).HasDefaultValue(0);
                entity.Property(e => e.DepositCount).HasDefaultValue(0);
            });

            modelBuilder.Entity<Deposit>(entity =>
            {
                entity.Property(e => e.DepositDate).HasDefaultValueSql("getdate()");
                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Deposits)
                    .HasForeignKey(d => d.CustomerId);
            });

            modelBuilder.Entity<Withdrawal>(entity =>
            {
                entity.Property(e => e.WithdrawalDate).HasDefaultValueSql("getdate()");
                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Withdrawals)
                    .HasForeignKey(d => d.CustomerId);
            });
        }
    }
}