using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MoneyTree.Models.MoneyTree_Models
{
    public class Household
    {
        public int Id { get; set; }

        public Household()
        {
            this.BudgetItems = new HashSet<BudgetItem>();

            this.Users = new HashSet<ApplicationUser>();

            this.Accounts = new HashSet<Account>();

            this.Invitations = new HashSet<Invitation>();

            this.BudgetTypes = new HashSet<BudgetType>();
        }
        public virtual ICollection<ApplicationUser> Users { get; set; }

        public virtual ICollection<BudgetItem> BudgetItems { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }

        public virtual ICollection<Invitation> Invitations { get; set; }

        public virtual ICollection<BudgetType> BudgetTypes { get; set; }
        
        [Display(Name="Household Name")]
        public string Name { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset? Updated { get; set; }

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; }    

       
    }
}