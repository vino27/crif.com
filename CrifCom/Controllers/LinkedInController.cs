using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using CrifCom.Utils;
using Umbraco.Web.Mvc;
using umbraco.NodeFactory;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml.Serialization;
 
namespace CrifCom.Controllers
{
    public class LinkedInController : SurfaceController
    {
        static int id = 0;
        static string CallbackUrl = "";
        public ActionResult index()
        {
            return AuthenticateToLinkedIn();
        }


        static string token_secret = "";
        public ActionResult AuthenticateToLinkedIn()
        {
            Node currentpage = Node.GetCurrent();
            id = currentpage.Id;
            int defaultPort = Request.IsSecureConnection ? 443 : 80;
            string url = Crifireland.Utils.Utility.GetDomainUrl(id, defaultPort);
            var ConsumerKey = ConfigurationManager.AppSettings["ClientIDForLinkedInRegister"];
            if (currentpage.NodeTypeAlias == "contact")
            {
                CallbackUrl = url + "/ContactCallback";
            }
            else
            {
                CallbackUrl = url + "/RegistrationCallback";
            }

            Response.Redirect("https://www.linkedin.com/oauth/v2/authorization?response_type=code&client_id=" + ConsumerKey + "&redirect_uri=" + CallbackUrl + "&state=bD8wPu6KZS&scope=r_basicprofile%20r_emailaddress");
            return null;
        }

        string token = "";
        string verifier = "";
        public ActionResult Callback()
        {
            var linkedInApiKey = ConfigurationManager.AppSettings["ClientIDForLinkedInRegister"];
            var linkedInSecretKey = ConfigurationManager.AppSettings["ClientSecretForLinkedInRegister"];
            Uri redirectUri = new Uri(CallbackUrl);
            var authorizationCode = Request["code"];
            var accessCodeUri =
                string.Format(
                    "https://www.linkedin.com/oauth/v2/accessToken?grant_type=authorization_code&code={0}&redirect_uri={1}&client_id={2}&client_secret={3}",
                    authorizationCode,
                    redirectUri.AbsoluteUri,
                    linkedInApiKey,
                    linkedInSecretKey);

            WebRequest request =
                WebRequest.Create(accessCodeUri);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add("Access-Control-Allow-Origin", "*");
            Stream dataStream = request.GetRequestStream();

            string postData = string.Empty;
            byte[] postArray = Encoding.ASCII.GetBytes(postData);
            dataStream.Write(postArray, 0, postArray.Length);
            dataStream.Close();


            WebResponse response = request.GetResponse();
            dataStream = response.GetResponseStream();
            var reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            string returnResponse = Share(responseFromServer);

            var data = Deserialize<Person>(returnResponse);
            //TempData["Name"] = String.Empty;
            //TempData["SurName"] = String.Empty;
            //TempData["Email"] = String.Empty;
            //TempData["Company"] = String.Empty;
            //TempData["Role"] = String.Empty;
            //TempData["Country"] = String.Empty;
            if (!string.IsNullOrEmpty(data.FirstName))
            {
                TempData["Name"] = data.FirstName;
            }
            if (!string.IsNullOrEmpty(data.LastName))
            {
                TempData["SurName"] = data.LastName;
            }
            if (!string.IsNullOrEmpty(data.Email))
            {
                TempData["Email"] = data.Email;
            }
            if (data.Positions!=null && data.Positions.Count() > 0)
            {
                if (!string.IsNullOrEmpty(data.Positions[0].company.Name))
                {
                    TempData["Company"] = data.Positions[0].company.Name;
                }
                if (!string.IsNullOrEmpty(data.Positions[0].Title))
                {
                    TempData["Role"] = data.Positions[0].Title;
                } 
            }
            if (data.location!=null && !string.IsNullOrEmpty(data.location.Name))
            {
                TempData["Country"] = data.location.Name;
            }

            reader.Close();
            dataStream.Close();
            response.Close();
            return RedirectToUmbracoPage(Umbraco.TypedContent(id));
        }
        public string Share(string linkedInResponseFromServer)
        {
            var data = (JObject)JsonConvert.DeserializeObject(linkedInResponseFromServer);
            string token = data["access_token"].Value<string>();
            string requestUrl = "https://api.linkedin.com/v1/people/~:(first-name,last-name,email-address,industry,positions,location)?oauth2_access_token=" + token;
            WebRequest request = WebRequest.Create(requestUrl);

            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";

            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            var reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();

            reader.Close();
            dataStream.Close();
            response.Close();
            return responseFromServer;
        }
        public T Deserialize<T>(string xmlContent)
        {
            MemoryStream memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(xmlContent));
            XmlSerializer deserializer = new XmlSerializer(typeof(T));
            return (T)deserializer.Deserialize(new StringReader(xmlContent));
        }
    }
}