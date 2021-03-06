﻿using System;
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
    public class AccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Accounts
        public ActionResult Index()
        {
            int houseId = int.Parse(User.Identity.GetHouseholdId());
            var accounts = db.Accounts.Where(t => t.HouseholdId == houseId);
            return View(accounts.ToList());
        }

        // GET: Accounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // GET: Accounts/Create
        public ActionResult Create()
        {
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name");
            return View();
        }

        // GET: Accounts/Create
        public PartialViewResult _CreateAccount()
        {
            //ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name");
            return PartialView();
        }
        //Get: Accounts/Reconcile 
        public ActionResult Reconcile()
        {
            int houseId = int.Parse(User.Identity.GetHouseholdId());
            var accounts = db.Accounts.Where(t => t.HouseholdId == houseId);
            return View(accounts.ToList());
        }
        //Post: Accounts/Reconcile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reconcile(int accountId, decimal userBalance)
        {
            var user = User.Identity.GetUserId();
            var account = db.Accounts.Find(accountId);
            var expenses = account.Transactions.Where(t => t.TransactionType.Name == "Expense").Where(t => t.Reconciled == false).Select(t => t.Amount).Sum();
            var income = account.Transactions.Where(t => t.TransactionType.Name == "Income").Where(t => t.Reconciled == false).Select(t => t.Amount).Sum();
            var accountBalance = income - expenses;
            if (userBalance == accountBalance)
            {
                account.Reconciled = true;
                db.SaveChanges();
            }
            else
            {
                var difference = userBalance - accountBalance;
                Transaction transaction = new Transaction();
                transaction.Name = "DifferenceId";
                transaction.Date = System.DateTimeOffset.Now;
                transaction.AccountId = accountId;
                transaction.BudgetTypeId = db.BudgetTypes.FirstOrDefault(n => n.Name == "Miscellaneous")?.Id;
                transaction.Amount = difference;
                if (transaction.Amount >= 0)
                {
                    transaction.TransactionTypeId = (int)db.TransactionTypes.FirstOrDefault(n => n.Name == "Income")?.Id;
                }
                else if (transaction.Amount <= 0)
                {
                    transaction.TransactionTypeId = (int)db.TransactionTypes.FirstOrDefault(n => n.Name == "Expense")?.Id;
                }
                transaction.Reconciled = true;
                transaction.UserId = user;
                db.Transactions.Add(transaction);
                db.SaveChanges();

                foreach (var trans in db.Transactions.Where(s => s.Reconciled == false).ToList())
                {
                    trans.Reconciled = true;
                    db.SaveChanges();
                };
                account.Balance = userBalance;
                account.Reconciled = true;
                db.SaveChanges();
            }
            return RedirectToAction("Dashboard", "Households");
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Balance")] Account account)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                account.Created = System.DateTimeOffset.Now;
                account.HouseholdId = (int)user.HouseholdId;

                db.Accounts.Add(account);
                db.SaveChanges();
                return RedirectToAction("Dashboard", "Households");
            }

            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", account.HouseholdId);
            return View(account);
        }

        // GET: Accounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", account.HouseholdId);
            return View(account);
        }

        // GET: Accounts/Edit/5
        public PartialViewResult _EditAccount(int? id)
        {

            Account account = db.Accounts.Find(id);


            return PartialView(account);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Balance,HouseholdId")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Entry(account).State = EntityState.Modified;
                account.Updated = System.DateTimeOffset.Now;
                // db.Entry(account).Property("Name").IsModified = true;
                // db.Entry(account).Property("Balance").IsModified = true;

                db.SaveChanges();
                return RedirectToAction("Dashboard", "Households");
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", account.HouseholdId);
            return RedirectToAction("Dashboard", "Households");
        }

        // GET: Accounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Account account = db.Accounts.Find(id);
            db.Accounts.Remove(account);
            db.SaveChanges();
            return RedirectToAction("Dashboard", "Households");
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
