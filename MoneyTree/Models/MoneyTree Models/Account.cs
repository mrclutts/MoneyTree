using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MoneyTree.Models.MoneyTree_Models
{
    public class Account
    {
        public Account()
        {
            this.Transactions = new HashSet<Transaction>();
            this.AccountHistories = new HashSet<AccountHistory>();
        }
        public virtual ICollection<Transaction> Transactions { get; set; }

        public virtual ICollection<AccountHistory> AccountHistories { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset Updated { get; set; }

        public int HouseholdId { get; set; }

        public virtual Household Household { get; set; }

    }
}