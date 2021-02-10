using GanjooRazor.Models.MuseumLink;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GanjooRazor.Pages
{
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class PinModel : PageModel
    {
        /// <summary>
        /// related image suggestion model
        /// </summary>
        [BindProperty]
        public RelatedImageSuggestionModel RelatedImageSuggestionModel { get; set; }
       
        public void OnGet()
        {
            RelatedImageSuggestionModel = new RelatedImageSuggestionModel();
        }
    }
}
