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

namespace MoneyTree.Controllers
{
    [RequireHttps]
    [AuthorizeHouseholdRequired]
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        public ActionResult Index()
        {
            var house = db.Households.Find(int.Parse(User.Identity.GetHouseholdId()));
            var transactions = house.Accounts.SelectMany(t => t.Transactions);
            return View(transactions.ToList());
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {
            int houseId = int.Parse(User.Identity.GetHouseholdId());
            ViewBag.BudgetTypeId = new SelectList(db.BudgetTypes.Where(h=>h.HouseholdId == houseId).OrderBy(n=>n.Name), "Id", "Name");
            ViewBag.AccountId = new SelectList(db.Accounts.Where(a => a.HouseholdId == houseId).OrderBy(n => n.Name), "Id", "Name");
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes.OrderBy(n => n.Name), "Id", "Name");
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Date,Amount,BudgetTypeId,AccountId,TransactionTypeId")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                transaction.UserId = user.Id;
                var account = db.Accounts.Find(transaction.AccountId);
                var transType = db.TransactionTypes.Find(transaction.TransactionTypeId);

                if (transType.Name == "Income")
                {
                    account.Balance = account.Balance + transaction.Amount;
                }
                else if(transType.Name == "Expense")
                {
                    account.Balance = account.Balance - transaction.Amount;
                }
                if(account.Reconciled == true)
                {
                    account.Reconciled = false;
                }
                db.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            int houseId = int.Parse(User.Identity.GetHouseholdId());
            ViewBag.BudgetTypeId = new SelectList(db.BudgetTypes.Where(h => h.HouseholdId == houseId).OrderBy(n => n.Name), "Id", "Name");
            ViewBag.AccountId = new SelectList(db.Accounts.Where(a => a.HouseholdId == houseId), "Id", "Name");
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes.OrderBy(n=>n.Name), "Id", "Name", transaction.TransactionTypeId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", transaction.UserId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            int houseId = int.Parse(User.Identity.GetHouseholdId());
            ViewBag.BudgetTypeId = new SelectList(db.BudgetTypes.Where(h=>h.HouseholdId==houseId || h.HouseholdId == null).OrderBy(n=>n.Name), "Id", "Name", transaction.BudgetTypeId);
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "Name", transaction.TransactionTypeId);
            ViewBag.AccountId = new SelectList(db.Accounts.Where(a => a.HouseholdId == houseId), "Id", "Name", transaction.AccountId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", transaction.UserId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Date,Amount,Reconciled,Void,BudgetTypeId,AccountId,TransactionTypeId")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                var oldTrans = db.Transactions.AsNoTracking().FirstOrDefault(t => t.Id == transaction.Id);
                var oldTransAccount = oldTrans.AccountId;
                var oldAccountId = db.Accounts.AsNoTracking().FirstOrDefault(a => a.Id == oldTrans.AccountId).Id;
                var oldAccount = db.Accounts.Find(oldAccountId);
                transaction.Updated = System.DateTimeOffset.Now;
                transaction.UserId = User.Identity.GetUserId();
                var transType = db.TransactionTypes.Find(transaction.TransactionTypeId);
                var account = db.Accounts.Find(transaction.AccountId);
                if (oldTrans.Amount != transaction.Amount || oldTrans.TransactionTypeId != transaction.TransactionTypeId || oldTrans.AccountId != transaction.AccountId)
                {
                    account.Reconciled = false;
                    if (transType.Name == "Income")
                    {
                        if(oldTrans.AccountId != transaction.AccountId)
                        {
                            oldAccount.Balance = oldAccount.Balance - transaction.Amount;
                        }
                        account.Balance = account.Balance + transaction.Amount;
                    }
                    else if (transType.Name == "Expense")
                    {
                        if (oldTrans.AccountId != transaction.AccountId)
                        {
                            oldAccount.Balance = oldAccount.Balance + transaction.Amount;
                        }
                        account.Balance = account.Balance - transaction.Amount;
                    }
                }
                if (transaction.Void == true)
                {
                    account.Reconciled = false;
                    account.Balance = account.Balance + transaction.Amount;
                }
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Dashboard","Households");
            }
            int houseId = int.Parse(User.Identity.GetHouseholdId());
            ViewBag.BudgetTypeId = new SelectList(db.BudgetTypes, "Id", "Name", transaction.BudgetTypeId);
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "Name", transaction.TransactionTypeId);
            ViewBag.AccountId = new SelectList(db.Accounts.Where(a => a.HouseholdId == houseId), "Id", "Name");
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", transaction.UserId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            var account = db.Accounts.Find(transaction.AccountId);
            if(transaction.TransactionType.Name == "Income")
            {
                account.Balance = account.Balance - transaction.Amount;
            }
            if (transaction.TransactionType.Name == "Expense")
            {
                account.Balance = account.Balance + transaction.Amount;
            }
            db.Transactions.Remove(transaction);
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
