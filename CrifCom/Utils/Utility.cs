using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Web;
using System.Configuration;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Web.Script.Serialization;
using System.Web.Configuration;
using System.Security.Cryptography;
using Umbraco.Core.Logging;

namespace Crifireland.Utils
{
    public static class Utility
    {
        public static string DatePlaceHolder { get { return "[DATE]"; } }
        public static string NamePlaceHolder { get { return "[NAME]"; } }
        public static string SurnamePlaceHolder { get { return "[SURNAME]"; } }
        public static string CompanyPlaceHolder { get { return "[COMPANY]"; } }
        public static string RolePlaceHolder { get { return "[ROLE]"; } }
        public static string EmailAddressPlaceHolder { get { return "[EMAILADDRESS]"; } }
        public static string TelephonePlaceHolder { get { return "[TELEPHONE]"; } }
        public static string MessagePlaceHolder { get { return "[MESSAGE]"; } }
        public static string PreferredDatePlaceHolder { get { return "[PREFFEREDDATE]"; } }
        public static string IPPlaceHolder { get { return "[IP]"; } }
        public static string UrlPlaceHolder { get { return "[URL]"; } }

        public static string FromEmail { get { return ConfigurationManager.AppSettings["SMTPFromEmail"]; } }
        public static string FromName { get { return ConfigurationManager.AppSettings["SMTPFromName"]; } }


        private const int keysize = 256;
        private static readonly byte[] initVectorBytes = Encoding.ASCII.GetBytes("tu89geji340t89u2");


        public static string GetVimeoId(string url)
        {
            string id = string.Empty;
            Regex VimeoVideoRegex = new Regex(@"vimeo\.com/(?:.*#|.*/videos/)?([0-9]+)");
            Match vimeoMatch = VimeoVideoRegex.Match(url);
            if (vimeoMatch.Success)
                id = vimeoMatch.Groups[1].Value;
            return id;
        }


        // Form submit helpers start
        public static string GetClientIP()
        {
            var request = HttpContext.Current.Request;

            string clientIp = string.Empty;
            string IP = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrWhiteSpace(IP) || IP.ToLower() == "unknown")
            {
                IP = request.ServerVariables["REMOTE_ADDR"];
            }

            if (string.IsNullOrWhiteSpace(IP))
            {
                string strHostName = request.UserHostAddress.ToString();
                IP = System.Net.Dns.GetHostAddresses(strHostName).GetValue(0).ToString();
            }
            else
            {
                IP = IP.Split(',').First().Trim();
            }
            return IP;
        }
        public static string GetUserAgent()
        {
            var request = HttpContext.Current.Request;
            return request.UserAgent;
        }

        public static string GetBrowser()
        {
            var request = HttpContext.Current.Request;
            return string.Format("{0} {1}", request.Browser.Browser, request.Browser.Version);
        }
        public static string GetDevice()
        {
            var request = HttpContext.Current.Request;

            return request.Browser.IsMobileDevice ? string.Format("Mobile {0} {1}", request.Browser.MobileDeviceManufacturer, request.Browser.MobileDeviceModel) : "PC";
        }
        public static string GetDevicePlatform()
        {
            var request = HttpContext.Current.Request;
            return request.Browser.Platform;
        }

        public static string GetFileHyperLink(string fileUrl)
        {
            if (!string.IsNullOrEmpty(fileUrl))
            {
                return string.Format("<a href=\"{0}\">{1}</a>", fileUrl, Path.GetFileNameWithoutExtension(fileUrl));
            }
            return "";
        }
        public static string GetHyperLink(string pageUrl, string pageName)
        {
            if (!string.IsNullOrEmpty(pageUrl))
            {
                if (string.IsNullOrWhiteSpace(pageName))
                {
                    pageName = pageUrl;
                }
                return string.Format("<a href=\"{0}\">{1}</a>", pageUrl, pageName);
            }
            return "";
        }
        public static string GetHyperLink(int pageId, UmbracoHelper umb)
        {
            var form = umb.TypedContent(pageId);

            string link = "";
            if (form != null)
            {
                link = Utility.GetHyperLink(form.UrlWithDomain(), form.Name);
            }
            return link;
        }

        public static string GetImageAsBackGround(IPublishedContent image)
        {
            string style = "";
            if (image != null)
            {
                string backgroundImageURL = image.Url;
                if (HttpContext.Current.Request.Browser.IsMobileDevice)
                {
                    backgroundImageURL = image.Url + string.Format("?width={0} & height={1}&mode=crop", 1100, 372);
                }
                style = string.Format("style=\"background-image : url('{0}')\"", backgroundImageURL);
            }
            return style;
        }

        public static DateTime? GetDateFromString(string dateString, string format = "M/d/yyyy hh:mm:ss tt")
        {
            DateTime dt;
            IFormatProvider culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            if (DateTime.TryParseExact(dateString, format, culture, System.Globalization.DateTimeStyles.AssumeLocal, out dt))
            {
                return dt;
            }

            return new DateTime?();
        }


        public static string convertEmailToAsciiCode(string emailId)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            Encoder encoder = encoding.GetEncoder();
            encoder.Fallback = new EncoderReplacementFallback(string.Empty);

            byte[] bAsciiString = encoding.GetBytes(emailId);
            string str = "";
            foreach(var item in bAsciiString)
            {
                str += "&#" + item + ";";

            }
            // or turn back into a "clean" string
            string hhh = HttpUtility.HtmlEncode(emailId);
            string cleanString = encoding.GetString(bAsciiString);

            return str;
        }

        public static string GetValidTelePhone(string phone)
        {
            if (!string.IsNullOrEmpty(phone))
            {
                return phone.Replace(" ", "");
            }
            return "";
        }
        // Form submit helpers end

        public static string Decrypt(string cipherText, string passPhrase)
        {
            try
            {
                if (string.IsNullOrEmpty(passPhrase))
                {
                    throw new ArgumentException("passPhrase is required");
                }
                cipherText = cipherText.Trim().Replace(" ", "+");

                if (cipherText.Length % 4 > 0)
                    cipherText = cipherText.PadRight(cipherText.Length + 4 - cipherText.Length % 4, '=');

                byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
                using (PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null))
                {
                    byte[] keyBytes = password.GetBytes(keysize / 8);
                    using (RijndaelManaged symmetricKey = new RijndaelManaged())
                    {
                        symmetricKey.Mode = CipherMode.CBC;
                        using (ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes))
                        {
                            using (MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
                            {
                                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                                {
                                    byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                                    int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                    return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "[RegistrationController][ResetPassword]- Invalid Id" + ex.Source, ex);
                throw;
            }
        }

        public static string ToBase64Url(this string str)
        {
            try
            {
                if (str == null)
                {
                    throw new ArgumentNullException("str");
                }

                var customBase64 = System.Web.HttpServerUtility.UrlTokenEncode(Encoding.UTF8.GetBytes(str));
                return customBase64.Length == 0 ? customBase64 : customBase64.Substring(0, customBase64.Length - 1);
            }
            catch (Exception ex)
            {               
                return string.Empty;
            }
        }

        public static string FromBase64Url(this string rfc4648)
        {
            try
            {
                if (rfc4648.Length % 4 != 0)
                {
                    rfc4648 += (4 - rfc4648.Length % 4);
                }
                else
                {
                    rfc4648 += 0;
                }

                return Encoding.UTF8.GetString(System.Web.HttpServerUtility.UrlTokenDecode(rfc4648));
            }
            catch (Exception ex)
            {               
                return string.Empty;
            }
        }
        public static string GetDomainUrl(int pageId, int defaultPort)
        {
            try
            {
                var domains = umbraco.library.GetCurrentDomains(pageId);
                string url = "";
                if (domains != null)
                {
                    foreach (var domain in domains)
                    {
                        if (domain.Name.Contains("https://"))
                        {
                            Uri uri = new Uri(domain.Name);
                            url = uri.Scheme + System.Uri.SchemeDelimiter + uri.Host + (uri.Port != defaultPort ? ":" + uri.Port : "");
                        }
                        else
                        {
                            Uri uri = new Uri(umbraco.library.NiceUrlWithDomain(pageId).ToString());
                            url = uri.Scheme + System.Uri.SchemeDelimiter + uri.Host + (uri.Port != defaultPort ? ":" + uri.Port : "");
                        }
                    }
                }
                else
                {
                    Uri uri = new Uri(umbraco.library.NiceUrlWithDomain(pageId).ToString());
                    url = uri.Scheme + System.Uri.SchemeDelimiter + uri.Host + (uri.Port != defaultPort ? ":" + uri.Port : "");
                }
                return url;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        /*google recaptcha*/
        public static bool isCaptchaValid(string response)
        {
            string responseData = response;
            bool valid = false;
            string Secretkey = WebConfigurationManager.AppSettings["SecretkeyForCaptcha"].ToString();
            var requestString = string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", Secretkey, responseData);
            try
            {
                HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(requestString);
                if (System.Configuration.ConfigurationManager.AppSettings["proxyServerAddress"].ToString() != "" && System.Configuration.ConfigurationManager.AppSettings["proxyServerPort"].ToString() != "")
                {
                    string strprxyaddr = System.Configuration.ConfigurationManager.AppSettings["proxyServerAddress"].ToString();
                    string strprxyport = System.Configuration.ConfigurationManager.AppSettings["proxyServerPort"].ToString();
                    WebProxy myproxy = new WebProxy("" + strprxyaddr + ":" + strprxyport + "", false);
                    if (System.Configuration.ConfigurationManager.AppSettings["proxyServerUserName"].ToString() != "" && System.Configuration.ConfigurationManager.AppSettings["proxyServerPassword"].ToString() != "")
                    {
                        string strprxyuser = System.Configuration.ConfigurationManager.AppSettings["proxyServerUserName"].ToString();
                        string strprxypass = System.Configuration.ConfigurationManager.AppSettings["proxyServerPassword"].ToString();
                        ICredentials credentials = new NetworkCredential(strprxyuser, strprxypass);
                        myproxy = new WebProxy("" + strprxyaddr + ":" + strprxyport + "", true, null, credentials);
                    }
                    request.Proxy = myproxy;
                }
                using (HttpWebResponse responseObj = (HttpWebResponse)request.GetResponse())
                {

                    using (StreamReader readStream = new StreamReader(responseObj.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();

                        JavaScriptSerializer js = new JavaScriptSerializer();
                        dynamic data = js.Deserialize<dynamic>(jsonResponse);

                        valid = data["success"];
                    }
                }

                return valid;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }
    }
}