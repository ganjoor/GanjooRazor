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
        public GanjoorPageCompleteViewModel GanjoorPage { get; set; }


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
                GanjoorPage = JObject.Parse(await pageQuery.Content.ReadAsStringAsync()).ToObject<GanjoorPageCompleteViewModel>();
            }

            return Page();
        }
    }
}
