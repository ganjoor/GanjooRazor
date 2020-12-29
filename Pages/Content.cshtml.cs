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

        public GanjoorPoetCompleteViewModel Cat { get; set; }

        public bool PoetPage { get; set; }

        public bool CatPage { get; set; }


        public async Task OnGet()
        {
            PoetPage = false;
            CatPage = false;
            using (HttpClient client = new HttpClient())
            {

                var response = await client.GetAsync($"{APIRoot.Url}/api/ganjoor/poets?includeBio=false");
                response.EnsureSuccessStatusCode();

                _poets = JArray.Parse(await response.Content.ReadAsStringAsync()).ToObject<List<GanjoorPoetViewModel>>();


                var poetQuery = await client.GetAsync($"{APIRoot.Url}/api/ganjoor/poet?url={Request.Path}");
                

                if(poetQuery.IsSuccessStatusCode)
                {
                    Cat = JObject.Parse(await poetQuery.Content.ReadAsStringAsync()).ToObject<GanjoorPoetCompleteViewModel>();
                    PoetPage = true;
                }
                else
                    if(poetQuery.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        var catQuery = await client.GetAsync($"{APIRoot.Url}/api/ganjoor/cat?url={Request.Path}");
                        if (catQuery.IsSuccessStatusCode)
                        {
                            Cat = JObject.Parse(await catQuery.Content.ReadAsStringAsync()).ToObject<GanjoorPoetCompleteViewModel>();
                            CatPage = true;
                        }
                    }

            }
        }
    }
}
