using KurumsalWeb.Models.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.Web.UI;
using KurumsalWeb.Models.Model;
using System.Net.Mail;
using System.Net;

namespace KurumsalWeb.Controllers
{
    public class HomeController : Controller
    {
        private KurumsalDBContext db = new KurumsalDBContext();
        // GET: Default
        [Route("")]
        [Route("AnaSayfa")]
        
        public ActionResult Index()
        {

            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            ViewBag.Hizmetler = db.Hizmet.ToList().OrderByDescending(x=>x.HizmetId);
           
            return View();
        }
        public ActionResult SliderPartial()
        {

            return View(db.Slider.ToList().OrderByDescending(x=>x.SliderId));
        }
        public ActionResult HizmetPartial()
        {

            return View(db.Hizmet.ToList());
        }
        [Route("Hakkimizda")]
        public ActionResult Hakkimizda()
        {
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            return View(db.Hakkimizda.SingleOrDefault());
        }
        [Route("Hizmetlerimiz")]
        public ActionResult Hizmetlerimiz()
        {
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            return View(db.Hizmet.ToList().OrderByDescending(x=>x.HizmetId));
        }
        [Route("İletisim")]

        public ActionResult İletisim()
        {
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            return View(db.İletisim.SingleOrDefault());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult İletisim(string adsoyad=null,string email=null,string konu=null,string mesaj=null)
        {
           
            if (adsoyad != null && email != null)
            {
                WebMail.SmtpServer = "smtp.gmail.com";
                WebMail.EnableSsl = true;
                WebMail.UserName = "busraayverdi.995@gmail.com";
                WebMail.Password = "Busra";
                WebMail.SmtpPort = 587;

                WebMail.Send("busraayverdi.995@gmail.com", konu, email + "-" + mesaj + mesaj);
                ViewBag.Uyari = "Mesajınız başarı ile gönderilmiştir.";
                Response.Redirect("/iletisim");
            }
            else
            {
                ViewBag.Uyari = "Hata oluştu tekrar deneyiniz.";
            }
            return View();
        }

        [Route("BlogPost")]
        public ActionResult Blog(int Page=1)
        {
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            return View(db.Blog.Include("Kategori").OrderByDescending(x=>x.BlogId).ToPagedList(Page,5));
        }
        [Route("BlogPost/{baslik}-{id:int}")]
        public ActionResult BlogDetay(int id)
        {
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            var b = db.Blog.Include("Kategori").Include("Yorum").Where(x => x.BlogId == id).SingleOrDefault();
            return View(b);
        }
        [Route("BlogPost/{Kategoriad}/{id:int}")]
        public ActionResult KategoriBlog(int id, int Page = 1)
        {
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            var b = db.Blog.Include("Kategori").OrderByDescending(x=>x.BlogId).Where(x => x.Kategori.KategoriId == id).ToPagedList(Page,5);
            return View(b);
        }
        public JsonResult YorumYap(string adsoyad,string eposta,string Icerik,int blogid)
        {
            if (Icerik==null)
            {
                return Json(true,JsonRequestBehavior.AllowGet);
            }
            db.Yorum.Add(new Yorum { AdSoyad = adsoyad, EPosta = eposta, Icerik = Icerik, BlogId = blogid,Onay=false });
            db.SaveChanges();
            
            return Json(false,JsonRequestBehavior.AllowGet);
        }
      
        public ActionResult BlogKategoriPartial()
        {
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            return PartialView(db.Kategori.Include("Blogs").ToList().OrderBy(x => x.KategoriAd));
        }
        public ActionResult BlogKayitPartial()
        {
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            return PartialView(db.Blog.ToList().OrderByDescending(x => x.BlogId));
        }
        public ActionResult FooterPartial()
        {
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            ViewBag.Hizmetler = db.Hizmet.ToList().OrderByDescending(x => x.HizmetId);
            ViewBag.İletisim = db.İletisim.SingleOrDefault();
            ViewBag.Blog = db.Blog.ToList().OrderByDescending(x => x.BlogId);
            return PartialView();
        }
      
    }
}