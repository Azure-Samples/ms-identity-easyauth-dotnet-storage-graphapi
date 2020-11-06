using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph;
using System.Net.Http.Headers;
using System.IO;
using Microsoft.Identity.Web;
using Microsoft.Extensions.Logging;

namespace WebApp_EasyAuth_DotNet.Pages.Graph_OBO
{
    [AuthorizeForScopes(Scopes = new[] { "user.read" })]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly GraphServiceClient _graphServiceClient;

        public IndexModel(ILogger<IndexModel> logger, GraphServiceClient graphServiceClient)
        {
            _logger = logger;
            _graphServiceClient = graphServiceClient;
        }

        public async Task OnGetAsync()
        {
            try
            {
                var user = await _graphServiceClient.Me.Request().GetAsync();
                ViewData["Me"] = user;
                ViewData["name"] = user.DisplayName;

                using (var photoStream = await _graphServiceClient.Me.Photo.Content.Request().GetAsync())
                {
                    byte[] photoByte = ((MemoryStream)photoStream).ToArray();
                    ViewData["photo"] = Convert.ToBase64String(photoByte);
                }
            }
            catch (Exception ex)
            {
                ViewData["photo"] = null;
            }
        }
    }

}

