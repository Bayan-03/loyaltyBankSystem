using System;
using System.Collections.Generic;

namespace BankLoyaltySystem.Models
{
    public partial class Deposit
    {
        public int DepositId { get; set; }
        public int? CustomerId { get; set; }
        public decimal Amount { get; set; }
        public DateTime? DepositDate { get; set; }

        public virtual Customer? Customer { get; set; }
    }
}
