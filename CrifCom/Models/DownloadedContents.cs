using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.DataLayer;
using Umbraco.Core.Persistence;

namespace CrifCom.Models
{
    [TableName("DownloadedContents")]
    [PrimaryKey("Id", autoIncrement = true)]
    public class DownloadedContents
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string UserRole { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string FormId { get; set; }
        public string ClientIP { get; set; }
        public string Device { get; set; }
        public string Browser { get; set; }
        public string UserAgent { get; set; } 
        public string DevicePlatform { get; set; }
        public string contentUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public int IsProduct { get; set; }
        public int IsSuccessStory { get; set; }
    }
}