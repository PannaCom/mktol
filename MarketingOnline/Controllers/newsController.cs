using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MarketingOnline.Models;
using PagedList;
using ImageProcessor.Processors;
using ImageProcessor.Imaging;
using System.Drawing;
using System.Text.RegularExpressions;
namespace MarketingOnline.Controllers
{
    public class newsController : Controller
    {
        private marketingolEntities db = new marketingolEntities();

        // GET: news
        public ActionResult Index(int? page)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Home");
            var p = (from q in db.news select q).OrderByDescending(o => o.id).Take(1000);
            int pageSize = Config.PageSize;
            int pageNumber = (page ?? 1);
            ViewBag.page = page;
            return View(p.ToPagedList(pageNumber, pageSize));
        }

        // GET: news/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            news news = db.news.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }
        public ActionResult GetDetails(int id = 0)
        {
            news news = db.news.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            //ViewBag.menuleft = Config.getProjectMenu();
            ViewBag.datetime = "<span itemprop=\"datePublished\" style=\"color:green;\">" + news.datetime.Value + "</span>";
            ViewBag.content = news.fullcontent;
            if (news.des != null)
            {
                ViewBag.des = Regex.Replace(news.des, "<.*?>", string.Empty);
            }
            else
            {
                ViewBag.des = news.title;
            }
            ViewBag.image = Config.domain + news.image;
            ViewBag.url = Config.domain + "/" + Config.unicodeToNoMark(news.title) + "-" + id;
            ViewBag.title = news.title;
            ViewBag.keywords = news.keywords;
            ViewBag.newsother = null;
            try
            {
                var p=(from q in db.news where q.id!=id select q).OrderByDescending(o=>o.id).Take(5).ToList();
                ViewBag.newsother = p;
            }
            catch (Exception ex) { 

            }
            return View(news);
        }
        // GET: news/Create
        public ActionResult Create()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Home");
            return View();
        }

        // POST: news/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,title,image,des,fullcontent,datetime,keywords,cat")] news news)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Home");
            if (ModelState.IsValid)
            {
                news.datetime = DateTime.Now;
                db.news.Add(news);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(news);
        }
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public string UploadImageProcess(HttpPostedFileBase file, string filename)
        {
            string physicalPath = HttpContext.Server.MapPath("../" + Config.NewsImagePath + "\\");
            string nameFile = String.Format("{0}.jpg", filename + "-" + Config.genCode());
            int countFile = Request.Files.Count;
            string fullPath = physicalPath + System.IO.Path.GetFileName(nameFile);
            for (int i = 0; i < countFile; i++)
            {
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                Request.Files[i].SaveAs(fullPath);
                break;
            }
            string ok = resizeImage(Config.imgWidthNews, Config.imgHeightNews, physicalPath, nameFile);
            return Config.NewsImagePath + "/" + nameFile;
        }
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public string UploadImageProcessContent(HttpPostedFileBase file, string filename)
        {
            string physicalPath = HttpContext.Server.MapPath("../" + Config.NewsImagePath + "\\");
            string nameFile = String.Format("{0}.jpg", filename + "-" + Config.genCode());
            int countFile = Request.Files.Count;
            string fullPath = physicalPath + System.IO.Path.GetFileName(nameFile);
            string content = "";
            for (int i = 0; i < countFile; i++)
            {
                nameFile = String.Format("{0}.jpg", filename + "-" + Guid.NewGuid().ToString());
                fullPath = physicalPath + System.IO.Path.GetFileName(nameFile);
                content += "<img src=\"" + Config.NewsImagePath + "/" + nameFile + "\" width=200 height=126>";
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                Request.Files[i].SaveAs(fullPath);
                //break;
            }
            //string ok = resizeImage(Config.imgWidthNews, Config.imgHeightNews, physicalPath, nameFile);
            //return Config.NewsImagePath + "/" + nameFile;
            return content;
        }
       
        public string resizeImage(int maxWidth, int maxHeight, string fullPath, string path)
        {
            string physicalPath = fullPath;
            string nameFile = path;
            //return resizeImage(Config.imgWidthProduct, Config.imgHeightProduct, physicalPath + nameFile, Config.ProductImagePath + "/" + nameFile);
            ImageProcessor.ImageFactory iFF = new ImageProcessor.ImageFactory();
            ////Tạo ra file thumbail không có watermark
            Size size1 = new Size(Config.imgWidthNews, Config.imgHeightNews);
            iFF.Load(physicalPath + nameFile).Resize(size1).BackgroundColor(Color.WhiteSmoke).Save(physicalPath + nameFile);
            //iFF.Load(physicalPath + nameFile).BackgroundColor(Color.WhiteSmoke).Resize(size1).Save(physicalPath + nameFile);
            return "ok";
            
        }
        // GET: news/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            news news = db.news.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // POST: news/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,title,image,des,fullcontent,datetime,keywords,cat")] news news)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Home");
            if (ModelState.IsValid)
            {
                news.datetime = DateTime.Now;
                db.Entry(news).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(news);
        }

        // GET: news/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            news news = db.news.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // POST: news/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Home");
            news news = db.news.Find(id);
            db.news.Remove(news);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult List(string keyword,int? page)
        {
            //ViewBag.menuleft = Config.getProjectMenu();
            //Lấy ra các tin
            if (keyword == null || keyword=="all") keyword = "";
            var p = (from q in db.news where q.title.Contains(keyword) || q.keywords.Contains(keyword) || q.fullcontent.Contains(keyword) select q).OrderByDescending(o => o.id).Take(1000);
            int pageSize = Config.PageSize;
            int pageNumber = (page ?? 1);
            ViewBag.page = page;
            ViewBag.des = "Tin tức thiết kế web và marketing online " + pageNumber;
            //ViewBag.image = Config.domain + db.;
            ViewBag.url = Config.domain + "/tin/" + +pageNumber;
            ViewBag.keywords = "digital marketing, content marketing, marketing online, thiết kế website, lập trình web, tạo website, lam web, web giá rẻ, quảng bá website, viết bài cho website ";
            return View(p.ToPagedList(pageNumber, pageSize));
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
