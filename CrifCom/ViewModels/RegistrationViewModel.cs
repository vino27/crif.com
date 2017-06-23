using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Umbraco.Core;

namespace CrifCom.ViewModels
{
    public class RegistrationViewModel
    {
        [Required]
        [RegularExpression("^[^<>,<|>]+$", ErrorMessage = "you are not allowed to insert html in this field.")]
        public string Name { get; set; }

        [Required]
        [RegularExpression("^[^<>,<|>]+$", ErrorMessage = "you are not allowed to insert html in this field.")]
        public string Surname { get; set; }

        [Required]
        [RegularExpression("^[^<>,<|>]+$", ErrorMessage = "you are not allowed to insert html in this field.")]
        public string Email { get; set; }

        [Required]
        [RegularExpression("^[^<>,<|>]+$", ErrorMessage = "you are not allowed to insert html in this field.")]
        public string Company { get; set; }

        [RegularExpression("^[^<>,<|>]+$", ErrorMessage = "you are not allowed to insert html in this field.")]
        public string Role { get; set; }

        [Required]
        public string Market { get; set; }

        [Required]
        [RegularExpression("^[^<>,<|>]+$", ErrorMessage = "you are not allowed to insert html in this field.")]
        public string Nation { get; set; }

        [Required]
        [RegularExpression(@"^.{8,}$", ErrorMessage = "Minimum 8 characters required.")]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string PasswordConfirmation { get; set; }

        [Required]
        public bool IsPrivacyChecked { get; set; }

        public RegistrationViewModel GetUserByEmail(string email)
        {
            var member = ApplicationContext.Current.Services.MemberService.GetByEmail(email);
            if (member != null)
            {
                RegistrationViewModel registrationViewModel = new RegistrationViewModel();
                registrationViewModel.Name = member.GetValue<string>("memberName");
                registrationViewModel.Surname = member.GetValue<string>("surname");
                registrationViewModel.Company = member.GetValue<string>("company");
                registrationViewModel.Role = member.GetValue<string>("role");
                registrationViewModel.Market = member.GetValue<string>("market");
                registrationViewModel.Email = member.Email;
                registrationViewModel.Nation = member.GetValue<string>("nation");
                return registrationViewModel;
            }
            return null;
        }
    }
}