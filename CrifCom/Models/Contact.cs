using CrifCom.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using umbraco.DataLayer;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using umbraco.presentation.nodeFactory;

namespace CrifCom.Models
{
    [TableName("contactUs")]
    [PrimaryKey("Id", autoIncrement = true)]
    public class Contact
    {

       
        public int Id { get; set; }

        [Required]
        [RegularExpression("^[^<>,<|>]+$", ErrorMessage = "you are not allowed to insert html in this field.")]
        public string Name { get; set; }

        [Required]
        [RegularExpression("^[^<>,<|>]+$", ErrorMessage = "you are not allowed to insert html in this field.")]
        public string Surname { get; set; }

        [Required]
        [RegularExpression("^[^<>,<|>]+$", ErrorMessage = "you are not allowed to insert html in this field.")]
        public string Company { get; set; }
        [Required]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Invalid Email.")]
        public string Email { get; set; }

        [Required]
        [RegularExpression("^[^<>,<|>]+$", ErrorMessage = "you are not allowed to insert html in this field.")]
        public string Telephone { get; set; }

       [RegularExpression("^[^<>,<|>]+$", ErrorMessage = "you are not allowed to insert html in this field.")]
        public string Role { get; set; }
     
        public string Market { get; set; }

        [Required]
        [RegularExpression("^[^<>,<|>]+$", ErrorMessage = "you are not allowed to insert html in this field.")]
        public string Nation { get; set; }

        [RegularExpression("^[^<><|>]+$", ErrorMessage = "you are not allowed to insert html in this field.")]
        public string Message { get; set; }

        public string RequestType { get; set; }
           
        public int IsProduct { get; set; }
        [Ignore]
        //[Mandatory]                       
        [Required]
        [Range(typeof(bool), "true", "true")]
        public bool IsPrivacyChecked { get; set; }
        public int FormId { get; set; }
        public string ClientIP { get; set; }
        public string Device { get; set; }
        public string Browser { get; set; }
        public string UserAgent { get; set; }
        public string DevicePlatform { get; set; }        
        public DateTime CreatedDate { get; set; }
       
    }
}