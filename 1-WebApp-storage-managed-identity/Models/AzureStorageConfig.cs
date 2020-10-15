using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp_EasyAuth_DotNet.Models
{
    public class AzureStorageConfig
    {
        public string AccountName { get; set; }
        public string ContainerName { get; set; }
        public string ConnectionString { get; set; }
    }
}
