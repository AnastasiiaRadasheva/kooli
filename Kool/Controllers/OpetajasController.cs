using Kool.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Kool.Controllers
{
    public class OpetajasController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public OpetajasController()
        {
            db = new ApplicationDbContext();
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
        }

        public ActionResult Index()
        {
            return View(db.Opetajad.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var opetaja = db.Opetajad.Find(id);
            if (opetaja == null) return HttpNotFound();
            return View(opetaja);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View(new OpetajaCreateVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(OpetajaCreateVM vm, HttpPostedFileBase foto)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var user = new ApplicationUser
            {
                UserName = vm.Email,
                Email = vm.Email
            };

            var result = userManager.Create(user, vm.Password);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError("", err);
                return View(vm);
            }

            if (!roleManager.RoleExists("Opetaja"))
                roleManager.Create(new IdentityRole("Opetaja"));

            userManager.AddToRole(user.Id, "Opetaja");

            string fileName = "default.png";
            if (foto != null && foto.ContentLength > 0)
            {
                var ext = Path.GetExtension(foto.FileName).ToLower();
                var allowed = new[] { ".jpg", ".jpeg", ".png" };

                if (!allowed.Contains(ext))
                {
                    ModelState.AddModelError("", "Ainult JPG ja PNG.");
                    return View(vm);
                }

                fileName = Guid.NewGuid() + ext;
                var path = Server.MapPath("~/Content/uploads/opetajad/");
                Directory.CreateDirectory(path);
                foto.SaveAs(Path.Combine(path, fileName));
            }

            var opetaja = new Opetaja
            {
                Nimi = vm.Nimi,
                Kvalifikatsioon = vm.Kvalifikatsioon,
                FotoPath = fileName,
                ApplicationUserId = user.Id
            };

            db.Opetajad.Add(opetaja);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var opetaja = db.Opetajad.Find(id);
            if (opetaja == null) return HttpNotFound();

            var user = userManager.FindById(opetaja.ApplicationUserId);
            if (user == null) return HttpNotFound();

            return View(new OpetajaEditVM
            {
                Id = opetaja.Id,
                Nimi = opetaja.Nimi,
                Kvalifikatsioon = opetaja.Kvalifikatsioon,
                FotoPath = opetaja.FotoPath,
                Email = user.Email,
                ApplicationUserId = opetaja.ApplicationUserId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(OpetajaEditVM vm, HttpPostedFileBase foto)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var opetaja = db.Opetajad.Find(vm.Id);
            var user = userManager.FindById(vm.ApplicationUserId);

            if (opetaja == null || user == null)
                return HttpNotFound();

            opetaja.Nimi = vm.Nimi;
            opetaja.Kvalifikatsioon = vm.Kvalifikatsioon;

            if (foto != null && foto.ContentLength > 0)
            {
                var ext = Path.GetExtension(foto.FileName).ToLower();
                var allowed = new[] { ".jpg", ".jpeg", ".png" };

                if (!allowed.Contains(ext))
                {
                    ModelState.AddModelError("", "Ainult JPG ja PNG.");
                    return View(vm);
                }

                var folder = Server.MapPath("~/Content/uploads/opetajad/");
                Directory.CreateDirectory(folder);

                if (!string.IsNullOrEmpty(opetaja.FotoPath) && opetaja.FotoPath != "default.png")
                {
                    var oldPath = Path.Combine(folder, opetaja.FotoPath);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                var newFileName = Guid.NewGuid() + ext;
                var fullPath = Path.Combine(folder, newFileName);

                foto.SaveAs(fullPath);
                opetaja.FotoPath = newFileName;
            }

            if (user.Email != vm.Email)
            {
                user.Email = vm.Email;
                user.UserName = vm.Email;

                var updateResult = userManager.Update(user);
                if (!updateResult.Succeeded)
                {
                    foreach (var err in updateResult.Errors)
                        ModelState.AddModelError("", err);

                    return View(vm);
                }
            }

            if (!string.IsNullOrWhiteSpace(vm.NewPassword))
            {
                if (!string.IsNullOrWhiteSpace(vm.NewPassword))
                {
                    user.PasswordHash = userManager.PasswordHasher.HashPassword(vm.NewPassword);
                    userManager.Update(user);
                }
    
            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var opetaja = db.Opetajad.Find(id);
            if (opetaja == null) return HttpNotFound();
            return View(opetaja);
        }

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