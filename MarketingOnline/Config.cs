using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace MarketingOnline
{
    public class Config
    {
        public static bool isCrawl = false;
        public static bool isCrawlCompany = false;
        public static int PageSize = 5;
        public static string NewsImagePath = "/Images/News";
        public static int imgWidthNews = 205;
        public static int imgHeightNews = 190;
        public static string domain = "http://marketingol.net";
        public static string convertToDateTimeId(string d)
        {
            DateTime d1;
            try
            {
                d1 = DateTime.Parse(d);//ToUniversalTime();
                return d1.Year.ToString() + d1.Month.ToString("00") + d1.Day.ToString("00");
            }
            catch (Exception ex)
            {
                //d1 = DateTime.Now;
                //return d1.Year.ToString() + d1.Month.ToString("00") + d1.Day.ToString("00");
                return "0";
            }
            //d1 = DateTime.Now;
            //return d1.Year.ToString() + d1.Month.ToString("00") + d1.Day.ToString("00");
        }
        public static int datetimeid()
        {
            DateTime d1;
            try
            {

                d1 = DateTime.Now;//.ToUniversalTime();
                string rs = d1.Year.ToString() + d1.Month.ToString("00") + d1.Day.ToString("00");
                return int.Parse(rs);

            }
            catch (Exception ex)
            {
                d1 = DateTime.Now;//.ToUniversalTime();
                string rs = d1.Year.ToString() + d1.Month.ToString("00") + d1.Day.ToString("00");
                return int.Parse(rs);
            }
        }
        public static void setCookie(string field, string value)
        {
            HttpCookie MyCookie = new HttpCookie(field);
            MyCookie.Value = value;
            MyCookie.Expires = DateTime.Now.AddDays(5);
            HttpContext.Current.Response.Cookies.Add(MyCookie);
            //Response.Cookies.Add(MyCookie);           
        }
        public static string getCookie(string v)
        {
            try
            {
                return HttpContext.Current.Request.Cookies[v].Value.ToString();
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static string genCode()
        {
            Random rnd = new Random();
            string[] temp = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            string[] temp2 = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            int a = rnd.Next(0, 26); // creates a number between 1 and 25
            int b = rnd.Next(0, 10); // creates a number between 1 and 9
            int c = rnd.Next(0, 26);
            int d = rnd.Next(0, 10);
            int e = rnd.Next(0, 26);
            string rs = temp[a] + temp2[b] + temp[c] + temp2[d] + temp[e];
            return rs;
        }
        public static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash. 
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string. 
            return sBuilder.ToString();
        }
        //convert tieng viet thanh khong dau va them dau -
        public static string unicodeToNoMark(string input)
        {
            input = input.ToLowerInvariant().Trim();
            if (input == null) return "";
            string noMark = "a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,e,e,e,e,e,e,e,e,e,e,e,e,u,u,u,u,u,u,u,u,u,u,u,u,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,i,i,i,i,i,i,y,y,y,y,y,y,d,A,A,E,U,O,O,D";
            string unicode = "a,á,à,ả,ã,ạ,â,ấ,ầ,ẩ,ẫ,ậ,ă,ắ,ằ,ẳ,ẵ,ặ,e,é,è,ẻ,ẽ,ẹ,ê,ế,ề,ể,ễ,ệ,u,ú,ù,ủ,ũ,ụ,ư,ứ,ừ,ử,ữ,ự,o,ó,ò,ỏ,õ,ọ,ơ,ớ,ờ,ở,ỡ,ợ,ô,ố,ồ,ổ,ỗ,ộ,i,í,ì,ỉ,ĩ,ị,y,ý,ỳ,ỷ,ỹ,ỵ,đ,Â,Ă,Ê,Ư,Ơ,Ô,Đ";
            string[] a_n = noMark.Split(',');
            string[] a_u = unicode.Split(',');
            for (int i = 0; i < a_n.Length; i++)
            {
                input = input.Replace(a_u[i], a_n[i]);
            }
            input = input.Replace("  ", " ");
            input = Regex.Replace(input, "[^a-zA-Z0-9% ._]", string.Empty);
            input = removeSpecialChar(input);
            input = input.Replace(" ", "-");
            input = input.Replace("--", "-");
            return input;
        }
        public static string removeSpecialChar(string input)
        {
            input = input.Replace("-", "").Replace(":", "").Replace(",", "").Replace("_", "").Replace("'", "").Replace("\"", "").Replace(";", "").Replace("”", "").Replace(".", "").Replace("%", "");
            return input;
        }
        public static string getDiffTimeMinuteFromTwoDate(DateTime date1, DateTime date2)
        {
            try
            {
                DateTime d1 = date1;
                DateTime d2 = date2;
                TimeSpan TS = new System.TimeSpan(d2.Ticks - d1.Ticks);
                int totalHours = (int)Math.Abs(TS.TotalSeconds);
                if (totalHours < 0)
                {
                    return "1 phút trước";
                    //d2 = d2.ToLocalTime();
                    //return d2.Day + "/" + d2.Month + "/" + d2.Year;
                }
                else
                {
                    if (totalHours >= 3600)
                    {
                        int days = totalHours / 3600;
                        return days + " giờ trước";
                    }
                    else return totalHours.ToString() + " giây trước";
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static void logFile(string val, string filename)
        {
            string path = "D:\\Marketingol.net\\";
            StreamWriter sw = new StreamWriter(path + filename);
            sw.WriteLine(val);
            sw.Close();
        }
        public static DateTime? toDateTime(string d)
        {
            //if (d.Contains("GMT")) d = d.Replace("GMT", "");
            DateTime? d1;
            try
            {
                d1 = DateTime.Parse(d);
            }
            catch (Exception ex)
            {
                return null;
            }
            return d1;
        }
        public static bool Sendmail(string from, string pass, string to, string topic, string content)
        {
            try
            {
                var fromAddress = from;
                var toAddress = to;
                MailAddress mcopy = new MailAddress("marketingol.net@gmail.com");
                //Password of your gmail address
                string fromPassword = pass;
                // Passing the values and make a email formate to display
                string subject = topic;
                string body = content;
                // smtp settings
                MailMessage message = new MailMessage();
                message.From = new MailAddress(fromAddress);
                message.To.Add(toAddress);
                message.Bcc.Add(mcopy);
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = body;
                var smtp = new System.Net.Mail.SmtpClient();
                {
                    smtp.Host = "smtp.gmail.com";//"smtp.gmail.com";
                    smtp.Port = 587;// 465;//587;
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    smtp.Credentials = new NetworkCredential(fromAddress, fromPassword);
                    smtp.Timeout = 20000;
                }
                // Passing values to smtp object
                smtp.Send(message);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}