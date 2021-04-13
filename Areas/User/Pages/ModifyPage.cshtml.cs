using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RMuseum.Models.Ganjoor.ViewModels;
using System.Net.Http;
using System.Threading.Tasks;

namespace GanjooRazor.Areas.User.Pages
{
    public class ModifyPageModel : PageModel
    {

        /// <summary>
        /// Corresponding Ganojoor Page
        /// </summary>
        /// <summary>
        /// api model
        /// </summary>
        [BindProperty]
        public GanjoorModifyPageViewModel ModifyModel { get; set; }

        /// <summary>
        /// page id
        /// </summary>
        public int PageId { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            if(string.IsNullOrEmpty(Request.Query["id"]))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "شناسهٔ صفحه مشخص نیست.");
            }
            using (HttpClient client = new HttpClient())
            {
                var pageUrlResponse = await client.GetAsync($"{APIRoot.Url}/api/ganjoor/pageurl?id={Request.Query["id"]}");

                if(!pageUrlResponse.IsSuccessStatusCode)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, await pageUrlResponse.Content.ReadAsStringAsync());
                }

                pageUrlResponse.EnsureSuccessStatusCode();

                var pageUrl = JsonConvert.DeserializeObject<string>(await pageUrlResponse.Content.ReadAsStringAsync());

                var pageQuery = await client.GetAsync($"{APIRoot.Url}/api/ganjoor/page?url={pageUrl}");
                if (!pageQuery.IsSuccessStatusCode)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, await pageQuery.Content.ReadAsStringAsync());
                }
                GanjoorPageCompleteViewModel page = JObject.Parse(await pageQuery.Content.ReadAsStringAsync()).ToObject<GanjoorPageCompleteViewModel>();

                ModifyModel = new GanjoorModifyPageViewModel()
                {
                    Title = page.Title,
                    UrlSlug = page.UrlSlug,
                    HtmlText = page.HtmlText,
                    Note = "",
                    SourceName = page.Poem == null ? null : page.Poem.SourceName,
                    SourceUrlSlug = page.Poem == null ? null : page.Poem.SourceUrlSlug,
                    OldTag = page.Poem == null ? null : page.Poem.OldTag,
                    OldTagPageUrl = page.Poem == null ? null : page.Poem.OldTagPageUrl,
                    RhymeLetters = page.Poem == null ? null : page.Poem.RhymeLetters,
                    Rhythm = page.Poem == null ? null : page.Poem.GanjoorMetre == null ? null : page.Poem.GanjoorMetre.Rhythm
                };

            }

            return Page();
        }
    }
}
