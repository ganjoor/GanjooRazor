using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using GanjooRazor.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using RMuseum.Models.Ganjoor;
using RMuseum.Models.Ganjoor.ViewModels;
using RMuseum.Models.MusicCatalogue.ViewModels;

namespace GanjooRazor.Pages
{
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class GolhaModel : PageModel
    {
        /// <summary>
        /// is logged on
        /// </summary>
        public bool LoggedIn { get; set; }

        /// <summary>
        /// PoemId
        /// </summary>
        public int PoemId { get; set; }

        /// <summary>
        /// Last Error
        /// </summary>
        public string LastError { get; set; }

        /// <summary>
        /// Post Success
        /// </summary>
        public bool PostSuccess { get; set; }

        /// <summary>
        /// Inserted song Id
        /// </summary>
        public int InsertedSongId { get; set; }

        /// <summary>
        /// suggested (unapproved) songs
        /// </summary>
        public PoemMusicTrackViewModel[] SuggestedSongs { get; set; }

        /// <summary>
        /// api model
        /// </summary>
        [BindProperty]
        public PoemMusicTrackViewModel PoemMusicTrackViewModel { get; set; }

        private async Task _GetSuggestedSongs(HttpClient client)
        {
            var response = await client.GetAsync($"{APIRoot.Url}/api/ganjoor/poem/{PoemId}/songs/?approved=false&trackType={(int)PoemMusicTrackType.Golha}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                SuggestedSongs = JsonConvert.DeserializeObject<PoemMusicTrackViewModel[]>(await response.Content.ReadAsStringAsync());
            }
            else
            {
                SuggestedSongs = new PoemMusicTrackViewModel[] { };
            }
        }

        public async Task OnGetAsync()
        {
            PostSuccess = false;
            LastError = "";
            LoggedIn = !string.IsNullOrEmpty(Request.Cookies["Token"]);
            InsertedSongId = 0;
            if (!string.IsNullOrEmpty(Request.Query["p"]))
            {
                PoemId = int.Parse(Request.Query["p"]);
            }
            else
            {
                PoemId = 0;
            }

            using (HttpClient client = new HttpClient())
            {
                await _GetSuggestedSongs(client);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            PostSuccess = false;
            LastError = "";
            LoggedIn = !string.IsNullOrEmpty(Request.Cookies["Token"]);
            PoemId = PoemMusicTrackViewModel.PoemId = int.Parse(Request.Query["p"]);
            PoemMusicTrackViewModel.TrackType = PoemMusicTrackType.Golha;
            InsertedSongId = 0;

            using (HttpClient client = new HttpClient())
            {
                if (await GanjoorSessionChecker.PrepareClient(client, Request, Response))
                {
                    var stringContent = new StringContent(JsonConvert.SerializeObject(PoemMusicTrackViewModel), Encoding.UTF8, "application/json");
                    var methodUrl = $"{APIRoot.Url}/api/ganjoor/song";
                    var response = await client.PostAsync(methodUrl, stringContent);
                    if (!response.IsSuccessStatusCode)
                    {
                        LastError = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        InsertedSongId = JsonConvert.DeserializeObject<PoemMusicTrackViewModel>(await response.Content.ReadAsStringAsync()).Id;

                        PostSuccess = true;
                    }
                }
                else
                {
                    LastError = "لطفا از گنجور خارج و مجددا به آن وارد شوید.";
                }
                

                await _GetSuggestedSongs(client);
            }

            return Page();
        }

        /// <summary>
        /// fill collection programs
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostFillProgramsAsync(int collection)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync($"{APIRoot.Url}/api/musiccatalogue/golha/collection/{collection}/programs");

                GolhaProgramViewModel[] programs = new GolhaProgramViewModel[] { };

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    programs = JsonConvert.DeserializeObject<GolhaProgramViewModel[]>(await response.Content.ReadAsStringAsync());
                }
                return new OkObjectResult(programs);
            }
        }

        /// <summary>
        /// fill program tracks
        /// </summary>
        /// <param name="program"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostFillTracksAsync(int program)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync($"{APIRoot.Url}/api/musiccatalogue/golha/program/{program}/tracks");

                GolhaTrackViewModel[] tracks = new GolhaTrackViewModel[] { };

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    tracks = JsonConvert.DeserializeObject<GolhaTrackViewModel[]>(await response.Content.ReadAsStringAsync());
                }
                return new OkObjectResult(tracks);
            }
        }
    }
}
