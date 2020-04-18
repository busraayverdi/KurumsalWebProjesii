using KurumsalWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KurumsalWeb.Models.Model;
using KurumsalWeb.Models.DataContext;
using System.Web.Helpers;

namespace KurumsalWeb.Controllers
{
    public class AdminController : Controller
    {
        KurumsalDBContext db = new KurumsalDBContext();
        // GET: Admin
        [Route("yonetimpaneli")]
        public ActionResult Index()
        {
            ViewBag.BlogSay = db.Blog.Count();
            ViewBag.KategoriSay = db.Kategori.Count();
            ViewBag.HizmetSay = db.Hizmet.Count();
            ViewBag.YorumSay = db.Yorum.Count();
            ViewBag.YorumOnay = db.Yorum.Where(x => x.Onay == false ).Count();
            var sorgu = db.Kategori.ToList();
            return View(sorgu);
        }
        [Route("yonetimpaneli/giris")]
        public ActionResult Login()
        {
            return View();
        }
      
        [HttpPost]
       
        public ActionResult Login(Admin admin)
        {//admin
            
            var login = db.Admin.Where(x=>x.EPosta==admin.EPosta).SingleOrDefault();
            if (login.EPosta == admin.EPosta && login.Sifre == Crypto.Hash(admin.Sifre, "MD5"))
            {
                Session["adminid"] = login.AdminId;
                Session["eposta"] = login.EPosta;
                Session["yetki"] = login.Yetki;
                return RedirectToAction("Index", "Admin");
              
            }
            ViewBag.Uyari = "Kullanıcı adı yada sifre yanlış";
            return View(admin);
        
        }
        public ActionResult Logout()
        {
            Session["adminid"] = null;
            Session["eposta"] = null;
            Session.Abandon();
            return RedirectToAction("Login","Admin");
            
        }
        public ActionResult SifremiUnuttum()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SifremiUnuttum(string eposta)
        {
            var mail=db.Admin.Where(x=>x.EPosta==eposta).SingleOrDefault();
            if ( mail!=null)
            {
                Random rnd = new Random();
                int yeniSifre = rnd.Next();
                Admin admin = new Admin();
                mail.Sifre = Crypto.Hash(Convert.ToString(yeniSifre),"MD5");
                db.SaveChanges();

                WebMail.SmtpServer = "smtp.gmail.com";
                WebMail.EnableSsl = true;
                WebMail.UserName = "busraayverdi.995@gmail.com";
                WebMail.Password = "Busra";
                WebMail.SmtpPort = 587;

                WebMail.Send(eposta,"Admin Panel Giriş Şifreniz", "Şifreniz:"+yeniSifre);
                ViewBag.Uyari = "Şifreniz başarı ile gönderilmiştir.";
            }
            else
            {
                ViewBag.Uyari = "Hata oluştu tekrar deneyiniz.";
            }
            return View();
           
        }
        public ActionResult Adminler()
        {
            return View(db.Admin.ToList());
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Admin admin,string sifre, string eposta)
        {

            if (ModelState.IsValid)
            {
                admin.Sifre = Crypto.Hash(sifre, "MD5");
                db.Admin.Add(admin);
                db.SaveChanges();
                return RedirectToAction("Adminler");
            }
            return View(admin);
        }
        public ActionResult Edit(int id)
        {
            var a = db.Admin.Where(x => x.AdminId == id).SingleOrDefault();
            return View(a);
        }
        [HttpPost]
        public ActionResult Edit(int id,Admin admin,string sifre,string eposta)
        {
           
            if (ModelState.IsValid)
            {
               var a = db.Admin.Where(x => x.AdminId == id).SingleOrDefault();
                a.Sifre = Crypto.Hash(sifre,"MD5");
                a.EPosta = admin.EPosta;
                a.Yetki = admin.Yetki;
                db.SaveChanges();
                return RedirectToAction("Adminler");
            
            }
            
            return View();
        }
        public ActionResult Delete(int id)
        {
            var a = db.Admin.Where(x => x.AdminId == id).SingleOrDefault();
            if (a!=null)
            {
                db.Admin.Remove(a);
                db.SaveChanges();
                return RedirectToAction("Adminler");
            }
            return View(a);
        }
    }
}