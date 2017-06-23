using CrifCom.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using umbraco.NodeFactory;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Web.Mvc;
using Phases.UmbracoUtils;
using System.Configuration;
using Crifireland.Utils;
using Umbraco.Web;
using System.Web.Security;
using Umbraco.Web.Models;

namespace CrifCom.Controllers
{
    public class RegistrationController : SurfaceController
    {
        public ActionResult SubmitRegistration(Registration registration)
        {
            try
            {
                string response = Request["g-recaptcha-response"];
                if (ModelState.IsValid && registration.IsPrivacyChecked && !string.IsNullOrEmpty(response) && Crifireland.Utils.Utility.isCaptchaValid(response))
                {
                    Node currentpage = Node.GetCurrent();
                    Node registrationPage = new Node(int.Parse(umbraco.library.GetDictionaryItem("RegistrationNodeId")));
                    IMember newMember = ApplicationContext.Services.MemberService.GetByUsername(registration.Email);
                    var thankYouPage = "";
                    if (registrationPage.GetProperty("thankYouPage") != null)
                    {
                        var node = registrationPage.GetProperty("thankYouPage").Value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        if (node != null)
                        {
                            thankYouPage = node[0];
                        }
                    }
                    if (newMember == null)
                    {
                        newMember = ApplicationContext.Services.MemberService.CreateMember(registration.Email, registration.Email, registration.Name + ", " + registration.Surname, "Member");

                        newMember.SetValue("memberName", registration.Name);
                        newMember.SetValue("surname", registration.Surname);
                        newMember.SetValue("role", registration.Role);
                        newMember.SetValue("company", registration.Company);
                        newMember.SetValue("market", registration.Market);
                        newMember.SetValue("nation", registration.Nation);
                        newMember.SetValue("telephone", registration.Telephone);
                        newMember.IsLockedOut = false;
                        ApplicationContext.Services.MemberService.Save(newMember);
                        ApplicationContext.Services.MemberService.SavePassword(newMember, registration.Password);
                        ApplicationContext.Services.MemberService.AssignRole(newMember.Id, "PendingUsers");

                        if (newMember.Id > 0)
                        {
                            bool success = SendAutoReplyEmail(registration, currentpage, registrationPage);
                            if (success)
                            {
                                TempData["sucessMessage"] = "ActivationMailSend";
                                if (!string.IsNullOrEmpty(thankYouPage))
                                {
                                    Node redirectPage = new Node(int.Parse(thankYouPage));
                                    //  return RedirectToUmbracoPage(int.Parse(thankYouPage));
                                    return new JsonNetResult { Data = new { Status = "Success", Url = redirectPage.Url } };
                                }
                                // return CurrentUmbracoPage();
                                return new JsonNetResult { Data = new { Status = "Success", Message = "" } };
                            }
                            else
                            {
                                return new JsonNetResult { Data = new { Status = "Failed", Message = "" } };
                            }
                        }
                        else
                        {
                            return new JsonNetResult { Data = new { Status = "Failed", Message = "" } };

                        }
                    }
                    else
                    {
                        return new JsonNetResult { Data = new { Status = "AlreadyExist", Message = "" } };

                    }
                }
                else
                {
                    return new JsonNetResult { Data = new { Status = "Failed", Message = "" } };
                }
            }
            catch (Exception)
            {

                return new JsonNetResult { Data = new { Status = "Failed", Message = "" } };
            }
            // return CurrentUmbracoPage();
        }

        public ActionResult SubmitFormDownload(FormDownload form)
        {
            try
            {
                string response = Request["g-recaptcha-response"];
                if (form != null && ModelState.IsValid && !string.IsNullOrEmpty(response) && Crifireland.Utils.Utility.isCaptchaValid(response))
                {
                    DownloadedContents docsDownloaded = new DownloadedContents();
                    docsDownloaded.FirstName = form.Name;
                    docsDownloaded.LastName = form.Surname;
                    docsDownloaded.Email = form.Email;
                    docsDownloaded.Telephone = form.Telephone;
                    docsDownloaded.UserRole = form.Role;
                    docsDownloaded.Company = form.Company;
                    docsDownloaded.CreatedDate = DateTime.Now;
                    docsDownloaded.ClientIP = Utility.GetClientIP();
                    docsDownloaded.Device = Utility.GetDevice();
                    docsDownloaded.Browser = Utility.GetBrowser();
                    docsDownloaded.UserAgent = Utility.GetUserAgent();
                    docsDownloaded.DevicePlatform = Utility.GetDevicePlatform();

                    Node currentpage = Node.GetCurrent();
                    if (currentpage.NodeTypeAlias == "singleResearchPage")
                    {
                        docsDownloaded.IsProduct = 0;
                        docsDownloaded.IsSuccessStory = 0;
                    }
                    else if (currentpage.NodeTypeAlias == "successStorySinglePage")
                    {
                        docsDownloaded.IsSuccessStory = 1;
                    }
                    else if (currentpage.NodeTypeAlias == "productOrService")
                    {
                        docsDownloaded.IsProduct = 1;
                    }

                    var formUrl = "";
                    Node formNode = new Node(int.Parse(form.NodeId));
                    if (formNode != null)
                    {
                        formUrl = formNode.Url;
                    }
                    docsDownloaded.FormId = formUrl;
                    // var mediaNode = new Node(int.Parse(form.contentId));

                    var mediaUrl = "";
                    var mediaNoe = Umbraco.TypedMedia(form.contentId);
                    if (mediaNoe != null)
                    {
                        mediaUrl = mediaNoe.Url;
                    }
                    docsDownloaded.contentUrl = mediaUrl;


                    var db = ApplicationContext.DatabaseContext.Database;
                    var id = db.Insert(docsDownloaded);
                    if (id != null)
                    {
                        if (currentpage.NodeTypeAlias == "singleResearchPage")
                        {
                            Node researchAndPublicationHome = new Node(int.Parse(umbraco.library.GetDictionaryItem("ResearchAndPublicationHomeId")));
                            SendAdminEmail(form ,researchAndPublicationHome);
                            SendAutoReplyEmail(form, researchAndPublicationHome);
                        }
                        else if (currentpage.NodeTypeAlias == "successStorySinglePage")
                        {
                            Node ContactPageId = new Node(int.Parse(umbraco.library.GetDictionaryItem("ContactPageId")));
                            SendAdminEmail(form, ContactPageId);
                            SendAutoReplyEmail(form, ContactPageId);
                        }
                        else if (currentpage.NodeTypeAlias == "productOrService")
                        {
                            Node ProductAndServiceHomeId = new Node(int.Parse(umbraco.library.GetDictionaryItem("ProductAndServiceHomeId")));
                            SendAdminEmail(form, ProductAndServiceHomeId);
                            SendAutoReplyEmail(form, ProductAndServiceHomeId);
                        }
                        var contactUsNode = Umbraco.TypedContent(form.NodeId);
                        int resultId = Convert.ToInt32(id);
                        if (!string.IsNullOrEmpty(form.contentId))
                        {
                            return new JsonNetResult { Data = new { Status = "Success", Message = "", IsMedia = 1, NodeId = form.NodeId, MediaId = form.contentId } };
                        }
                        else
                        {
                            return new JsonNetResult { Data = new { Status = "Success", Message = "", NodeId = form.NodeId, IsMedia = 0 } };
                        }

                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
            return new JsonNetResult { Data = new { Status = "Failed", Message = "" } };
        }

        private bool SendAutoReplyEmail(Registration registration, Node currentpage, Node registrationPage)
        {
            bool success = false;
            string toEmailAddress = ""
                        , bccvalue = ""
                        , emailSubject = ""
                        , emailbody = ""
                        , replyMesssage = ""
                        , replyemailSubject = ""
                        , date = DateTime.Now.Date.ToString("dd MMM yyyy");
            var homeNode = Umbraco.TypedContent(currentpage.Id).AncestorOrSelf(1);
            var domains = umbraco.library.GetCurrentDomains(homeNode.Id);
            string url = "";
            foreach (var domain in domains)
            {
                if (domain.Name.Contains("https://"))
                {
                    Uri uri = new Uri(domain.Name);
                    int defaultPort = Request.IsSecureConnection ? 443 : 80;
                    url = uri.Scheme + Uri.SchemeDelimiter + uri.Host + (uri.Port != defaultPort ? ":" + uri.Port : "");
                }
            }
            string activationLink = registrationPage.GetProperty("activationLinkText") != null && !string.IsNullOrEmpty(registrationPage.GetProperty("activationLinkText").Value)
                                   ? registrationPage.GetProperty("activationLinkText").Value : "Click Here to activate your account";

            try
            {
                if (registrationPage.GetProperty("replyToUser") != null && !string.IsNullOrEmpty(registrationPage.GetProperty("replyToUser").Value) && registrationPage.GetProperty("replyToUser").Value == "1")
                {
                    if (registrationPage.GetProperty("replyEmailSubject") != null && !string.IsNullOrEmpty(registrationPage.GetProperty("replyEmailSubject").Value.Trim()))
                    {
                        replyemailSubject = registrationPage.GetProperty("replyEmailSubject").Value.Trim();
                    }
                    else
                    {
                        replyemailSubject = "Thank you for contacting us.";
                    }

                    //Reply Senders Body
                    if (registrationPage.GetProperty("emailReplyBody") != null && !string.IsNullOrEmpty(registrationPage.GetProperty("emailReplyBody").Value.Trim()))
                    {
                        replyMesssage = registrationPage.GetProperty("emailReplyBody").Value.Trim();
                        replyMesssage = ReplaceConfirmationEmail(registration, replyMesssage, url, currentpage, registrationPage, activationLink);

                    }
                    if (!string.IsNullOrEmpty(registration.Email))
                    {
                        var fromEmail = registrationPage.GetProperty("fromEmail").Value;
                        Email.SendMail(replyemailSubject, replyMesssage, registration.Email.Split(',').ToList(), Enumerable.Empty<string>(), Enumerable.Empty<string>(), fromEmail, null, Enumerable.Empty<string>(), true);
                        //Email.SendMail(replyemailSubject, replyMesssage, "jerin.jose@phases.dk", "", "", fromEmail, "", true);
                    }
                }
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                LogHelper.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, ex.Message, ex);

            }
            return success;

        }

        private bool SendAdminEmail(Registration registration, Node currentpage, Node registrationPage)
        {

            bool success = false;
            string adminEmail = registrationPage.GetProperty("sendNotificationTo").Value.ToString();
            string toEmailAddress = ""
                        , bccvalue = ""
                        , emailSubject = ""
                        , emailbody = ""
                        , replyMesssage = ""
                        , replyemailSubject = ""
                        , date = DateTime.Now.Date.ToString("dd MMM yyyy");
            try
            {
                if (!string.IsNullOrEmpty(adminEmail))
                {

                    toEmailAddress = registrationPage.GetProperty("sendNotificationTo").Value;
                    if (registrationPage.GetProperty("bCCRecipients") != null && !string.IsNullOrEmpty(registrationPage.GetProperty("bCCRecipients").Value.Trim()))
                    {
                        bccvalue = registrationPage.GetProperty("bCCRecipients").Value.Trim();
                    }

                    if (registrationPage.GetProperty("eMailSubject") != null && !string.IsNullOrEmpty(registrationPage.GetProperty("eMailSubject").Value.Trim()))
                    {
                        emailSubject = registrationPage.GetProperty("eMailSubject").Value.Trim();
                        emailSubject = ReplaceConfirmationEmail(registration, emailSubject, "", currentpage, registrationPage,"");
                    }
                    else
                    {
                        emailSubject = "New email contact - " + date;
                    }

                    if (registrationPage.GetProperty("notificationBody") != null && !string.IsNullOrEmpty(registrationPage.GetProperty("notificationBody").Value.Trim()))
                    {
                        emailbody = registrationPage.GetProperty("notificationBody").Value.Trim();
                    }
                    else
                    {
                        emailbody = @"<p>Hi,</p>
                            <p>There is a new email from the Crif Italia registration page.</p>
                            <p>Please see the details below.</p>
                            <p>Date: [DATE]</p>
                            <p>Name: [NAME]</p>
                            <p>SurName: [SURNAME]</p>
                            <p>Role: [ROLE]</p>
                            <p>Market:[MARKET]</p>
                            <p>Company Name:[COMPANYNAME]</p>
                            <p>Nation : [NATION]</p>
                            <p>Email:[EMAIL]</p> 
                            <p>Telephone:[TELEPHONE]</p> 
                            <p> </p>
                            <p> </p>";
                        //< p > password:[PASSWORD]</p>     
                    }
                    emailbody = ReplaceConfirmationEmail(registration, emailbody, "", currentpage, registrationPage,"");

                    if (!string.IsNullOrEmpty(toEmailAddress))
                    {
                        var fromEmail = registrationPage.GetProperty("fromEmail").Value;
                        Email.SendMail(emailSubject, replyMesssage, toEmailAddress.Split(',').ToList(), Enumerable.Empty<string>(), bccvalue.Split(',').ToList(), fromEmail, null, Enumerable.Empty<string>(), true);

                    }


                }
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                LogHelper.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, string.Format("[RegistrationController][Register]- Error sending   admin email to {0}.", adminEmail), ex);

            }
            return success;
        }

        //[Route("crif/registerUser")]
        public ActionResult ActivateUser(string id, string node, string reg)
        {
            if (!string.IsNullOrEmpty(id))
            {
                string emailId = Cryptography.Decrypt(Utility.FromBase64Url(HttpUtility.UrlDecode(id)), ConfigurationManager.AppSettings["EncryptionPassPhrase"].ToString());
                string nodeId = node;

                var membership = ApplicationContext.Services.MemberService.GetByUsername(emailId);
                try
                {
                    if (membership != null)
                    {

                        ApplicationContext.Services.MemberService.DissociateRole(membership.Id, "PendingUsers");
                        ApplicationContext.Services.MemberService.AssignRole(membership.Id, "ApprovedUsers");
                        Registration registration = GetMemberDetails(membership);
                        Members.Login(registration.Email, registration.Password);
                        FormsAuthentication.SetAuthCookie(registration.Email, false);
                        Node currentpage = new Node(int.Parse(nodeId));
                        Node registrationPage = new Node(int.Parse(reg));
                        SendAdminEmail(registration, currentpage, registrationPage);
                        SendActivationSuccessMail(registration, currentpage, registrationPage, emailId);
                    }
                    TempData["activation"] = "true";
                    return RedirectToUmbracoPage(int.Parse(nodeId));
                }
                catch (Exception ex)
                {
                    LogHelper.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, ex.Message, ex);
                    return null;
                }
            }
            return null;
        }

        public ActionResult GetMemberByID(string member,string membermail)
        {
            string email = "";
            string member_id = "";
            if (!string.IsNullOrEmpty(member))
            {
                member_id = Cryptography.Decrypt(Utility.FromBase64Url(HttpUtility.UrlDecode(member)), ConfigurationManager.AppSettings["EncryptionPassPhrase"].ToString());
            }
            if (!string.IsNullOrEmpty(membermail))
            {
                email = Cryptography.Decrypt(Utility.FromBase64Url(HttpUtility.UrlDecode(membermail)), ConfigurationManager.AppSettings["EncryptionPassPhrase"].ToString());
            }
            var currentMember = Phases.UmbracoUtils.Member.GetCurrentLoggedinMember();
            if (currentMember != null && !string.IsNullOrEmpty(member) && currentMember.Id == int.Parse(member_id) && !string.IsNullOrEmpty(email) && currentMember.Email == email)
            {
                FormDownload form = new FormDownload();
                var membership = ApplicationContext.Services.MemberService.GetById(int.Parse(member_id));
                if (membership != null)
                {
                    try
                    {
                        if ((membership.Properties["memberName"]).Value != null)
                        {
                            form.Name = (membership.Properties["memberName"]).Value.ToString();
                        }
                        if ((membership.Properties["surname"]).Value != null)
                        {
                            form.Surname = (membership.Properties["surname"]).Value.ToString();
                        }
                        if ((membership.Properties["company"]).Value != null)
                        {
                            form.Company = (membership.Properties["company"]).Value.ToString();
                        }
                        if ((membership.Properties["role"]).Value != null)
                        {
                            form.Role = (membership.Properties["role"]).Value.ToString();
                        }
                        if (!string.IsNullOrEmpty(membership.Email))
                        {
                            form.Email = membership.Email;
                        }
                        if ((membership.Properties["telephone"]).Value != null)
                        {
                            form.Telephone = (membership.Properties["telephone"]).Value.ToString();
                        }



                        return new JsonNetResult { Data = new { Status = "Success", Name = form.Name, SurName = form.Surname, Company = form.Company, Role = form.Role, Email = form.Email, Telephone = form.Telephone } };

                    }
                    catch (Exception ex)
                    {
                        return new JsonNetResult { Data = new { Status = "Failed", Message = "" } };
                    }

                }
            }
            return new JsonNetResult { Data = new { Status = "Failed", Message = "" } };
        }

        public Registration GetMemberDetails(IMember member)
        {

            Registration registration = new Registration();

            registration.Name = (member.Properties["memberName"]).Value.ToString();
            registration.Surname = (member.Properties["surname"]).Value.ToString();
            registration.Company = (member.Properties["company"]).Value.ToString();
            registration.Email = member.Email;
            registration.Password = member.RawPasswordValue;
            registration.Market = (member.Properties["market"]).Value.ToString();
            registration.Nation = (member.Properties["nation"]).Value.ToString();
            if ((member.Properties["role"]).Value != null)
            {
                registration.Role = (member.Properties["role"]).Value.ToString();
            }
            else
            {
                registration.Role = string.Empty;
            }
            registration.Telephone = (member.Properties["telephone"]).Value.ToString();
            return registration;
        }

        private string ReplaceConfirmationEmail(Registration registration, string emailContent, string url, Node currentpage, Node registrationPage, string activationLink)
        {
            string activationLinkForUser = "";
            string namePlaceholder = "[NAME]",
                   surnamePlaceholder = "[SURNAME]",
                   rolePlaceholder = "[ROLE]",
                   companyNamePlaceholder = "[COMPANYNAME]",
                   marketPlaceHolder = "[MARKET]",
                   nationPlaceHolder = "[NATION]",
                   telephonePlaceHolder = "[TELEPHONE]",
                   emailPlaceholder = "[EMAIL]",
                   passwordPlaceholder = "[PASSWORD]",
                   activationMailLink = "[ACTIVATIONLINK]",
                   datePlaceholder = "[DATE]";

            if (url!=null)
            {
                string encryptedMailId = Utility.ToBase64Url(Cryptography.Encrypt(registration.Email, ConfigurationManager.AppSettings["EncryptionPassPhrase"].ToString()));

                activationLinkForUser = "<a href='" + url + "/umbraco/Surface/Registration/ActivateUser?id=" + HttpUtility.UrlEncode(encryptedMailId) +
                    "&node=" + HttpUtility.UrlEncode(currentpage.Id.ToString()) + "&reg=" + HttpUtility.UrlEncode(registrationPage.Id.ToString()) + " '>" + activationLink + "</a>";
            }
            Node marketPage = new Node(int.Parse(registration.Market));
            emailContent = emailContent.Replace(namePlaceholder, registration.Name)
                                  .Replace(surnamePlaceholder, registration.Surname)
                                  .Replace(rolePlaceholder, registration.Role)
                                  .Replace(companyNamePlaceholder, registration.Company)
                                  .Replace(emailPlaceholder, registration.Email)
                                  .Replace(telephonePlaceHolder, registration.Telephone)
                                  .Replace(marketPlaceHolder, marketPage.Name)
                                  .Replace(nationPlaceHolder, registration.Nation)
                                  .Replace(passwordPlaceholder, registration.Password)
                                  .Replace(activationMailLink, activationLinkForUser)

                                  .Replace(datePlaceholder, DateTime.Now.Date.ToString("dd MMMM yyyy"));

            return emailContent;
        }

        public ActionResult Login(Login model, FormCollection formCollection)
        {

            if (!string.IsNullOrWhiteSpace(model.UserName) && !string.IsNullOrWhiteSpace(model.PassWord))
            {
                var member = ApplicationContext.Services.MemberService.GetByUsername(model.UserName);
                if (member != null && member.IsLockedOut && !member.LastLockoutDate.Equals(DateTime.MinValue)
                    && member.LastLockoutDate.AddHours(1) < DateTime.Now)
                {
                    member.IsLockedOut = false;
                    ApplicationContext.Services.MemberService.Save(member);
                }
                if ((ModelState.IsValid) && Membership.ValidateUser(model.UserName, model.PassWord))
                {
                    //var member = ApplicationContext.Services.MemberService.GetByUsername(model.UserName);
                    if (member != null)
                    {
                        string[] roles = Roles.GetRolesForUser(model.UserName);
                        if (roles != null && roles.Contains("ApprovedUsers"))
                        {
                            string membername = member.GetValue<string>("memberName");
                            //string nodeTypeAlias = Node.GetCurrent().NodeTypeAlias;
                            //FormsAuthentication.SetAuthCookie(member.Username, true);
                            //if (nodeTypeAlias == "registration" || nodeTypeAlias == "standardPageWithoutLink")
                            //{
                            //    Response.Redirect("/");
                            //}
                            // Response.Redirect("/")
                            var authTicket = new FormsAuthenticationTicket(1, member.Username,
                                DateTime.Now, DateTime.Now.AddMinutes(30), true,
                                "ApprovedUsers");
                            string cookieContents = FormsAuthentication.Encrypt(authTicket);
                            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookieContents)
                            {
                                Expires = authTicket.Expiration,
                                Path = FormsAuthentication.FormsCookiePath,
                                Secure = true,
                                HttpOnly = true
                            };
                            Response.Cookies.Add(cookie);
                            Response.SetCookie(cookie);
                            Session["LoginId"] = member.GetValue<string>("memberName");
                            return new JsonNetResult { Data = new { Status = "Success", Message = "" } };
                            //return RedirectToCurrentUmbracoPage();
                        }
                        else
                        {
                            return new JsonNetResult { Data = new { Status = "Pending", Message = "" } };
                        }
                    }
                    else
                    {
                        return new JsonNetResult { Data = new { Status = "Failed", Message = "" } };
                    }
                }
                else
                {
                    return new JsonNetResult { Data = new { Status = "Failed", Message = "" } };
                }
            }
            else
            {
                return new JsonNetResult { Data = new { Status = "Failed", Message = "" } };
            }
            // return RedirectToCurrentUmbracoPage();
        }

        public ActionResult MemberLogout()
        {
            Session.Abandon();
            Session.Clear();
            Session.RemoveAll();
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }
            FormsAuthentication.SignOut();
            return Redirect("/");
        }
        //Form for Download - SendAdminEmail
        private bool SendAdminEmail(FormDownload form, Node registrationPage)
        {

            bool success = false;
            bool isregistered=false;

            if (!string.IsNullOrEmpty(form.Email))
            {
                var member = ApplicationContext.Services.MemberService.GetByUsername(form.Email);
                if (member != null)
                {
                    string[] roles = Roles.GetRolesForUser(form.Email);
                    if (roles != null && roles.Contains("ApprovedUsers"))
                    {
                        isregistered = true;
                    }
                }

            }
            string adminEmail = registrationPage.GetProperty("formDownloadSendNotificationTo").Value.ToString();
            string toEmailAddress = ""
                        , bccvalue = ""
                        , emailSubject = ""
                        , emailbody = ""
                        , replyMesssage = ""
                        , replyemailSubject = ""
                        , date = DateTime.Now.Date.ToString("dd MMM yyyy");
            try
            {
                if (!string.IsNullOrEmpty(adminEmail))
                {

                    toEmailAddress = registrationPage.GetProperty("formDownloadSendNotificationTo").Value;
                    if (registrationPage.GetProperty("formDownloadBCCRecipients") != null && !string.IsNullOrEmpty(registrationPage.GetProperty("formDownloadBCCRecipients").Value.Trim()))
                    {
                        bccvalue = registrationPage.GetProperty("formDownloadBCCRecipients").Value.Trim();
                    }

                    if (registrationPage.GetProperty("formDownloadEMailSubject") != null && !string.IsNullOrEmpty(registrationPage.GetProperty("formDownloadEMailSubject").Value.Trim()))
                    {
                        emailSubject = registrationPage.GetProperty("formDownloadEMailSubject").Value.Trim();
                        emailSubject = ReplaceConfirmationEmail(form, emailSubject, "", isregistered);
                    }
                    else
                    {
                        emailSubject = "New email contact - " + date;
                    }

                    if (registrationPage.GetProperty("formDownloadNotificationBody") != null && !string.IsNullOrEmpty(registrationPage.GetProperty("formDownloadNotificationBody").Value.Trim()))
                    {
                        emailbody = registrationPage.GetProperty("formDownloadNotificationBody").Value.Trim();
                    }
                    else
                    {
                        emailbody = @"<p>Hi,</p>
                            <p>There is a new email from the Crif Italia registration page.</p>
                            <p>Please see the details below.</p>
                            <p>Date: [DATE]</p>
                            <p>Name: [NAME]</p>
                            <p>SurName: [SURNAME]</p>
                            <p>Role: [ROLE]</p>
                            <p>Company Name:[COMPANYNAME]</p>
                            <p>Email:[EMAIL]</p> 
                            <p>Telephone:[TELEPHONE]</p> 
                            <p> </p>
                            <p> </p>";
                        //< p > password:[PASSWORD]</p>     
                    }
                    emailbody = ReplaceConfirmationEmail(form, emailbody, "", isregistered);

                    if (!string.IsNullOrEmpty(toEmailAddress))
                    {
                        var fromEmail = registrationPage.GetProperty("formDownloadFromEmail").Value;

                        Email.SendMail(emailSubject, replyMesssage, toEmailAddress.Split(',').ToList(), Enumerable.Empty<string>(), bccvalue.Split(',').ToList(), fromEmail, null, Enumerable.Empty<string>(), true);
                    }


                }
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                LogHelper.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, string.Format("[RegistrationController][FormDownload]- Error sending   admin email to {0}.", adminEmail), ex);

            }
            return success;
        }

        //Form for Download - SendAutoReplyEmail
        private bool SendAutoReplyEmail(FormDownload form, Node registrationPage)
        {

            bool success = false;
            string toEmailAddress = ""
                        , bccvalue = ""
                        , emailSubject = ""
                        , emailbody = ""
                        , replyMesssage = ""
                        , replyemailSubject = ""
                        , date = DateTime.Now.Date.ToString("dd MMM yyyy");
            bool isregistered = false;

            if (!string.IsNullOrEmpty(form.Email))
            {
                var member = ApplicationContext.Services.MemberService.GetByUsername(form.Email);
                if (member != null)
                {
                    string[] roles = Roles.GetRolesForUser(form.Email);
                    if (roles != null && roles.Contains("ApprovedUsers"))
                    {
                        isregistered = true;
                    }
                }

            }
            try
            {
                if (registrationPage.GetProperty("formDownloadReplyToUser") != null && !string.IsNullOrEmpty(registrationPage.GetProperty("formDownloadReplyToUser").Value) && registrationPage.GetProperty("formDownloadReplyToUser").Value == "1")
                {
                    if (registrationPage.GetProperty("formDownloadReplyEmailSubject") != null && !string.IsNullOrEmpty(registrationPage.GetProperty("formDownloadReplyEmailSubject").Value.Trim()))
                    {
                        replyemailSubject = registrationPage.GetProperty("formDownloadReplyEmailSubject").Value.Trim();
                    }
                    else
                    {
                        replyemailSubject = "Thank you for contacting us.";
                    }

                    //Reply Senders Body
                    if (registrationPage.GetProperty("formDownloadEmailReplyBody") != null && !string.IsNullOrEmpty(registrationPage.GetProperty("formDownloadEmailReplyBody").Value.Trim()))
                    {
                        replyMesssage = registrationPage.GetProperty("formDownloadEmailReplyBody").Value.Trim();
                        replyMesssage = ReplaceConfirmationEmail(form, replyMesssage, "", isregistered);

                    }
                    if (!string.IsNullOrEmpty(form.Email))
                    {
                        var fromEmail = registrationPage.GetProperty("formDownloadFromEmail").Value;


                        Email.SendMail(replyemailSubject, replyMesssage, form.Email.Split(',').ToList(), Enumerable.Empty<string>(), Enumerable.Empty<string>(), fromEmail, null, Enumerable.Empty<string>(), true);
                        //Email.SendMail(replyemailSubject, replyMesssage, "jerin.jose@phases.dk", "", "", fromEmail, "", true);
                    }
                }
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                LogHelper.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, ex.Message, ex);

            }
            return success;

        }

        private bool SendActivationSuccessMail(Registration registration, Node currentpage, Node registrationPage, string emailId)
        {
            bool success = false;
            string toEmailAddress = ""
                        , bccvalue = ""
                        , emailSubject = ""
                        , emailbody = ""
                        , replyMesssage = ""
                        , replyemailSubject = ""
                        , date = DateTime.Now.Date.ToString("dd MMM yyyy");
            try
            {
                if (!string.IsNullOrEmpty(emailId))
                {

                    toEmailAddress = emailId;

                    if (registrationPage.GetProperty("approvedEmailSubject") != null && !string.IsNullOrEmpty(registrationPage.GetProperty("approvedEmailSubject").Value.Trim()))
                    {
                        emailSubject = registrationPage.GetProperty("approvedEmailSubject").Value.Trim();
                        emailSubject = ReplaceConfirmationEmail(registration, emailSubject, "", currentpage, registrationPage, "");
                    }
                    else
                    {
                        emailSubject = "Approval Email - " + date;
                    }

                    if (registrationPage.GetProperty("emailBody") != null && !string.IsNullOrEmpty(registrationPage.GetProperty("emailBody").Value.Trim()))
                    {
                        emailbody = registrationPage.GetProperty("emailBody").Value.Trim();
                    }
                    else
                    {
                        emailbody = @"<p>Hi [NAME]</p>
                                    <p>Your Crif Italia account have been activated.</p>
                                    <p>Thanks</p>
                                    <p>Kind Regards,</p>
                                    <p>CRIF</p>
                                    <p> </p>
                                    <p> </p>";
                        //< p > password:[PASSWORD]</p>     
                    }
                    emailbody = ReplaceConfirmationEmail(registration, emailbody, "", currentpage, registrationPage, "");

                    if (!string.IsNullOrEmpty(toEmailAddress))
                    {
                        var fromEmail = registrationPage.GetProperty("senderEmail") != null && !string.IsNullOrEmpty(registrationPage.GetProperty("senderEmail").Value)
                            ? registrationPage.GetProperty("senderEmail").Value : "";
                        Email.SendMail(emailSubject, emailbody, toEmailAddress.Split(',').ToList(), Enumerable.Empty<string>(), Enumerable.Empty<string>(), fromEmail, null, Enumerable.Empty<string>(), true);
                    }


                }
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                LogHelper.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, string.Format("[RegistrationController][Register]- Error sending   admin email to {0}.", toEmailAddress), ex);

            }
            return success;
        }

        private string ReplaceConfirmationEmail(FormDownload registration, string emailContent, string url, bool isRegisteredUser)
        {
            var formUrl = ""; var contenturl = ""; IPublishedContent mediaNode = null;
            Node formNode = new Node(int.Parse(registration.NodeId));
            if (!string.IsNullOrEmpty(registration.contentId))
            {
                mediaNode = Umbraco.TypedMedia(int.Parse(registration.contentId));
            }
            if (formNode != null)
            {
                formUrl = Utility.GetHyperLink(formNode.Url, formNode.Name);
            }
            if (mediaNode != null)
            {
                contenturl = Utility.GetHyperLink(mediaNode.Url, mediaNode.Name);
            }
           string namePlaceholder = "[NAME]",
                   surnamePlaceholder = "[SURNAME]",
                   rolePlaceholder = "[ROLE]",
                   companyNamePlaceholder = "[COMPANY]",
                   telephonePlaceHolder = "[TELEPHONE]",
                   emailPlaceholder = "[EMAIL]",
                   formUrlPlaceholder = "[PAGEURL]",
                   formTitlePlaceholder = "[PAGETITLE]",
                   isRegisteredPlaceholder="[ISREGISTERED]",
                   mediaContentUrlPlaceholder="[DOWNLOADFILE]",
                   datePlaceholder = "[DATE]";

            emailContent = emailContent.Replace(namePlaceholder, registration.Name)
                                  .Replace(surnamePlaceholder, registration.Surname)
                                  .Replace(rolePlaceholder, registration.Role)
                                  .Replace(companyNamePlaceholder, registration.Company)
                                  .Replace(emailPlaceholder, registration.Email)
                                  .Replace(telephonePlaceHolder, registration.Telephone)
                                  .Replace(formUrlPlaceholder, formUrl)
                                  .Replace(mediaContentUrlPlaceholder, contenturl)
                                  .Replace(formTitlePlaceholder, formNode.Name)
                                  .Replace(isRegisteredPlaceholder, (isRegisteredUser)?"Y":"N")
                                  .Replace(datePlaceholder, DateTime.Now.Date.ToString("dd MMMM yyyy"));

            return emailContent;
        }
    }
}