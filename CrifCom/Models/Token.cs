using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CrifCom.Models
{
    public class Token
    {
        public int MemberId { get; set; }
        public string Email { get; set; }
        public DateTime ExpDate { get; set; }
    }
}