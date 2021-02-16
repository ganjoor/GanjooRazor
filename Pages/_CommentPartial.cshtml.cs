using Microsoft.AspNetCore.Mvc.RazorPages;
using RMuseum.Models.Ganjoor.ViewModels;

namespace GanjooRazor.Pages
{
    public class _CommentPartialModel : PageModel
    {
        public GanjoorCommentSummaryViewModel Comment { get; set; }

        public string Error { get; set; }
        public void OnGet()
        {
        }
    }
}
