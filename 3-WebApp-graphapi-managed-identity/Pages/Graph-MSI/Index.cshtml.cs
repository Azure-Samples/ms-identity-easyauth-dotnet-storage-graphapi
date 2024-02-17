using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp_EasyAuth_DotNet.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Azure.Identity;


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
            // Create the Graph service client with a ChainedTokenCredential which gets an access
            // token using the available Managed Identity or environment variables if running
            // in development.
            var credential = new ChainedTokenCredential(
                new ManagedIdentityCredential(),
                new EnvironmentCredential());

            string[] scopes = new[] { "https://graph.microsoft.com/.default" };

            var graphServiceClient = new GraphServiceClient(
                credential, scopes);

            List<MSGraphUser> msGraphUsers = new List<MSGraphUser>();
            try
            {
                //var users = await graphServiceClient.Users.Request().GetAsync();
                var users = await graphServiceClient.Users.GetAsync();
                foreach (var u in users.Value)
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
