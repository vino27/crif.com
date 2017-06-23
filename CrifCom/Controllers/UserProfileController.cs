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
using CrifCom.ViewModels;
using CrifCom.Models;

namespace CrifCom.Controllers
{
    public class UserProfileController : SurfaceController
    {
        // GET: UserProfile
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ForgotPassword(ForgotPassword forgotPassword)
        {
            try
            {
                // var forgotPasswordnode = Umbraco.TypedContent(forgotPassword.NodeId);
                if (ModelState.IsValid && forgotPassword != null)
                {
                    Node resetPage = new Node(int.Parse(umbraco.library.GetDictionaryItem("ResetPasswordNodeId")));
                    var member = ApplicationContext.Services.MemberService.GetByUsername(forgotPassword.Email);
                    if (member != null)
                    {
                        string resetPasswordLinkPlaceHolder = "[PASSWORDRESETLINK]";
                        string namePlaceHolder = "[NAME]";
                        string[] roles = System.Web.Security.Roles.GetRolesForUser(member.Username);
                        if (roles != null && roles.Contains("ApprovedUsers"))
                        {
                            var forgotPasswordNode = Umbraco.TypedContent(Umbraco.GetDictionaryValue("ForgotPasswordNodeId"));
                            if (forgotPasswordNode != null && !string.IsNullOrEmpty(forgotPassword.Email))
                            {
                                string from = forgotPasswordNode.GetProperty("senderEmailAutoReply").Value.ToSafeString();
                                string emailSubject = forgotPasswordNode.GetProperty("replyEmailSubject").Value.ToSafeString();
                                string emailBody = forgotPasswordNode.GetProperty("replyEmailBody").Value.ToSafeString();
                                if (!string.IsNullOrEmpty(emailBody))
                                {
                                    Token token = new Token();
                                    var memberObjrct = ApplicationContext.Services.MemberService.GetByUsername(forgotPassword.Email);
                                    token.Email = memberObjrct.Email.Trim();
                                    token.MemberId = memberObjrct.Id;
                                    token.ExpDate = DateTime.Now.AddDays(2);
                                    string tokeString = Newtonsoft.Json.JsonConvert.SerializeObject(token);
                                    string encryptedMailId = Utility.ToBase64Url(Cryptography.Encrypt(tokeString, ConfigurationManager.AppSettings["EncryptionPassPhrase"].ToString()));
                                    //string encryptedMailId = Cryptography.Encrypt(forgotPassword.Email, ConfigurationManager.AppSettings["EncryptionPassPhrase"].ToString());
                                    var homeNode = Umbraco.TypedContent(forgotPasswordNode.Id).AncestorOrSelf(1);
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
                                    //string url = Request.Url.GetLeftPart(UriPartial.Authority);

                                    //string encryptedDate = Cryptography.Encrypt(DateTime.Now.AddDays(1).ToString().Replace(" ", "-"), ConfigurationManager.AppSettings["EncryptionPassPhrase"].ToString());
                                    string resetLink = "<a href='" + url + "/umbraco/Surface/UserProfile/ResetForgotPassword?id=" + HttpUtility.UrlEncode(encryptedMailId) + "&node=" + resetPage.Id.ToString() + "'>Reset Password</a>";
                                    emailBody = emailBody.Replace(resetPasswordLinkPlaceHolder, resetLink)
                                                            .Replace(namePlaceHolder, member.Name);
                                    emailBody = emailBody.Replace("\n", "<br />");
                                }
                                Email.SendMail(emailSubject, emailBody, forgotPassword.Email.Split(',').ToList(), Enumerable.Empty<string>(), Enumerable.Empty<string>(), from,null, Enumerable.Empty<string>(), true);
                            
                                forgotPassword.Email = "";
                                return new JsonNetResult { Data = new { Status = "Success", Message = "" } };
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
            }
            catch (Exception ex)
            {

                return new JsonNetResult { Data = new { Status = "Failed", Message = "" } };
            }


            return RedirectToCurrentUmbracoPage();
        }
        public ActionResult ResetForgotPassword(string id, string g, string node)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    string stringText = Utility.FromBase64Url(HttpUtility.UrlDecode(id));
                    string emailId = Cryptography.Decrypt(stringText, ConfigurationManager.AppSettings["EncryptionPassPhrase"].ToString());
                    Token token = Newtonsoft.Json.JsonConvert.DeserializeObject<Token>(emailId);
                    var membership = ApplicationContext.Services.MemberService.GetByUsername(token.Email);
                    //string emailId = Utility.Decrypt(HttpUtility.UrlDecode(id), ConfigurationManager.AppSettings["EncryptionPassPhrase"].ToString());
                    //var membership = ApplicationContext.Services.MemberService.GetByUsername(emailId);
                    if (membership == null)
                    {
                        id = null;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "[RegistrationController][ResetPassword]- Invalid Id(" + id + ")" + ex.Source, ex);
                id = null;
            }
            //
            var resetNodeId = node;
            return RedirectToUmbracoPage(int.Parse(resetNodeId), string.Format("id={0}", id));

        }

        public ActionResult ResetPassword(ResetPassword ResetPassword)
        {

            try
            {
                int memberId = 0;
                if (ResetPassword.Email != null)
                {
                    string stringText = Utility.FromBase64Url(HttpUtility.UrlDecode(ResetPassword.Email));
                    string emailId = Cryptography.Decrypt(stringText, ConfigurationManager.AppSettings["EncryptionPassPhrase"].ToString());
                    Token token = Newtonsoft.Json.JsonConvert.DeserializeObject<Token>(emailId);

                    memberId = token.MemberId;
                }
                if (ResetPassword != null && ModelState.IsValid && memberId !=0)
                {
                    if (ResetPassword.PassWord == ResetPassword.ConfirmPassword)
                    {
                        var member = ApplicationContext.Services.MemberService.GetById(memberId);
                        if (member != null)
                        {
                            ApplicationContext.Services.MemberService.SavePassword(member, ResetPassword.PassWord);
                            // ViewBag.ResetPasswordSuccessMessage = node.GetProperty("ResetSuccess").Value;
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
            }
            catch (Exception ex)
            {
                LogHelper.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "[RegistrationController][ActivateAccount]-" + ex.Source, ex);
                return new JsonNetResult { Data = new { Status = "Failed", Message = "" } };
            }
            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => x.Key);
            ResetPassword.PassWord = "";
            ResetPassword.ConfirmPassword = "";
            return CurrentUmbracoPage();
        }

        [HttpPost]
        [ValidateAntiForgeryTokenAttribute]
        public ActionResult UpdateUser(RegistrationViewModel existingMember)
        {
            Node currentNode = Node.GetCurrent();
            try
            {
                if (Membership.ValidateUser(existingMember.Email, existingMember.Password))
                {
                    var member = ApplicationContext.Services.MemberService.GetByEmail(existingMember.Email);
                    if (member != null)
                    {
                        member.SetValue("memberName", existingMember.Name);
                        member.SetValue("surname", existingMember.Surname);
                        member.SetValue("role", existingMember.Role);
                        member.SetValue("company", existingMember.Company);
                        member.SetValue("market", existingMember.Market);
                        member.SetValue("nation", existingMember.Nation);
                        member.IsLockedOut = false;
                        ApplicationContext.Services.MemberService.Save(member);
                        ApplicationContext.Services.MemberService.SavePassword(member, existingMember.Password);
                    }

                    if (currentNode.GetProperty("replyToUser") != null && !string.IsNullOrEmpty(currentNode.GetProperty("replyToUser").Value) && currentNode.GetProperty("replyToUser").Value == "1")
                    {
                        if (currentNode.GetProperty("fromEmail") != null && !string.IsNullOrEmpty(currentNode.GetProperty("fromEmail").Value))
                        {
                            string fromEmail = currentNode.GetProperty("fromEmail").Value.ToString();
                            string replyMesssage = "", replyemailSubject = "", date = DateTime.Now.Date.ToString("dd MMM yyyy");
                            string datePlaceholder = "[DATE]";

                            if (currentNode.GetProperty("replyEmailSubject") != null && !string.IsNullOrEmpty(currentNode.GetProperty("replyEmailSubject").Value.Trim()))
                            {
                                replyemailSubject = currentNode.GetProperty("replyEmailSubject").Value.Trim();
                                replyemailSubject = replyemailSubject.Replace(datePlaceholder, date);
                            }
                            else
                            {
                                replyemailSubject = "New email contact - " + date;
                            }

                            if (currentNode.GetProperty("emailReplyBody") != null && !string.IsNullOrEmpty(currentNode.GetProperty("emailReplyBody").Value.Trim()))
                            {
                                replyMesssage = currentNode.GetProperty("emailReplyBody").Value.Trim();
                            }
                            else
                            {
                                replyMesssage = @"<p>Hi,</p>
                            <p>There is a new email from the Crif Italia.</p>
                            <p>You have successfully Updated your profile.</p>                            
                            <p> </p>
                            <p> </p>";
                            }

                            replyMesssage = replyMesssage.Replace(datePlaceholder, date);

                            Email.SendMail(replyemailSubject, replyMesssage, existingMember.Email.Split(',').ToList(), Enumerable.Empty<string>(), Enumerable.Empty<string>(),null,null, Enumerable.Empty<string>(),true);

                        }

                    }

                    TempData["sucessMessage"] = currentNode.GetProperty("successMessage") != null ? currentNode.GetProperty("successMessage").Value : "You have successfully changed you password.";
                    return new JsonNetResult { Data = new { Status = "Success", Message = "" } };
                }
                else
                {
                    TempData["failureMessage"] = currentNode.GetProperty("authenticationFailureMessage") != null ? currentNode.GetProperty("authenticationFailureMessage").Value : "Authentication failed. You have entered wrong password.";
                    return new JsonNetResult { Data = new { Status = "failure", Message = "" } };
                }

            }
            catch (Exception ex)
            {
                TempData["failureMessage"] = currentNode.GetProperty("failureMessage") != null ? currentNode.GetProperty("failureMessage").Value : "Something went wrong. Please try again.";
                return new JsonNetResult { Data = new { Status = "failure", Message = "" } };
            }
            //return RedirectToCurrentUmbracoPage();
        }

        [HttpPost]
        public JsonResult ChangePassword(ResetPassword resetPassword)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var member = ApplicationContext.Services.MemberService.GetByUsername(resetPassword.Email);
                    if (member != null)
                    {
                        ApplicationContext.Services.MemberService.SavePassword(member, resetPassword.PassWord);
                        return new JsonResult { Data = new { Status = "Success", Message = "" } };
                    }
                    else
                    {
                        return new JsonResult { Data = new { Status = "Error", Message = "" } };
                    }

                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "[RegistrationController][ActivateAccount]-" + ex.Source, ex);
            }
            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => x.Key);
            return new JsonResult { Data = new { Status = "ValidationError", Message = errors } };
        }


    }
}