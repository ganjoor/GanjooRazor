using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using RMuseum.Models.Ganjoor.ViewModels;

namespace GanjooRazor.Pages
{
    public class ContentModel : PageModel
    {
        private List<GanjoorPoetViewModel> _poets;
        public List<GanjoorPoetViewModel> Poets { get { return _poets; } }

        public GanjoorPoetCompleteViewModel Poet { get; set; }

        public async Task OnGet()
        {
            using (HttpClient client = new HttpClient())
            {

                var poetQuery = await client.GetAsync($"{APIRoot.Url}/api/ganjoor/poet?url={Request.Path}");
                poetQuery.EnsureSuccessStatusCode();

                Poet = JObject.Parse(await poetQuery.Content.ReadAsStringAsync()).ToObject<GanjoorPoetCompleteViewModel>();

                var response = await client.GetAsync($"{APIRoot.Url}/api/ganjoor/poets?includeBio=false");
                response.EnsureSuccessStatusCode();

                _poets = JArray.Parse(await response.Content.ReadAsStringAsync()).ToObject<List<GanjoorPoetViewModel>>();
            }
        }
    }
}
