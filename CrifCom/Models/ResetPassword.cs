using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Umbraco.Core;

namespace CrifCom.Models
{
    public class ResetPassword
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^.{8,}$", ErrorMessage = "Minimum 8 characters required.")]
        public string PassWord { get; set; }

        [Required]
        [Compare("PassWord")]
        public string ConfirmPassword { get; set; }

        public ResetPassword GetUserByEmail(string email)
        {
            var member = ApplicationContext.Current.Services.MemberService.GetByEmail(email);
            if (member != null)
            {
                ResetPassword resetPassword = new ResetPassword();
                resetPassword.Email = member.Email;
                return resetPassword;
            }
            return null;
        }
    }
}