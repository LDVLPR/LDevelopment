using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LDevelopment.Models;

namespace LDevelopment.Controllers
{
    public class TagsController : BaseController
    {
        // GET: Tags
        public ActionResult Index()
        {
            return View(db.Tags.ToList());
        }

        // GET: Tags/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var tagModel = db.Tags.Find(id);

            if (tagModel == null)
            {
                return HttpNotFound();
            }
            return View(tagModel);
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
        public ActionResult Create([Bind(Include = "Id,Title")] TagModel tagModel)
        {
            if (ModelState.IsValid)
            {
                db.Tags.Add(tagModel);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(tagModel);
        }

        // GET: Tags/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var tagModel = db.Tags.Find(id);

            if (tagModel == null)
            {
                return HttpNotFound();
            }
            return View(tagModel);
        }

        // POST: Tags/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title")] TagModel tagModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tagModel).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(tagModel);
        }

        // GET: Tags/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var tagModel = db.Tags.Find(id);

            if (tagModel == null)
            {
                return HttpNotFound();
            }

            return View(tagModel);
        }

        // POST: Tags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var tagModel = db.Tags.Find(id);

            db.Tags.Remove(tagModel);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
