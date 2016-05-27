using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MoneyTree.Models.MoneyTree_Models
{
    public class Transaction
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTimeOffset Date { get; set; }

        public DateTimeOffset? Updated { get; set; }

        public decimal Amount { get; set; }

        public bool Reconciled { get; set; }

        public bool Void { get; set; }

        public int? BudgetTypeId { get; set; }

        public virtual BudgetType BudgetType { get; set; }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public int AccountId { get; set; }

        public virtual Account Account { get; set; }

        public int TransactionTypeId { get; set; }

        public virtual TransactionType TransactionType { get; set; }

        
    }
}