using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MoneyTree.Models.MoneyTree_Models
{
    public class BudgetItem
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset? Updated { get; set; }

        [Display(Name="Budgeted Amount")]
        public decimal BudgetedAmount { get; set; }

        [Display(Name="Actual Amount")]
        public decimal ActualAmount { get; set; }

        public int? BudgetTypeId { get; set; }

        public virtual BudgetType BudgetType { get; set; }

        public int HouseholdId { get; set; }

        public virtual Household Household { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }       
    }
}