using System.Linq;
using System.Web.Mvc;
using LDevelopment.Models;

namespace LDevelopment.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TagsController : BaseController
    {
        // GET: Tags
        public ActionResult Index()
        {
            var tags = Repository.All<Tag>().ToList();

            return View(tags);
        }

        // GET: Tags/Details/5
        public ActionResult Details(int id)
        {
            var tag = Repository.Find<Tag>(id);

            return View(tag);
        }

        // GET: Tags/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Tags/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title")] Tag tag)
        {
            if (ModelState.IsValid)
            {
                Repository.Add(tag);
                Repository.Save();

                return RedirectToAction("Index");
            }

            return View(tag);
        }

        // GET: Tags/Edit/5
        public ActionResult Edit(int id)
        {
            var tag = Repository.Find<Tag>(id);

            return View(tag);
        }

        // POST: Tags/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title")] Tag tag)
        {
            if (ModelState.IsValid)
            {
                Repository.Update(tag);
                Repository.Save();

                return RedirectToAction("Index");
            }

            return View(tag);
        }

        // GET: Tags/Delete/5
        public ActionResult Delete(int id)
        {
            var tag = Repository.Find<Tag>(id);

            return View(tag);
        }

        // POST: Tags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Repository.Delete<Tag>(id);
            Repository.Save();

            return RedirectToAction("Index");
        }
    }
}
