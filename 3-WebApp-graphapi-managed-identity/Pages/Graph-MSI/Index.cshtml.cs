using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Azure.Identity;
using Microsoft.Graph.Core;
using System.Net.Http.Headers;
using WebApp_EasyAuth_DotNet.Models;

namespace WebApp_EasyAuth_DotNet.Pages.Graph_MSI
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;

        }

        public IList<MSGraphUser> Users { get; set; }
        public async Task OnGetAsync()
        {
            // Create the Graph service client with a DefaultAzureCredential which gets an access token using the available Managed Identity
            var credential = new DefaultAzureCredential();
            var token = credential.GetToken(
                new Azure.Core.TokenRequestContext(
                    new[] { "https://graph.microsoft.com/.default" }));

            var accessToken = token.Token;
            var graphServiceClient = new GraphServiceClient(
                new DelegateAuthenticationProvider((requestMessage) =>
                {
                    requestMessage
                    .Headers
                    .Authorization = new AuthenticationHeaderValue("bearer", accessToken);

                    return Task.CompletedTask;
                }));

            List<MSGraphUser> msGraphUsers = new List<MSGraphUser>();
            try
            {
                var users = await graphServiceClient.Users.Request().GetAsync();
                foreach (var u in users)
                {
                    MSGraphUser user = new MSGraphUser();
                    user.userPrincipalName = u.UserPrincipalName;
                    user.displayName = u.DisplayName;
                    user.mail = u.Mail;
                    user.jobTitle = u.JobTitle;

                    msGraphUsers.Add(user);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            Users = msGraphUsers;
        }
    }
}
