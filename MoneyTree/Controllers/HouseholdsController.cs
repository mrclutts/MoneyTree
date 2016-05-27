using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MoneyTree.Models;
using MoneyTree.Models.MoneyTree_Models;
using Microsoft.AspNet.Identity;
using MoneyTree.Helpers;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Helpers;
using Newtonsoft.Json;

namespace MoneyTree.Controllers
{
    [RequireHttps]
    public class HouseholdsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Households
        public ActionResult Index()
        {
            var households = db.Households.Include(h => h.BudgetItems).Include(h => h.CreatedBy);
            return View(households.ToList());
        }

        // GET: Households/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        //Get:  Households/Dashboard
        [AuthorizeHouseholdRequired]
        public ActionResult Dashboard()
        {
            
            string houseId = User.Identity.GetHouseholdId();
            Household house = db.Households.Find(Int32.Parse(houseId));


            return View(house);
        }
        //Get Households/TransactionsChart
        public ActionResult AccountChart()
        {
            var house = db.Households.Find(int.Parse(User.Identity.GetHouseholdId()));
            var data = (from acct in house.Accounts
                        let sum = (from trans in acct.Transactions.Where(x => x.Reconciled == false)
                                   select trans.Amount).DefaultIfEmpty().Sum()
                        select new
                        {
                            label = acct.Name,
                            value = sum
                        }).ToArray();
            return Content(JsonConvert.SerializeObject(data), "application/json");
        }

        //Get Households/BudgetChart
        public ActionResult BudgetChart()
        {
            var house = db.Households.Find(int.Parse(User.Identity.GetHouseholdId()));
            var data = (from budget in house.BudgetTypes
                        let sum = (from bud in house.BudgetItems
                                   select bud.BudgetedAmount).DefaultIfEmpty().Sum()
                        select new
                        {
                            label = budget.Name,
                            value = sum
                        }).ToArray();
            return Content(JsonConvert.SerializeObject(data), "application/json");
        }
        // GET: Households/Create
        public ActionResult Create()
        {
            
            return View();
        }

        // GET: Households/CreateJoinHousehold
        public ActionResult CreateJoinHousehold()
        {

            return View();
        }

        // POST: Households/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Name")] Household household)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsInHousehold())
                {
                    return RedirectToAction("Dashboard", "Households");
                }
                var user = db.Users.Find(User.Identity.GetUserId());
                
                household.Created = System.DateTimeOffset.Now;
                household.CreatedBy = user.FirstName + " " + user.LastName;
                
                
                db.Households.Add(household);               
                db.SaveChanges();
                user.HouseholdId = household.Id;
                var BudgetTypes = db.BudgetTypes.Where(b => b.HouseholdId == null).ToList();
                foreach (var bud in BudgetTypes)
                {
                    bud.HouseholdId = household.Id;
                }
                db.BudgetTypes.AddRange(BudgetTypes);
                db.SaveChanges();

                await ControllerContext.HttpContext.RefreshAuthentication(user);
                return RedirectToAction("Dashboard", "Households");
            }

            return View(household);
        }

        
        // GET: Households/Invite
        public ActionResult Invite()
        {
            
            return View();
        }

        //POST: Households/Invite
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Invite([Bind(Include = "Id,Email")] Invitation invitation)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                invitation.InviteDate = System.DateTimeOffset.Now;
                invitation.HouseholdId = (int)user.HouseholdId;
                var house = db.Households.Find(invitation.HouseholdId);
                Random rnd = new Random();
                int code = rnd.Next(101, 999) * rnd.Next(101, 999);
                invitation.VerifyCode = code;
                db.Invitations.Add(invitation);
                db.SaveChanges();

                EmailService es = new EmailService();
                IdentityMessage im = new IdentityMessage();
                im.Subject = "Invitation from Money Tree!";
                im.Destination = invitation.Email;
                
                im.Body = "Hi,<br/>" + user.DisplayName + " has invited you to join" + house.Name + "!<br/>" + "Please enter your Code: " + invitation.VerifyCode + "<br/>Click the link to accept!" + "<a href='https://mrclutts-budget.azurewebsites.net/households/createjoinhousehold/'></a>";
                await es.SendAsync(im);

                return RedirectToAction("Dashboard", "Households");
            }

            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", invitation.HouseholdId);
            return View(invitation);
        }

        // POST: Households/Join
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Join(int code)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                var invite = db.Invitations.FirstOrDefault(e => e.Email == user.Email && e.VerifyCode == code);
                if (invite != null)
                {
                    var house = invite.Household;
                    HouseholdHelper helper = new HouseholdHelper();

                    if (!(helper.HasHouse(user.Id)))
                    {
                        house.Users.Add(user);
                        db.Invitations.Remove(invite);
                        db.SaveChanges();
                        await ControllerContext.HttpContext.RefreshAuthentication(user);
                        return RedirectToAction("Dashboard", "Households");
                    }

                }

            }
            return RedirectToAction("CreateJoinHousehold", "Households");
        }
    

        //Post: Households/Leave
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Leave(int householdId)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                var house = db.Households.Find(householdId);
                                
                if(User.Identity.IsInHousehold())
                {
                    if (house.Users.Count() == 1)
                    {
                        house.Users.Remove(user);
                        house.IsDeleted = true;
                        db.SaveChanges();
                    }
                    else
                    {
                        house.Users.Remove(user);
                        db.SaveChanges();
                    }                  
                }
                await ControllerContext.HttpContext.RefreshAuthentication(user);

            }
            
            return RedirectToAction("CreateJoinHousehold", "Households");
        }

        // GET: Households/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id = new SelectList(db.BudgetItems, "HouseholdId", "Name", household.Id);
            ViewBag.CreatedById = new SelectList(db.Users, "Id", "FirstName", household.CreatedBy);
            return View(household);
        }

        // POST: Households/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Household household)
        {
            if (ModelState.IsValid)
            {
                db.Entry(household).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id = new SelectList(db.BudgetItems, "HouseholdId", "Name", household.Id);
            ViewBag.CreatedById = new SelectList(db.Users, "Id", "FirstName", household.CreatedBy);
            return View(household);
        }

        // GET: Households/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Household household = db.Households.Find(id);
            db.Households.Remove(household);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
