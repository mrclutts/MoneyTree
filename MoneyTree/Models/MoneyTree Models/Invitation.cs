using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MoneyTree.Models.MoneyTree_Models
{
    public class Invitation
    {
        public int Id { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public int VerifyCode { get; set; }

        public DateTimeOffset InviteDate { get; set; }

        public int HouseholdId { get; set; }

        public virtual Household Household { get; set; }
    }
}