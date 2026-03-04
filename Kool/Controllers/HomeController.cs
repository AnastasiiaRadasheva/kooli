using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Kool.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult ContactForm()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult SendEmail(string email, string subject, string message)
        {
            try
            {
                WebMail.SmtpServer = "smtp.gmail.com";
                WebMail.SmtpPort = 587;
                WebMail.EnableSsl = true;
                WebMail.UserName = "eha20082@gmail.com";
                WebMail.Password = "iakc rgui tmxd erwf";

                string body = "Kasutaja email: " + email + "<br/><br/>" + message;

                WebMail.Send(
                    to: "yourmail@gmail.com",
                    subject: subject,
                    body: body
                );

                TempData["Msg"] = "Kiri on saadetud!";
            }
            catch (Exception ex)
            {
                TempData["Msg"] = "Viga: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}