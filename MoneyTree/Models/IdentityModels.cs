using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MoneyTree.Models.MoneyTree_Models;
using System.Collections;
using System.Collections.Generic;

namespace MoneyTree.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string Avatar { get; set; }
        public int ?HouseholdId { get; set; }
        public virtual Household Household { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

       

        public DbSet<Household> Households { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionType> TransactionTypes { get; set; }
        public DbSet<BudgetItemHistory> BudgetItemHistories { get; set; }
        public DbSet<AccountHistory> AccountHistories { get; set; }
        public DbSet<BudgetType> BudgetTypes { get; set; }
        public DbSet<BudgetItem> BudgetItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure HouseholdId as PK for Budget
            modelBuilder.Entity<Budget>()
                .HasKey(e => e.HouseholdId);

            // Configure HouseholdId as FK for Budget
            modelBuilder.Entity<Budget>()
                        .HasRequired(s => s.Household)
                        .WithRequiredPrincipal(ad => ad.Budget);

        }
    }
}