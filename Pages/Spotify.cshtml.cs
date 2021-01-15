using GSpotifyProxy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace GanjooRazor.Pages
{
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class SpotifyModel : PageModel
    {
        public void OnGet()
        {
            ViewData["p"] = Request.Query["p"];
        }

        
        public async Task<IActionResult> OnPostSearchByArtistNameAsync(string search)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = $"http://spotify.ganjoor.net/spotifyapi/search/artists/{HttpUtility.UrlEncode(search)}";
                var response = await client.GetAsync(url);

                NameIdUrlImage[] artists = new NameIdUrlImage[] { };

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    artists = JsonConvert.DeserializeObject<NameIdUrlImage[]>(await response.Content.ReadAsStringAsync());
                }


                return new PartialViewResult()
                {
                    ViewName = "SpotifySearchPartial",
                    ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                    {
                        Model = new SpotifySearchPartialModel()
                        {
                            Artists = artists
                        }
                    }
                };
            }
        }
    }
}
