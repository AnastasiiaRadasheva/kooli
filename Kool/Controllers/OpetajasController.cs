using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Kool.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Kool.Controllers
{
    public class OpetajasController : Controller
    {
        private UserManager<ApplicationUser> userManager;
        private RoleManager<IdentityRole> roleManager;

        public OpetajasController()
        {
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
        }

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Opetajas  (ВСЕМ можно смотреть)
        public ActionResult Index()
        {
            return View(db.Opetajad.ToList());
        }

        // GET: Opetajas/Details/5  (ВСЕМ можно смотреть)
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var opetaja = db.Opetajad.Find(id);
            if (opetaja == null) return HttpNotFound();

            return View(opetaja);
        }

        // GET: Opetajas/Create  (ТОЛЬКО Admin)
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Opetajas/Create  (ТОЛЬКО Admin)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "Id,Nimi,Kvalifikatsioon,FotoPath")] Opetaja opetaja)
        {
            if (ModelState.IsValid)
            {
                db.Opetajad.Add(opetaja); // ApplicationUserId будет null
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(opetaja);
        }


        // GET: Opetajas/Edit/5  (ТОЛЬКО Admin)
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var opetaja = db.Opetajad.Find(id);
            if (opetaja == null) return HttpNotFound();

            return View(opetaja);
        }

        // POST: Opetajas/Edit/5  (ТОЛЬКО Admin)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "Id,Nimi,Kvalifikatsioon,FotoPath,ApplicationUserId")] Opetaja opetaja)
        {
            if (ModelState.IsValid)
            {
                db.Entry(opetaja).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(opetaja);
        }

        // GET: Opetajas/Delete/5  (ТОЛЬКО Admin)
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var opetaja = db.Opetajad.Find(id);
            if (opetaja == null) return HttpNotFound();

            return View(opetaja);
        }

        // POST: Opetajas/Delete/5  (ТОЛЬКО Admin)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            var opetaja = db.Opetajad.Find(id);
            if (opetaja == null) return HttpNotFound();

            db.Opetajad.Remove(opetaja);
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
