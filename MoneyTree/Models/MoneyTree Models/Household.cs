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

        [Display(Name="Household Name")]
        public string Name { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset? Updated { get; set; }

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; }

        [ForeignKey("Budget")]
        public int? BudgetId { get; set; }

        public virtual Budget Budget { get; set; }

        public Household()
        {
            this.Users = new HashSet<ApplicationUser>();
            
            this.Accounts = new HashSet<Account>();

            this.Invitations = new HashSet<Invitation>();
        }
        public virtual ICollection<ApplicationUser> Users { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }

        public virtual ICollection<Invitation> Invitations { get; set; }
    }
}