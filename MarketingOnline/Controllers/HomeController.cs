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
using System.Drawing;
using System.Security.Cryptography;

namespace MarketingOnline.Controllers
{
    public class HomeController : Controller
    {
        private marketingolEntities db = new marketingolEntities();
        public ActionResult Index()
        {
            var n=(from q in db.news select q).OrderByDescending(o=>o.id).Take(4).ToList();
            ViewBag.news = n;
            return View();
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
        [HttpPost]
        public string CheckLogin(string name, string pass)
        {
            MD5 md5Hash = MD5.Create();
            pass = Config.GetMd5Hash(md5Hash, pass);
            var p = (from q in db.users where q.name.Contains(name) && q.pass.Contains(pass) select q.name).SingleOrDefault();
            if (p != null && p != "")
            {
                //Ghi ra cookie
                Config.setCookie("logged", "logged");
                return "1";
            }
            else
            {
                return "0";
            }
            return "0";
        }
        public ActionResult Login()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }
        public ActionResult ThietKeWeb()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }
    }
}