using GanjooRazor.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using RMuseum.Models.Ganjoor.ViewModels;
using System.Net.Http;
using System.Threading.Tasks;

namespace GanjooRazor.Areas.User.Pages
{
    public class ReviewSongsModel : PageModel
    {
        /// <summary>
        /// Last Error
        /// </summary>
        public string LastError { get; set; }

        /// <summary>
        /// api model
        /// </summary>
        [BindProperty]
        public PoemMusicTrackViewModel PoemMusicTrackViewModel { get; set; }

        /// <summary>
        /// get
        /// </summary>
        public async Task OnGetAsync()
        {
            LastError = "";
            using (HttpClient client = new HttpClient())
            {
                if (await GanjoorSessionChecker.PrepareClient(client, Request, Response))
                {
                    var methodUrl = $"{APIRoot.Url}/api/ganjoor/song?skip=0&onlyMine=false";
                    var response = await client.GetAsync(methodUrl);
                    if (!response.IsSuccessStatusCode)
                    {
                        LastError = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        PoemMusicTrackViewModel = JsonConvert.DeserializeObject<PoemMusicTrackViewModel>(await response.Content.ReadAsStringAsync());
                    }
                }
                else
                {
                    LastError = "لطفا از گنجور خارج و مجددا به آن وارد شوید.";
                }

            }
        }
    }
}
