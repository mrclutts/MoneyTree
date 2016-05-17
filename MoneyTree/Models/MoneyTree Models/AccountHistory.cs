using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MoneyTree.Models.MoneyTree_Models
{
    public class AccountHistory
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public virtual Account Account { get; set; }

        public decimal ?OldBalance { get; set; }

        public decimal NewBalance { get; set; }

        public string UpdatedById { get; set; }

        public virtual ApplicationUser UpdatedBy { get; set; }
    }
}