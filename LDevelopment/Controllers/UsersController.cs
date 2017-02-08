using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using LDevelopment.Models;

namespace LDevelopment.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : BaseController
    {
        // GET: Users
        public ActionResult Index()
        {
            var users = Repository.Context.Users.ToList();

            return View(users);
        }

        // GET: Users/Details/5
        public ActionResult Details(string id)
        {
            var user = Repository.Context.Users.Find(id);

            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                Repository.Context.Users.Add(user);
                Repository.Save();

                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(string id)
        {
            var user = Repository.Context.Users.Find(id);

            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                Repository.Context.Entry(user).State = EntityState.Modified;
                Repository.Save();

                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(string id)
        {
            var user = Repository.Context.Users.Find(id);

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var user = Repository.Context.Users.Find(id);

            Repository.Context.Users.Remove(user);
            Repository.Save();

            return RedirectToAction("Index");
        }
    }
}
