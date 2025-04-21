namespace BankLoyaltySystem.Models.ViewModels
{
    public class TransactionHistoryViewModel
    {
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
