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
using Microsoft.AspNet.Identity;
using System.Diagnostics;

namespace Project5.Controllers
{
    [Authorize]
    public class BorrowingsController : Controller
    {
        private Project5Context db = new Project5Context();


        // GET: Borrowings
        [Authorize]
        public ActionResult Index()
        {
            var currentUserId = User.Identity.GetUserId();
            var currentUser = db.Users.FirstOrDefault(u => u.Email == User.Identity.Name);

            if (currentUser == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "User not found.");
            }

            IQueryable<Borrowing> borrowingsQuery = db.Borrowings.Include(b => b.Book).Include(b => b.User);

            // Filter to only show borrowings that have not been returned
            borrowingsQuery = borrowingsQuery.Where(b => !b.ActualReturnDate.HasValue);

            if (!User.IsInRole("Librarian") && !User.IsInRole("Manager"))
            {
                borrowingsQuery = borrowingsQuery.Where(b => b.UserId == currentUser.UserId);
            }

            var borrowingsList = borrowingsQuery.ToList();

            if (!borrowingsList.Any())
            {
                Debug.WriteLine("No currently borrowed books found.");
            }

            return View(borrowingsList);
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
                // Decrease the book's quantity by 1
                var book = db.Books.Find(borrowing.BookId);
                if (book != null && book.Quantity > 0)
                {
                    book.Quantity -= 1; // Decrease quantity

                    db.Entry(book).State = EntityState.Modified; // Mark the book entity as modified
                }
                else
                {
                    // Handle the case where the book is not found or there are no more copies available
                    ModelState.AddModelError("", "The book is not available for borrowing.");
                    return View(borrowing);
                }

                db.Borrowings.Add(borrowing);
                db.SaveChanges(); // Save changes for both the borrowing and the updated book quantity

                return RedirectToAction("Index");
            }

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
        public ActionResult Return(int? borrowingId)
        {
            if (borrowingId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var borrowing = db.Borrowings.Find(borrowingId);
            if (borrowing == null)
            {
                return HttpNotFound();
            }

            // Check if the book hasn't been returned yet by looking at ActualReturnDate
            if (!borrowing.ActualReturnDate.HasValue)
            {
                // Set the actual return date to now
                borrowing.ActualReturnDate = DateTime.Now;

                var book = db.Books.Find(borrowing.BookId);
                if (book != null)
                {
                    book.Quantity += 1; // Return the book to the inventory
                    db.Entry(book).State = EntityState.Modified;
                }

                db.Entry(borrowing).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Success"] = "The book has been successfully returned.";
            }
            else
            {
                TempData["Info"] = "This book has already been returned.";
            }

            return RedirectToAction("Index");
        }

        // GET: Borrowings/Summary
        [Authorize]
        public ActionResult Summary()
        {
            var currentUserId = User.Identity.GetUserId();
            // Assuming that the email is the username and is used to log in
            var currentUser = db.Users.FirstOrDefault(u => u.Email == User.Identity.Name);

            if (currentUser == null)
            {
                // The user could not be found in the database. This means either the user is not logged in
                // or there is an issue with the user data.
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "User not found.");
            }

            IQueryable<BorrowingSummaryViewModel> summaryQuery;

            if (User.IsInRole("Librarian") || User.IsInRole("Manager"))
            {
                summaryQuery = db.Borrowings
                                 .Include(b => b.Book)
                                 .Include(b => b.User)
                                 .Select(b => new BorrowingSummaryViewModel
                                 {
                                     BookTitle = b.Book.Title,
                                     UserName = b.User.Name,
                                     BorrowDate = b.BorrowDate,
                                     ReturnDate = b.ReturnDate,
                                     ActualReturnDate = b.ActualReturnDate
                                 });
            }
            else
            {
                summaryQuery = db.Borrowings
                                 .Where(b => b.UserId == currentUser.UserId)
                                 .Include(b => b.Book)
                                 .Select(b => new BorrowingSummaryViewModel
                                 {
                                     BookTitle = b.Book.Title,
                                     BorrowDate = b.BorrowDate,
                                     ReturnDate = b.ReturnDate,
                                     ActualReturnDate = b.ActualReturnDate
                                 });
            }

            var summaryList = summaryQuery.ToList();
            return View(summaryList);
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
