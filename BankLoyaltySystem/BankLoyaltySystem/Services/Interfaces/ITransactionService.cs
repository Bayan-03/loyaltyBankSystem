using BankLoyaltySystem.Models;

namespace BankLoyaltySystem.Services
{
    public interface ITransactionService
    {
        CustomerFullDetail GetCustomerWithDetails(int customerId);
        void Deposit(int customerId, decimal amount);
        void Withdraw(int customerId, decimal amount);
        void UpdateLoyaltyPoints(int customerId);
    }
}