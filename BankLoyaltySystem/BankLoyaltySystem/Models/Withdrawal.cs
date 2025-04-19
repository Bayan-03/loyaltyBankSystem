using System;
using System.Collections.Generic;

namespace BankLoyaltySystem.Models
{
    public partial class Withdrawal
    {
        public int WithdrawalId { get; set; }
        public int? CustomerId { get; set; }
        public decimal Amount { get; set; }
        public DateTime? WithdrawalDate { get; set; }

        public virtual Customer? Customer { get; set; }
    }
}
