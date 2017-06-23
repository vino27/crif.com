using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CrifCom.Models
{
    public class Login
    {
        [Required]
        [RegularExpression("^[^<>,<|>]+$", ErrorMessage = "you are not allowed to insert html in this field.")]
        public string UserName { get; set; }

        [Required]
        [RegularExpression("^[^<>,<|>]+$", ErrorMessage = "you are not allowed to insert html in this field.")]
        public string PassWord { get; set; }
    }
}