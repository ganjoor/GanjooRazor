using GanjooRazor.Utils;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RMuseum.Models.Auth.Memory;

namespace GanjooRazor.Areas.Panel.Pages
{
    public class IndexModel : PageModel
    {
        /// <summary>
        /// get
        /// </summary>
        public void OnGet()
        {
            ViewData["ganjoor-import"] = GanjoorSessionChecker.IsPermitted(Request, RMuseumSecurableItem.GanjoorEntityShortName, RMuseumSecurableItem.ImportOperationShortName);
        }
    }
}
