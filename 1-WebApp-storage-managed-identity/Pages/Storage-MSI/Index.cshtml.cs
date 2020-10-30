using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp_EasyAuth_DotNet.Models;

namespace WebApp_EasyAuth_DotNet.Pages.Storage_MSI
{
    public class IndexModel : PageModel
    {
        private readonly WebApp_EasyAuth_DotNet.Data.CommentsContext _context;
        public IndexModel(WebApp_EasyAuth_DotNet.Data.CommentsContext context)
        {
            _context = context;
        }

        public IList<Comment> Comments { get; set; }
        public async Task OnGetAsync()
        {
            Comments = await _context.GetComments();
        }
    }
}
