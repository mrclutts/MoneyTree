using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MoneyTree.Models.MoneyTree_Models
{
    public class Budget
    {
        public Budget()
        {
            this.BudgetItemHistories = new HashSet<BudgetItemHistory>();
            this.BudgetItems = new HashSet<BudgetItem>();

        }

        public virtual ICollection<BudgetItem> BudgetItems { get; set; }

        public virtual ICollection<BudgetItemHistory> BudgetItemHistories { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset? Updated { get; set; }

        public decimal Limit { get; set; }

        public decimal Actual { get; set; }
        [ForeignKey("Household")]
        public int HouseholdId { get; set; }

        public virtual Household Household { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }       
    }
}