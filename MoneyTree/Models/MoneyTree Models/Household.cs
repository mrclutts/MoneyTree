using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MoneyTree.Models.MoneyTree_Models
{
    public class Household
    {
        public int Id { get; set; }

        [Display(Name="Household Name")]
        public string Name { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset Updated { get; set; }

        public string CreatedById { get; set; }

        public virtual ApplicationUser CreatedBy { get; set; }

        public int BudgetId { get; set; }

        public virtual Budget Budget { get; set; }

        public Household()
        {
            this.Users = new HashSet<ApplicationUser>();
            
            this.Accounts = new HashSet<Account>();
        }
        public virtual ICollection<ApplicationUser> Users { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
    }
}