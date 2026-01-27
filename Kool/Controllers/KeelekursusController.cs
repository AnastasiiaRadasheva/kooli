using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Kool.Models;

namespace Kool.Controllers
{
    public class KeelekursusController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Keelekursus  (ВСЕМ можно смотреть)
        public ActionResult Index()
        {
            return View(db.Keelekursused.ToList());
        }

        // GET: Keelekursus/Details/5  (ВСЕМ можно смотреть)
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var keelekursus = db.Keelekursused.Find(id);
            if (keelekursus == null) return HttpNotFound();

            return View(keelekursus);
        }

        // GET: Keelekursus/Create  (ТОЛЬКО Admin)
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Keelekursus/Create  (ТОЛЬКО Admin)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "Id,Nimetus,Keel,Tase,Kirjeldus")] Keelekursus keelekursus)
        {
            if (ModelState.IsValid)
            {
                db.Keelekursused.Add(keelekursus);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(keelekursus);
        }

        // GET: Keelekursus/Edit/5  (ТОЛЬКО Admin)
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var keelekursus = db.Keelekursused.Find(id);
            if (keelekursus == null) return HttpNotFound();

            return View(keelekursus);
        }

        // POST: Keelekursus/Edit/5  (ТОЛЬКО Admin)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "Id,Nimetus,Keel,Tase,Kirjeldus")] Keelekursus keelekursus)
        {
            if (ModelState.IsValid)
            {
                db.Entry(keelekursus).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(keelekursus);
        }

        // GET: Keelekursus/Delete/5  (ТОЛЬКО Admin)
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var keelekursus = db.Keelekursused.Find(id);
            if (keelekursus == null) return HttpNotFound();

            return View(keelekursus);
        }

        // POST: Keelekursus/Delete/5  (ТОЛЬКО Admin)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            var keelekursus = db.Keelekursused.Find(id);
            if (keelekursus == null) return HttpNotFound();

            db.Keelekursused.Remove(keelekursus);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
