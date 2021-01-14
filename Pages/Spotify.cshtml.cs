using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GanjooRazor.Pages
{
    public class SpotifyModel : PageModel
    {
        public void OnGet()
        {
            ViewData["p"] = Request.Query["p"];
        }
    }
}
