using System;
using System.Collections.Generic;

namespace BankLoyaltySystem.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Deposits = new HashSet<Deposit>();
            Withdrawals = new HashSet<Withdrawal>();
        }

        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = null!;
        public decimal? AccountBalance { get; set; }
        public int? DepositCount { get; set; }
        public int? BronzePoints { get; set; }
        public int? SilverPoints { get; set; }
        public int? GoldPoints { get; set; }

        public virtual ICollection<Deposit> Deposits { get; set; }
        public virtual ICollection<Withdrawal> Withdrawals { get; set; }
    }
}
