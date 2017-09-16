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
using Boilerpipe;
using Boilerpipe.Net.Extractors;
using HtmlAgilityPack;
using ReadSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Text.RegularExpressions;
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
        public string[] url = new string[] { "http://vneconomy.vn/rss/doanh-nhan.rss", "http://vneconomy.vn/rss/chung-khoan.rss", "http://vneconomy.vn/rss/bat-dong-san.rss", "http://vneconomy.vn/rss/xe-360.rss", "http://cafef.vn/trang-chu.rss", "http://cafef.vn/thi-truong-chung-khoan.rss", "http://cafef.vn/doanh-nghiep.rss", "http://cafef.vn/tai-chinh-ngan-hang.rss"};
        
        public async Task<string> crawl()
        {
            if (Config.isCrawl) return "Crawling..";
            Config.isCrawl = true;
            DateTime startTime = DateTime.Now;
            DateTime endTime = DateTime.Now;
            StringBuilder logF = new StringBuilder();

            try
            {

                for (int i = 0; i < url.Length; i++)
                {
                    try
                    {
                        XmlDocument RSSXml = new XmlDocument();

                        try
                        {

                            RSSXml.Load(url[i]);
                            logF.Append("Bắt đầu lấy tin " + url[i] + "\r\n");
                        }
                        catch (Exception ex)
                        {
                            logF.Append("Bắt đầu lấy tin " + url[i] + ", gặp lỗi " + ex.ToString() + "\r\n");
                        }

                        XmlNodeList RSSNodeList = RSSXml.SelectNodes("rss/channel/item");
                        XmlNode RSSDesc = RSSXml.SelectSingleNode("rss/channel/title");

                        //StringBuilder sb = new StringBuilder();
                        try
                        {
                            foreach (XmlNode RSSNode in RSSNodeList)
                            {

                                try
                                {
                                    XmlNode RSSSubNode;
                                    RSSSubNode = RSSNode.SelectSingleNode("title");
                                    string title = RSSSubNode != null ? RSSSubNode.InnerText : "";

                                    RSSSubNode = RSSNode.SelectSingleNode("link");
                                    string link = RSSSubNode != null ? RSSSubNode.InnerText : "";
                                    if (link.Contains("http://news.google.com"))
                                    {
                                        link = link.Substring(link.IndexOf("&url=") + 5);
                                    }
                                    if (link.Contains("bbc.com") || link.Contains("rfa.org") || link.Contains("vtimes.com.au") || link.Contains("kenh14.vn") || link.Contains("voatiengviet")) continue;//link.Contains("kenh14.vn") || link.Contains("tuoitre.vn")
                                    RSSSubNode = RSSNode.SelectSingleNode("description");
                                    string desc = RSSSubNode != null ? RSSSubNode.InnerText : "";

                                    RSSSubNode = RSSNode.SelectSingleNode("pubDate");

                                    string date = RSSSubNode != null ? RSSSubNode.InnerText : "";
                                    // Kiểm tra nếu ngày gửi quá lâu thì không lấy
                                    string image = "";
                                    try
                                    {
                                        if (link.Contains("http://news.google.com"))
                                        {
                                            image = getImageSrc(desc);
                                        }
                                        else
                                        {
                                            RSSSubNode = RSSNode.SelectSingleNode("media:thumbnail");
                                            image = RSSSubNode != null ? RSSSubNode.Attributes["url"].Value : "";
                                        }
                                    }
                                    catch (Exception img1)
                                    {

                                    }


                                    //try { 
                                    //    RSSSubNode = RSSNode.SelectSingleNode("media:thumbnail");
                                    //    image = RSSSubNode != null ? RSSSubNode.Attributes["url"].Value: "";
                                    //}catch(Exception img1){

                                    //}
                                    if (image == "")
                                    {
                                        try
                                        {
                                            RSSSubNode = RSSNode.SelectSingleNode("media:content");
                                            image = RSSSubNode != null ? RSSSubNode.Attributes["url"].Value : "";
                                        }
                                        catch (Exception img2)
                                        {

                                        }
                                    }

                                    if (title != null && !title.Equals(""))// && state1 != ""
                                    {
                                        link = link.Trim();
                                        title = title.Trim();
                                        int datetimeid = int.Parse(Config.convertToDateTimeId(date));
                                        if (datetimeid == 0) datetimeid = Config.datetimeid();
                                        if (datetimeid != 0)
                                        {
                                            DateTime? fdate = Config.toDateTime(date);
                                            if (fdate == null) fdate = DateTime.Now;
                                            //fdate = fdate.Value.AddHours(-12);
                                            var any = db.news.Any(o => o.date_id == datetimeid && o.title == title && o.link == link);
                                            if (!any)
                                            {
                                                //Uri urldomain = new Uri(link);
                                                //string pdf = Config.unicodeToNoMark(title) + ".pdf";
                                                //savePdf(link, pdf, urldomain.Host);
                                                string full_content = "";
                                                try
                                                {
                                                    Reader reader = new Reader();
                                                    Article result = await reader.Read(new Uri(link));
                                                    if (image == "")
                                                    {
                                                        try
                                                        {
                                                            image = result.FrontImage.ToString();
                                                        }
                                                        catch (Exception img1)
                                                        {
                                                            try
                                                            {
                                                                image = result.Images.ElementAt(0).Uri.ToString();
                                                            }
                                                            catch (Exception img2)
                                                            {
                                                                image = "";
                                                            }
                                                        }
                                                    }
                                                    full_content = result.Content;
                                                }
                                                catch (Exception ex222)
                                                {

                                                }
                                                if (full_content == "") { full_content = getAllContentByAi3(link); }
                                                try
                                                {
                                                    if (image == "" || link.Contains("soha.vn") || link.Contains("zing.vn") || link.Contains("tuoitre.vn"))
                                                    {
                                                        image = getImageFromAllContent(link);
                                                    }
                                                }
                                                catch (Exception images)
                                                {

                                                }
                                                if (!image.StartsWith("http") && !image.StartsWith("www")) continue;
                                                if (title == "" || title == null) continue;
                                                news n = new news();
                                                n.date_id = datetimeid;
                                                n.datetime = fdate;
                                                n.des = desc;
                                                //n.full_content = "";
                                                n.link = link;
                                                n.title = title;
                                                n.cat = "tin";
                                                n.fullcontent = full_content;
                                                n.image = image;
                                                db.news.Add(n);
                                                db.SaveChanges();
                                                if (image != "")
                                                {
                                                    try
                                                    {
                                                        string fileName = n.id.ToString() + ".jpg";
                                                        string file_name = Server.MapPath(@"\") + "\\images\\news\\" + fileName;
                                                        save_file_from_url(file_name, image);
                                                        image = "/images/news/" + fileName;
                                                        ImageProcessor.ImageFactory iFF = new ImageProcessor.ImageFactory();
                                                        iFF.Load(file_name).Quality(50).Save(file_name);
                                                        db.Database.ExecuteSqlCommand("update news set image=N'" + image + "' where id=" + n.id);
                                                    }
                                                    catch (Exception dlimage) { }
                                                }
                                            }//any
                                            else
                                            {
                                                //Đã tồn tại thì update
                                                //Nếu bài trong vòng 4 tiếng thì cập nhật
                                                TimeSpan span = DateTime.Now.Subtract((DateTime)fdate);
                                                if (span.Hours >= 4) continue;
                                                string full_content = "";
                                                try
                                                {
                                                    Reader reader = new Reader();
                                                    Article result = await reader.Read(new Uri(link));
                                                    full_content = result.Content;
                                                }
                                                catch (Exception ex2222)
                                                {

                                                }
                                                if (full_content == "") { full_content = getAllContentByAi3(link); }
                                                try
                                                {
                                                    db.Database.ExecuteSqlCommand("update news set fullcontent=N'" + HttpUtility.HtmlEncode(full_content) + "' where date_id=" + datetimeid + " and title=N'" + title + "' and link=N'" + link + "'");
                                                }
                                                catch
                                                {

                                                }

                                            }
                                        }

                                    }
                                    else continue;
                                }
                                catch (Exception exInFor1)
                                {
                                    //int abc = 0;
                                    //Array.Resize(ref arrItem, Length);
                                    logF.Append("Lỗi vòng for , gặp lỗi " + exInFor1.ToString() + "\r\n");
                                }
                            }//for node

                        }
                        catch (Exception exTryFor2)
                        {
                            //int abc = 0;
                            logF.Append("Bắt đầu lấy tin " + url[i] + ", gặp lỗi " + exTryFor2.ToString() + "\r\n");
                        }
                    }
                    catch
                    {

                    }
                } //for       

            }
            catch (Exception all)
            {
                logF.Append("Lỗi All " + all.ToString() + "\r\n");
            }
            endTime = DateTime.Now;
            string howLong = Config.getDiffTimeMinuteFromTwoDate(startTime, endTime) + " Done All \r\n";
            howLong += "Start at: " + startTime.ToString() + ".\r\n";
            howLong += "End at: " + DateTime.Now.ToString() + ".\r\n";
            StreamWriter SW = new StreamWriter(HttpRuntime.AppDomainAppPath + "HowLong.txt");
            SW.WriteLine(howLong);
            SW.Close();
            Config.logFile(logF.ToString(), "logAll.txt");
            Config.isCrawl = false;
            return "Done";
        }
        public string getImageFromAllContent(string link)
        {
            try
            {
                if (link.Contains("soha.vn") || link.Contains("zing.vn") || link.Contains("tuoitre.vn"))
                {
                    return getImageFromFacebookTag(link);
                }
                String page = String.Empty;
                WebRequest request = WebRequest.Create(link);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
                page = streamReader.ReadToEnd();
                string pattern = @"<(img)\b[^>]*>";
                Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                MatchCollection matches = rgx.Matches(page);
                return getImageSrc(matches[0].Value);

            }
            catch (Exception all)
            {
                return "";
            }
        }
        public string getImageFromFacebookTag(string link)
        {
            HttpWebRequest request = HttpWebRequest.Create(link) as HttpWebRequest;
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            var ResponseStream = response.GetResponseStream();

            HtmlDocument document = new HtmlDocument();
            document.Load(ResponseStream);
            string imgSrc = string.Empty;
            var ogMeta = document.DocumentNode.SelectNodes("//meta[@property]");
            //Check if contain Open graph element 
            if (ogMeta != null)
            {
                var ogImage = document.DocumentNode.SelectNodes("//meta[@property]").Where(x => x.Attributes["property"].Value == "og:image");
                if (ogImage.Count() > 0) //check og:image found 
                    return ogImage.FirstOrDefault().Attributes["content"].Value;
                else  //return some images 
                    return GetImages(document.DocumentNode.SelectNodes("//img"));
            }
            else
            {

                return GetImages(document.DocumentNode.SelectNodes("//img"));
            }
            return "";//Images.ToString();
        }
        public string GetImages(HtmlNodeCollection DOM)
        {
            StringBuilder Images = new StringBuilder();
            if (DOM != null)
            {
                foreach (var img in DOM)
                {

                    return getImageSrc(img.OuterHtml);
                    //Images.AppendFormat("<li>");
                    //Images.AppendFormat(img.OuterHtml);
                    //Images.AppendFormat("</li>");

                }
            }
            return Images.ToString();
        }
        public static void save_file_from_url(string file_name, string url)
        {
            byte[] content;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            WebResponse response = request.GetResponse();

            Stream stream = response.GetResponseStream();

            using (BinaryReader br = new BinaryReader(stream))
            {
                content = br.ReadBytes(500000);
                br.Close();
            }
            response.Close();

            FileStream fs = new FileStream(file_name, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            try
            {
                bw.Write(content);
            }
            finally
            {
                fs.Close();
                bw.Close();
            }

        }
        public string getImageSrc(string content)
        {
            string matchString = Regex.Match(content, "<img.*?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase).Groups[1].Value;

            return matchString;
        }
        public string getAllContentByAi3(string link)
        {
            try
            {
                String url = link;
                string allImage = "";
                String page = String.Empty;
                WebRequest request = WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
                page = streamReader.ReadToEnd();
                string pattern = @"<(img)\b[^>]*>";
                Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                MatchCollection matches = rgx.Matches(page);

                for (int i = 0, l = matches.Count; i < l; i++)
                {

                    allImage += matches[i].Value;
                    if (i >= 3) break;
                }
                string text = CommonExtractors.ArticleExtractor.GetText(page);
                return text + allImage;
            }
            catch
            {
                return "";
            }
        }
    }
}