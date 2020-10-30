using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp_EasyAuth_DotNet.Models
{
    public class MSGraphUser
    {
        public string displayName { get; set; }
        public string jobTitle { get; set; }
        public string mail { get; set; }
        public string userPrincipalName { get; set; }
        public string id { get; set; }
    }
}
