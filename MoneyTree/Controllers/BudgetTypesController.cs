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

namespace MoneyTree.Controllers
{
    public class BudgetTypesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BudgetTypes
        public ActionResult Index()
        {
            int houseId = int.Parse(User.Identity.GetHouseholdId());
            var budgetTypes = db.BudgetTypes.Where(h=>h.HouseholdId == houseId);
            return View(budgetTypes.ToList());
        }

        // GET: BudgetTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetType budgetType = db.BudgetTypes.Find(id);
            if (budgetType == null)
            {
                return HttpNotFound();
            }
            return View(budgetType);
        }

        // GET: BudgetTypes/Create
        public ActionResult Create()
        {
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name");
            return View();
        }

        // POST: BudgetTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] BudgetType budgetType)
        {
            if (ModelState.IsValid)
            {
                int houseId = int.Parse(User.Identity.GetHouseholdId());
                budgetType.HouseholdId = houseId;
                db.BudgetTypes.Add(budgetType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", budgetType.HouseholdId);
            return View(budgetType);
        }

        // GET: BudgetTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetType budgetType = db.BudgetTypes.Find(id);
            if (budgetType == null)
            {
                return HttpNotFound();
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", budgetType.HouseholdId);
            return View(budgetType);
        }

        // POST: BudgetTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,HouseholdId")] BudgetType budgetType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(budgetType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", budgetType.HouseholdId);
            return View(budgetType);
        }

        // GET: BudgetTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetType budgetType = db.BudgetTypes.Find(id);
            if (budgetType == null)
            {
                return HttpNotFound();
            }
            return View(budgetType);
        }

        // POST: BudgetTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BudgetType budgetType = db.BudgetTypes.Find(id);
            db.BudgetTypes.Remove(budgetType);
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
