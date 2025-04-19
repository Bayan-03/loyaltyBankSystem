using System;
using System.Collections.Generic;

namespace BankLoyaltySystem.Models
{
    public partial class CustomerFullDetail
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = null!;
        public decimal? AccountBalance { get; set; }
        public int? BronzePoints { get; set; }
        public int? SilverPoints { get; set; }
        public int? GoldPoints { get; set; }
        public int? TotalDeposits { get; set; }
        public int? TotalWithdrawals { get; set; }
        public decimal? TotalDepositedAmount { get; set; }
        public decimal? TotalWithdrawnAmount { get; set; }
    }
}
