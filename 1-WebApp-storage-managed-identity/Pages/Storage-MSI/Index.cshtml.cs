using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp_EasyAuth_DotNet.Models;
using System.Collections.Generic;

namespace WebApp_EasyAuth_DotNet.Pages.Storage_MSI
{
    public class IndexModel : PageModel
    {
        private readonly WebApp_EasyAuth_DotNet.Data.CommentsContext _context;
        public IndexModel(WebApp_EasyAuth_DotNet.Data.CommentsContext context)
        {
            _context = context;
        }

        public IList<Comment>? Comments { get; set; }
        public async Task OnGetAsync()
        {
            Comments = await _context.GetComments();
        }
    }
}
