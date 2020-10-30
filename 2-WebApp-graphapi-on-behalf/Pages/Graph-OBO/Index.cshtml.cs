using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph;
using System.Net.Http.Headers;
using System.IO;

namespace WebApp_EasyAuth_DotNet.Pages.Graph_OBO
{
    public class IndexModel : PageModel
    {
        public async Task OnGetAsync()
        {
            string accessToken = Request.Headers["X-MS-TOKEN-AAD-ACCESS-TOKEN"];

            try
            {

                var graphServiceClient = new GraphServiceClient(
                new DelegateAuthenticationProvider((requestMessage) =>
                {
                    requestMessage
                    .Headers
                    .Authorization = new AuthenticationHeaderValue("bearer", accessToken);

                    return Task.CompletedTask;
                }));

                var user = await graphServiceClient.Me.Request().GetAsync();
                ViewData["Me"] = user;
                ViewData["name"] = user.DisplayName;

                using (var photoStream = await graphServiceClient.Me.Photo.Content.Request().GetAsync())
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

