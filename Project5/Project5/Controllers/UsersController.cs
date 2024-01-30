using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Project5.Data;
using Project5.Models;
using BCrypt.Net;
using System.Web.Security;

namespace Project5.Controllers
{
    public class UsersController : Controller
    {
        private Project5Context db = new Project5Context();

        // GET: Users/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Users/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password, bool rememberMe = false)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.FirstOrDefault(u => u.Email == email);

                if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                {
                    FormsAuthentication.SetAuthCookie(email, rememberMe);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid email or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View();
        }

        // GET: Users/Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Users");
        }

        [Authorize (Roles="Librarian,Manager")]
        // GET: Users
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        // GET: Users/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            User user;

            // If no ID is specified, or if the current user is not a librarian or admin,
            // default to showing the current user's own details.
            if (!id.HasValue || !User.IsInRole("Librarian") && !User.IsInRole("Admin"))
            {
                var userEmail = User.Identity.Name;
                user = db.Users.FirstOrDefault(u => u.Email.Equals(userEmail, StringComparison.OrdinalIgnoreCase));
            }
            else // If the user is a librarian or admin, and an ID is specified, attempt to show that user's details.
            {
                user = db.Users.Find(id.Value);
            }

            if (user == null)
            {
                return HttpNotFound();
            }

            // Additional check to prevent non-privileged users from seeing other users' details
            if (!User.IsInRole("Librarian") && !User.IsInRole("Admin") && User.Identity.Name != user.Email)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            return View(user);
        }


        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,Name,DOB,Email,PhoneNumber,Address,PasswordHash")] User user)
        {
            if (ModelState.IsValid)
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }
        // GET: Users/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = db.Users.Find(id);
            if (user == null || (!User.IsInRole("Admin") && User.Identity.Name != user.Email))
            {
                return HttpNotFound();
            }

            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "UserId,Name,DOB,Email,PhoneNumber,Address,PasswordHash")] User user)
        {
            if (ModelState.IsValid)
            {
                var userInDb = db.Users.Find(user.UserId);
                if (userInDb != null && (User.IsInRole("Admin") || User.Identity.Name == userInDb.Email))
                {
                    // Update the properties you want to allow the user to change
                    userInDb.Name = user.Name;
                    userInDb.DOB = user.DOB;
                    userInDb.PhoneNumber = user.PhoneNumber;
                    userInDb.Address = user.Address;
                    // Do not update the password here as it's not included in the form

                    db.Entry(userInDb).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index"); // Or redirect to 'Details'
                }
            }

            return View(user);
        }
        [Authorize(Roles="Librarian,Manager")]
        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }
        [Authorize(Roles="Librarian,Manager")]
        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
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
