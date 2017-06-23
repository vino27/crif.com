using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CrifCom.Models
{
    public class Registration
    {
        [Required]
        [RegularExpression("^[^<>,<|>]+$", ErrorMessage = "you are not allowed to insert html in this field.")]
        public string Name { get; set; }

        [Required]
        [RegularExpression("^[^<>,<|>]+$", ErrorMessage = "you are not allowed to insert html in this field.")]
        public string Surname { get; set; }

        [Required]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Invalid Email.")]
        public string Email { get; set; }

        [Required]
        [RegularExpression("^[^<>,<|>]+$", ErrorMessage = "you are not allowed to insert html in this field.")]
        public string Company { get; set; }
        [Required]
        [RegularExpression("^[^<>,<|>]+$", ErrorMessage = "you are not allowed to insert html in this field.")]
        public string Telephone { get; set; }

        [RegularExpression("^[^<>,<|>]+$", ErrorMessage = "you are not allowed to insert html in this field.")]
        public string Role { get; set; }

        [Required]
        public string Market { get; set; }

        [Required]
        [RegularExpression(@"^.{8,}$", ErrorMessage = "Minimum 8 characters required.")]
        public string Password { get; set; }

        [Required]
        [RegularExpression("^[^<>,<|>]+$", ErrorMessage = "you are not allowed to insert html in this field.")]
        public string Nation { get; set; }

        [Required]
        [Compare("Password")]
        public string PasswordConfirmation { get; set; }
        [Required]
        public bool IsPrivacyChecked { get; set; }

    }
}