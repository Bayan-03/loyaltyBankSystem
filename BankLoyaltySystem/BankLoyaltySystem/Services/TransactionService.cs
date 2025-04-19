using BankLoyaltySystem.Data;
using BankLoyaltySystem.Models;

namespace BankLoyaltySystem.Services
{
    public class TransactionService : ITransactionService
    {

        private readonly BankLoyaltySystem1Context _context;

        public TransactionService(BankLoyaltySystem1Context context)
        {
            _context = context;
        }

        public CustomerFullDetail GetCustomerWithDetails(int customerId)
        {
            return _context.CustomerFullDetails.FirstOrDefault(c => c.CustomerId == customerId);
        }

        public void Deposit(int customerId, decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero");

            var customer = _context.Customers.Find(customerId);
            if (customer == null)
                throw new KeyNotFoundException("Customer not found");

            // Update balance
            customer.AccountBalance = (customer.AccountBalance ?? 0) + amount;

            // Record deposit
            _context.Deposits.Add(new Deposit
            {
                CustomerId = customerId,
                Amount = amount,
                DepositDate = DateTime.Now
            });

            // Increment deposit count
            customer.DepositCount = (customer.DepositCount ?? 0) + 1;

            _context.SaveChanges();
            UpdateLoyaltyPoints(customerId);
        }

        public void Withdraw(int customerId, decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero");

            var customer = _context.Customers.Find(customerId);
            if (customer == null)
                throw new KeyNotFoundException("Customer not found");

            if ((customer.AccountBalance ?? 0) < amount)
                throw new InvalidOperationException("Insufficient balance");

            // Update balance
            customer.AccountBalance = (customer.AccountBalance ?? 0) - amount;

            // Record withdrawal
            _context.Withdrawals.Add(new Withdrawal
            {
                CustomerId = customerId,
                Amount = amount,
                WithdrawalDate = DateTime.Now
            });

            _context.SaveChanges();
        }

        public void UpdateLoyaltyPoints(int customerId)
        {
            var customer = _context.Customers.Find(customerId);
            if (customer == null) return;

            int depositCount = customer.DepositCount ?? 0;

            // Bronze: 1 point per deposit up to 10
            customer.BronzePoints = Math.Min(depositCount, 10);

            // Silver: 1 point per deposit from 11 to 20
            customer.SilverPoints = depositCount > 10 ? Math.Min(depositCount - 10, 10) : 0;

            // Gold: 1 point per deposit after 20
            customer.GoldPoints = depositCount > 20 ? depositCount - 20 : 0;

            _context.SaveChanges();
        }
    }
}