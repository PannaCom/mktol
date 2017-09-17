using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MarketingOnline.Models;
using PagedList;
namespace MarketingOnline.Controllers
{
    public class companyController : Controller
    {
        // GET: company
        private marketingolEntities db = new marketingolEntities();
        public ActionResult Index(string pro, string dis, string str, int? page)
        {
            if (pro == null || pro == "all") pro = "";
            if (dis == null || dis == "all") dis = "";
            if (str == null || str == "all") str = "";
            var p = (from q in db.companies where q.province.Contains(pro) || q.district.Contains(dis) || q.street.Contains(str) select q).OrderBy(o => o.id).Take(1000);
            int pageSize = Config.PageSize;
            int pageNumber = (page ?? 1);
            ViewBag.page = page;
            if (pro == "all")
            {
                ViewBag.title = "Thông tin công ty, thông tin doanh nghiệp Việt Nam, trang " + pageNumber;
                ViewBag.des = "Xem, tra cứu doanh nghiệp tại Việt Nam trang " + pageNumber;
            }
            else { 
                ViewBag.title = "Thông tin công ty, thông tin doanh nghiệp " + pro+","+dis+","+str+", trang "+pageNumber;
                ViewBag.des = "Xem, tra cứu doanh nghiệp tại " + pro + "," + dis + "," + str + ", trang " + pageNumber;
            }
            //ViewBag.image = Config.domain + db.;
            if (pro == null || pro == "") pro = "all";
            if (dis == null || dis == "") dis = "all";
            if (str == null || str == "") str = "all";
            ViewBag.url = Config.domain + "/cong-ty/" + pro + "/" + dis + "/" + str + "/" + +pageNumber;
            ViewBag.pro = pro;
            ViewBag.dis = dis;
            ViewBag.str = str;
            //ViewBag.keywords = "digital marketing, content marketing, marketing online, thiết kế website, lập trình web, tạo website, lam web, web giá rẻ, quảng bá website, viết bài cho website ";
            return View(p.ToPagedList(pageNumber, pageSize));
        }
    }
}