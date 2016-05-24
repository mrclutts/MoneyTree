using MoneyTree.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MoneyTree.Helpers
{
    public class HouseholdHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public bool IsUserAssigned(string userId, int householdId)
        {
            var house = db.Households.FirstOrDefault(x => x.Id == householdId);
            //Any() checks to see if there is anything returned by the query
            if (house.Users.Where(x => x.Id == userId).Any())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool HasHouse(string userId)
        {
            var user = db.Users.FirstOrDefault(x => x.Id == userId);
            if(!(user.HouseholdId == null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}