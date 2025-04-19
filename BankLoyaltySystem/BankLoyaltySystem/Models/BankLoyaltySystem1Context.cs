using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BankLoyaltySystem.Models
{
    public partial class BankLoyaltySystem1Context : DbContext
    {
        public BankLoyaltySystem1Context()
        {
        }

        public BankLoyaltySystem1Context(DbContextOptions<BankLoyaltySystem1Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<CustomerFullDetail> CustomerFullDetails { get; set; } = null!;
        public virtual DbSet<Deposit> Deposits { get; set; } = null!;
        public virtual DbSet<Withdrawal> Withdrawals { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=MYCUMPUTER;Database=BankLoyaltySystem1;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.CustomerId).HasColumnName("customerID");

                entity.Property(e => e.AccountBalance)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("accountBalance")
                    .HasDefaultValueSql("((0.00))");

                entity.Property(e => e.BronzePoints)
                    .HasColumnName("bronzePoints")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.CustomerName)
                    .HasMaxLength(100)
                    .HasColumnName("customerName");

                entity.Property(e => e.DepositCount)
                    .HasColumnName("depositCount")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.GoldPoints)
                    .HasColumnName("goldPoints")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.SilverPoints)
                    .HasColumnName("silverPoints")
                    .HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<CustomerFullDetail>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("CustomerFullDetails");

                entity.Property(e => e.AccountBalance)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("accountBalance");

                entity.Property(e => e.BronzePoints).HasColumnName("bronzePoints");

                entity.Property(e => e.CustomerId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("customerID");

                entity.Property(e => e.CustomerName)
                    .HasMaxLength(100)
                    .HasColumnName("customerName");

                entity.Property(e => e.GoldPoints).HasColumnName("goldPoints");

                entity.Property(e => e.SilverPoints).HasColumnName("silverPoints");

                entity.Property(e => e.TotalDepositedAmount)
                    .HasColumnType("decimal(38, 2)")
                    .HasColumnName("totalDepositedAmount");

                entity.Property(e => e.TotalDeposits).HasColumnName("totalDeposits");

                entity.Property(e => e.TotalWithdrawals).HasColumnName("totalWithdrawals");

                entity.Property(e => e.TotalWithdrawnAmount)
                    .HasColumnType("decimal(38, 2)")
                    .HasColumnName("totalWithdrawnAmount");
            });

            modelBuilder.Entity<Deposit>(entity =>
            {
                entity.Property(e => e.DepositId).HasColumnName("depositID");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("amount");

                entity.Property(e => e.CustomerId).HasColumnName("customerID");

                entity.Property(e => e.DepositDate)
                    .HasColumnType("datetime")
                    .HasColumnName("depositDate")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Deposits)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__Deposits__custom__3F466844");
            });

            modelBuilder.Entity<Withdrawal>(entity =>
            {
                entity.Property(e => e.WithdrawalId).HasColumnName("withdrawalID");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("amount");

                entity.Property(e => e.CustomerId).HasColumnName("customerID");

                entity.Property(e => e.WithdrawalDate)
                    .HasColumnType("datetime")
                    .HasColumnName("withdrawalDate")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Withdrawals)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__Withdrawa__custo__4316F928");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
