using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Project5.Data;
using Project5.Models;

namespace Project5.Controllers
{
    [Authorize]
    public class BorrowingsController : Controller
    {
        private Project5Context db = new Project5Context();
        

        // GET: Borrowings
        public ActionResult Index()
        {
            var borrowings = db.Borrowings.Include(b => b.Book).Include(b => b.User);
            return View(borrowings.ToList());
        }

        // GET: Borrowings/Details/5
     
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Borrowing borrowing = db.Borrowings.Find(id);
            if (borrowing == null)
            {
                return HttpNotFound();
            }
            return View(borrowing);
        }

        // GET: Borrowings/Create
        
        public ActionResult Create()
        {
            ViewBag.BookId = new SelectList(db.Books, "BookId", "Title");
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Name");
            return View();
        }

        // POST: Borrowings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BorrowingId,BookId,UserId,BorrowDate,ReturnDate")] Borrowing borrowing)
        {
            if (ModelState.IsValid)
            {
                db.Borrowings.Add(borrowing);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BookId = new SelectList(db.Books, "BookId", "Title", borrowing.BookId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Name", borrowing.UserId);
            return View(borrowing);
        }

        // GET: Borrowings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Borrowing borrowing = db.Borrowings.Find(id);
            if (borrowing == null)
            {
                return HttpNotFound();
            }
            ViewBag.BookId = new SelectList(db.Books, "BookId", "Title", borrowing.BookId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Name", borrowing.UserId);
            return View(borrowing);
        }

        // POST: Borrowings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BorrowingId,BookId,UserId,BorrowDate,ReturnDate")] Borrowing borrowing)
        {
            if (ModelState.IsValid)
            {
                db.Entry(borrowing).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BookId = new SelectList(db.Books, "BookId", "Title", borrowing.BookId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Name", borrowing.UserId);
            return View(borrowing);
        }

        // GET: Borrowings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Borrowing borrowing = db.Borrowings.Find(id);
            if (borrowing == null)
            {
                return HttpNotFound();
            }
            return View(borrowing);
        }

        // POST: Borrowings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Borrowing borrowing = db.Borrowings.Find(id);
            db.Borrowings.Remove(borrowing);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
     
       [Authorize]
        public ActionResult Summary()
        {
            var summary = db.Borrowings.Include(b => b.Book).Include(b => b.User)
                           .Select(b => new BorrowingSummaryViewModel
                           {
                               BookTitle = b.Book.Title,
                               UserName = b.User.Name,
                               BorrowDate = b.BorrowDate,
                               ReturnDate = b.ReturnDate
                           }).ToList();
            return View(summary);
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
