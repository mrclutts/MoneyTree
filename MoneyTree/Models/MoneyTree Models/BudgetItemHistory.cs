using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MoneyTree.Models.MoneyTree_Models
{
    public class BudgetItemHistory
    {
        public int Id { get; set; }

        public int BudgetItemId { get; set; }

        public virtual BudgetItem BudgetItem { get; set; }

        public DateTimeOffset Created { get; set; }

        public decimal? OldLimit { get; set; }

        public decimal NewLimit { get; set; }

        public string UpdatedById { get; set; }

        public virtual ApplicationUser UpdatedBy { get; set; }
    }
}