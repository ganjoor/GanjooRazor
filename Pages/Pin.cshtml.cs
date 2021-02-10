using GanjooRazor.Models.MuseumLink;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using RMuseum.Models.GanjoorIntegration.ViewModels;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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

        /// <summary>
        /// «—”«· „Ê›ﬁ
        /// </summary>
        public bool Succeeded { get; set; }

        /// <summary>
        /// Œÿ«
        /// </summary>
        public string LastError { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            Succeeded = false;
            LastError = "";
            if (Request.Query["final"] == "1")
            {
                using (HttpClient client = new HttpClient())
                {
                    PinterestLinkViewModel model = new PinterestLinkViewModel()
                    {
                        GanjoorPostId = RelatedImageSuggestionModel.PoemId,
                        GanjoorUrl = $"https://ganjoor.net{RelatedImageSuggestionModel.GanjoorUrl}",
                        GanjoorTitle = RelatedImageSuggestionModel.GanjoorTitle,
                        AltText = RelatedImageSuggestionModel.AltText,
                        LinkType = RMuseum.Models.GanjoorIntegration.LinkType.Pinterest,
                        PinterestUrl = RelatedImageSuggestionModel.PinterestUrl,
                        PinterestImageUrl = RelatedImageSuggestionModel.PinterestImageUrl
                    };
                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync($"{APIRoot.Url}/api/artifacts/pinterest", stringContent);

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        LastError = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        Succeeded = true;
                    }
                }
            }
            return Page();
        }
    }
}
