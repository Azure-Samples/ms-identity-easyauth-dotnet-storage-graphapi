using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp_EasyAuth_DotNet.Models;

namespace WebApp_EasyAuth_DotNet.Pages.Storage_MSI
{
    public class DeleteModel : PageModel
    {
        private readonly WebApp_EasyAuth_DotNet.Data.CommentsContext _context;
        public DeleteModel(WebApp_EasyAuth_DotNet.Data.CommentsContext context)
        {
            _context = context;
        }
        [BindProperty]
        public Comment Comment { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            List<Comment> comments = await _context.GetComments();
            Comment = comments.FirstOrDefault(m => m.Name == id);

            if (Comment == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            List<Comment> comments = await _context.GetComments();
            Comment = comments.FirstOrDefault(m => m.Name == id);

            if (Comment != null)
            {
                await _context.DeleteComment(Comment);
            }

            return RedirectToPage("./Index");
        }
    }
}
