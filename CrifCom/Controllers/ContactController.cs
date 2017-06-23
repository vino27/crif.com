using Crifireland.Utils;
using CrifCom.Models;
using Phases.UmbracoUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using umbraco.NodeFactory;
using Umbraco.Web.Mvc;
using CrifCom.ViewModels;

namespace CrifCom.Controllers
{
    public class ContactController : SurfaceController
    {
        public ActionResult SubmitContact(Contact contact)
        {
            Node currentpage = Node.GetCurrent();
            try
            {
                string response = Request["g-recaptcha-response"];
                if (ModelState.IsValid && contact.IsPrivacyChecked && !string.IsNullOrEmpty(response) && Crifireland.Utils.Utility.isCaptchaValid(response))
                {
                    string toEmailAddress = ""
                            , bccvalue = ""
                            , emailSubject = ""
                            , emailbody = ""
                            , replyMesssage = ""
                            , replyemailSubject = "";
                    contact.CreatedDate = DateTime.Now;
                    contact.ClientIP = Utility.GetClientIP();
                    contact.UserAgent = Utility.GetUserAgent();
                    contact.Browser = Utility.GetBrowser();
                    contact.DevicePlatform = Utility.GetDevicePlatform();
                    contact.Device = Utility.GetDevicePlatform();
                    contact.FormId = currentpage.Id;
                    contact.IsProduct = 0;
                    //string message = null;
                    if (contact.Message != null)
                    {
                        contact.Message = contact.Message.Replace("\n", "<br/>");
                    }

                    var db = ApplicationContext.DatabaseContext.Database;
                    var id = db.Insert(contact);

                    if (currentpage.GetProperty("sendNotificationTo") != null && !string.IsNullOrEmpty(currentpage.GetProperty("sendNotificationTo").Value))
                    {
                        toEmailAddress = currentpage.GetProperty("sendNotificationTo").Value;
                    }
                    if (currentpage.GetProperty("bCCRecipients") != null && !string.IsNullOrEmpty(currentpage.GetProperty("bCCRecipients").Value))
                    {
                        bccvalue = currentpage.GetProperty("bCCRecipients").Value.Trim();
                    }

                    if (currentpage.GetProperty("eMailSubject") != null && !string.IsNullOrEmpty(currentpage.GetProperty("eMailSubject").Value))
                    {
                        emailSubject = currentpage.GetProperty("eMailSubject").Value;
                        emailSubject = ReplacePlaceholders(emailSubject, contact);
                    }
                    else
                    {
                        emailSubject = "New email contact - " + DateTime.Now.Date.ToString("dd MMM yyyy hh:mm");
                    }

                    if (currentpage.GetProperty("notificationBody") != null && !string.IsNullOrEmpty(currentpage.GetProperty("notificationBody").Value))
                    {
                        emailbody = currentpage.GetProperty("notificationBody").Value;
                    }
                    else
                    {
                        emailbody = @"<p>Hi,</p>
                            <p>There is a new email from the crif Italia contact page.</p>
                            <p>Please see the details below.</p>
                                <p>Name : [NAME]</p>
                                <p>surname :  [SURNAME]</p>
                                <p>CompanyPlaceholder : [COMPANY]</p>
                                <p>Email : [EMAIL]</p>
                                <p>RequestedType :[REQUEST TYPE]</p>
                                <p>Message : [MESSAGE]</p>
                                <p>Nation : [NATION]</p>
                                <p>Role : [ROLE]</p>
                                <p>Telephone:[TELEPHONE]</p>
                                <p>Market :[MARKET]</p>
                                <p>Date :[DATE]</p>
                                <p> </p>
                                <p> </p>";
                    }
                    emailbody = ReplacePlaceholders(emailbody, contact);
                    if (currentpage.GetProperty("fromEmail") != null && !string.IsNullOrEmpty(currentpage.GetProperty("fromEmail").Value))
                    {
                        var fromEmail = currentpage.GetProperty("fromEmail").Value;
                        Email.SendMail(emailSubject, emailbody, toEmailAddress.Split(',').ToList(), Enumerable.Empty<string>(), bccvalue.Split(',').ToList(), fromEmail, "", Enumerable.Empty<string>(), true);
                    }

                    if (currentpage.GetProperty("replyToUser") != null && !string.IsNullOrEmpty(currentpage.GetProperty("replyToUser").Value) && currentpage.GetProperty("replyToUser").Value == "1")
                    {
                        if (currentpage.GetProperty("replyEmailSubject") != null && !string.IsNullOrEmpty(currentpage.GetProperty("replyEmailSubject").Value))
                        {
                            replyemailSubject = currentpage.GetProperty("replyEmailSubject").Value;
                        }
                        else
                        {
                            replyemailSubject = "Thank you for contacting us.";
                        }

                        //Reply Senders Body
                        if (currentpage.GetProperty("emailReplyBody") != null && !string.IsNullOrEmpty(currentpage.GetProperty("emailReplyBody").Value))
                        {
                            replyMesssage = currentpage.GetProperty("emailReplyBody").Value.Trim();
                            replyMesssage = ReplacePlaceholders(replyMesssage, contact);

                        }
                        if (!string.IsNullOrEmpty(contact.Email))
                        {
                            var fromEmail = currentpage.GetProperty("fromEmail").Value;
                            Email.SendMail(replyemailSubject, replyMesssage, contact.Email.Split(',').ToList(), Enumerable.Empty<string>(), Enumerable.Empty<string>(), fromEmail,null, Enumerable.Empty<string>(), true);
                        }
                    }

                    if (id != null)
                    {
                        if (currentpage.GetProperty("redirectForThankYouPage") != null && !string.IsNullOrEmpty(currentpage.GetProperty("redirectForThankYouPage").Value))
                        {
                            Response.Redirect(umbraco.library.NiceUrl(int.Parse((currentpage.GetProperty("redirectForThankYouPage").Value).ToString())));
                        }
                    }
                }
                else
                {
                    TempData["falied"] = currentpage.GetProperty("contactRequestFailedMessage") != null && !string.IsNullOrEmpty(currentpage.GetProperty("contactRequestFailedMessage").Value)
                        ? currentpage.GetProperty("contactRequestFailedMessage").Value : "The request cannot be submitted";
                }
            }
            catch (Exception ex)
            {
                //TempData["falied"] = currentpage.GetProperty("contactRequestFailedMessage") != null && !string.IsNullOrEmpty(currentpage.GetProperty("contactRequestFailedMessage").Value)
                //        ? currentpage.GetProperty("contactRequestFailedMessage").Value : "The request cannot be submitted";
                TempData["falied"] = ex.Message;
                return CurrentUmbracoPage();
            }
            return CurrentUmbracoPage();
        }
        public ActionResult SubmitModalContactDetails(ModalContactForm contact)
        {
            try
            {
                if (ModelState.IsValid && contact.IsPrivacyChecked)
                {
                    Node currentpage = Node.GetCurrent();
                    Node contactPage = new Node(int.Parse(umbraco.library.GetDictionaryItem("ContactPageId")));
                    string toEmailAddress = ""
                            , bccvalue = ""
                            , emailSubject = ""
                            , emailbody = ""
                            , replyMesssage = ""
                            , replyemailSubject = "";
                    contact.CreatedDate = DateTime.Now;
                    contact.ClientIP = Utility.GetClientIP();
                    contact.UserAgent = Utility.GetUserAgent();
                    contact.Browser = Utility.GetBrowser();
                    contact.DevicePlatform = Utility.GetDevicePlatform();
                    contact.Device = Utility.GetDevicePlatform();
                    contact.FormId = contactPage.Id;
                    contact.IsProduct = 1;
                    contact.ProductName = currentpage.Name;
                    contact.ProductPageUrl = currentpage.Url;

                    //string message = null;
                    if (contact.Message != null)
                    {
                        contact.Message = contact.Message.Replace("\n", "<br/>");
                    }

                    var db = ApplicationContext.DatabaseContext.Database;
                    var id = db.Insert(contact);

                    if (contactPage.GetProperty("sendNotificationTo") != null && !string.IsNullOrEmpty(contactPage.GetProperty("sendNotificationTo").Value))
                    {
                        toEmailAddress = contactPage.GetProperty("sendNotificationTo").Value;
                    }
                    if (contactPage.GetProperty("bCCRecipients") != null && !string.IsNullOrEmpty(contactPage.GetProperty("bCCRecipients").Value))
                    {
                        bccvalue = contactPage.GetProperty("bCCRecipients").Value.Trim();
                    }

                    if (contactPage.GetProperty("eMailSubject") != null && !string.IsNullOrEmpty(contactPage.GetProperty("eMailSubject").Value))
                    {
                        emailSubject = contactPage.GetProperty("eMailSubject").Value;
                        emailSubject = ReplaceModalPlaceHolders(emailSubject, contact);
                    }
                    else
                    {
                        emailSubject = "New email contact - " + DateTime.Now.Date.ToString("dd MMM yyyy hh:mm");
                    }

                    if (contactPage.GetProperty("notificationBody") != null && !string.IsNullOrEmpty(contactPage.GetProperty("notificationBody").Value))
                    {
                        emailbody = contactPage.GetProperty("notificationBody").Value;
                    }
                    else
                    {
                        emailbody = @"<p>Hi,</p>
                            <p>There is a new email from the crif Italia contact page.</p>
                            <p>Please see the details below.</p>
                                <p>Name : [NAME]</p>
                                <p>surname :  [SURNAME]</p>
                                <p>CompanyPlaceholder : [COMPANY]</p>
                                <p>Email : [EMAIL]</p>
                                <p>RequestedType :[REQUEST TYPE]</p>
                                <p>Message : [MESSAGE]</p>
                                <p>Nation : [NATION]</p>
                                <p>Role : [ROLE]</p>
                                <p>Telephone:[TELEPHONE]</p>
                                <p>Market :[MARKET]</p>
                                <p>Date :[DATE]</p>
                                <p> </p>
                                <p> </p>";
                    }
                    emailbody = ReplaceModalPlaceHolders(emailbody, contact);
                    if (contactPage.GetProperty("fromEmail") != null && !string.IsNullOrEmpty(contactPage.GetProperty("fromEmail").Value))
                    {
                        var fromEmail = contactPage.GetProperty("fromEmail").Value;
                        Email.SendMail(emailSubject, emailbody, toEmailAddress.Split(',').ToList(), Enumerable.Empty<string>(), bccvalue.Split(',').ToList(), fromEmail,null , Enumerable.Empty<string>(), true);
                    }

                    if (contactPage.GetProperty("replyToUser") != null && !string.IsNullOrEmpty(contactPage.GetProperty("replyToUser").Value) && contactPage.GetProperty("replyToUser").Value == "1")
                    {
                        if (contactPage.GetProperty("replyEmailSubject") != null && !string.IsNullOrEmpty(contactPage.GetProperty("replyEmailSubject").Value))
                        {
                            replyemailSubject = contactPage.GetProperty("replyEmailSubject").Value;
                        }
                        else
                        {
                            replyemailSubject = "Thank you for contacting us.";
                        }

                        //Reply Senders Body
                        if (contactPage.GetProperty("emailReplyBody") != null && !string.IsNullOrEmpty(contactPage.GetProperty("emailReplyBody").Value))
                        {
                            replyMesssage = contactPage.GetProperty("emailReplyBody").Value.Trim();
                            replyMesssage = ReplaceModalPlaceHolders(replyMesssage, contact);

                        }
                        if (!string.IsNullOrEmpty(contact.Email))
                        {
                            var fromEmail = contactPage.GetProperty("fromEmail").Value;
                            Email.SendMail(replyemailSubject, replyMesssage, contact.Email.Split(',').ToList(), Enumerable.Empty<string>(), Enumerable.Empty<string>(), fromEmail,null , Enumerable.Empty<string>(), true);
                        }
                    }

                    if (id != null)
                    {
                        var thankYbouPageId = contactPage.GetProperty("redirectForThankYouPage").Value;
                        Node thankYouPage = new Node(int.Parse(thankYbouPageId));
                        return new JsonNetResult { Data = new { Status = "Success", Url = thankYouPage.Url } };
                        //if (currentpage.GetProperty("redirectForThankYouPage") != null && !string.IsNullOrEmpty(currentpage.GetProperty("redirectForThankYouPage").Value))
                        //{

                        //    Response.Redirect(umbraco.library.NiceUrl(int.Parse((currentpage.GetProperty("redirectForThankYouPage").Value).ToString())));
                        //}

                    }

                }
                else
                {
                    return new JsonNetResult { Data = new { Status = "Failed", Url = "" } };
                }
            }
            catch
            {
                return new JsonNetResult { Data = new { Status = "Failed", Url = "" } };
            }
            return new JsonNetResult { Data = new { Status = "Failed", Url = "" } };
        }
        public string ReplacePlaceholders(string mailContent, Contact contact)
        {

            string namePlaceholder = "[NAME]",
                    surnamePlaceholder = "[SURNAME]",
                    companyPlaceholder = "[COMPANY]",
                    nationPlaceholder = "[NATION]",
                    emailPlaceholder = "[EMAIL]",
                    requestedTypePlaceholder = "[REQUEST TYPE]",
                    messagePlaceholder = "[MESSAGE]",
                    rolePlaceholder = " [ROLE]",
                    telephonePlaceholder = "[TELEPHONE]",
                    marketPlaceholder = "[MARKET]",
                    datePlaceholder = "[DATE]",
                    date = DateTime.Now.Date.ToString("dd MMM yyyy hh:mm");

            mailContent = mailContent.Replace(namePlaceholder, contact.Name)
                                           .Replace(surnamePlaceholder, contact.Surname)
                                           .Replace(rolePlaceholder, contact.Role)
                                           .Replace(companyPlaceholder, contact.Company)
                                           .Replace(emailPlaceholder, contact.Email)
                                           .Replace(requestedTypePlaceholder, contact.RequestType)
                                            .Replace(telephonePlaceholder, contact.Telephone)
                                            .Replace(messagePlaceholder, contact.Message)
                                            .Replace(nationPlaceholder, contact.Nation)
                                            .Replace(marketPlaceholder, Umbraco.TypedContent(contact.Market).Name)
                                           .Replace(datePlaceholder, date);
            return mailContent;

        }
        public string ReplaceModalPlaceHolders(string mailContent, ModalContactForm contact)
        {

            string namePlaceholder = "[NAME]",
                    surnamePlaceholder = "[SURNAME]",
                    companyPlaceholder = "[COMPANY]",
                    emailPlaceholder = "[EMAIL]",
                    nationPlaceholder = "[NATION]",
                    requestedTypePlaceholder = "[REQUEST TYPE]",
                    messagePlaceholder = "[MESSAGE]",
                    rolePlaceholder = " [ROLE]",
                    telephonePlaceholder = "[TELEPHONE]",
                    marketPlaceholder = "[MARKET]",
                    datePlaceholder = "[DATE]",
                    date = DateTime.Now.Date.ToString("dd MMM yyyy hh:mm");

            mailContent = mailContent.Replace(namePlaceholder, contact.Name)
                                           .Replace(surnamePlaceholder, contact.Surname)
                                           .Replace(rolePlaceholder, contact.Role)
                                           .Replace(companyPlaceholder, contact.Company)
                                           .Replace(emailPlaceholder, contact.Email)
                                           .Replace(requestedTypePlaceholder, contact.RequestType)
                                            .Replace(telephonePlaceholder, contact.Telephone)
                                            .Replace(nationPlaceholder, contact.Nation)
                                            .Replace(messagePlaceholder, contact.Message)
                                            .Replace(marketPlaceholder, Umbraco.TypedContent(contact.Market).Name)
                                           .Replace(datePlaceholder, date);
            return mailContent;

        }


    }
}
