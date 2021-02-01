using GanjooRazor.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using RMuseum.Models.Ganjoor.ViewModels;
using RMuseum.Services.Implementation.ImportedFromDesktopGanjoor;
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
        /// poem model
        /// </summary>
        public GanjoorPoemCompleteViewModel Poem { get; set; }

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
                    var trackResponse = await client.GetAsync($"{APIRoot.Url}/api/ganjoor/song?skip=0&onlyMine=false");
                    if (!trackResponse.IsSuccessStatusCode)
                    {
                        LastError = await trackResponse.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        PoemMusicTrackViewModel = JsonConvert.DeserializeObject<PoemMusicTrackViewModel>(await trackResponse.Content.ReadAsStringAsync());

                        PoemMusicTrackViewModel.ArtistName = GPersianTextSync.Sync(PoemMusicTrackViewModel.ArtistName);
                        PoemMusicTrackViewModel.AlbumName = GPersianTextSync.Sync(PoemMusicTrackViewModel.AlbumName);
                        PoemMusicTrackViewModel.TrackName = GPersianTextSync.Sync(PoemMusicTrackViewModel.TrackName);


                        var poemResponse = await client.GetAsync($"{APIRoot.Url}/api/ganjoor/poem/{PoemMusicTrackViewModel.PoemId}?catInfo=false&rhymes=false&recitations=false&images=false&songs=false&comments=false&verseDetails=false&navigation=false");
                        if (poemResponse.IsSuccessStatusCode)
                        {
                            Poem = JsonConvert.DeserializeObject<GanjoorPoemCompleteViewModel>(await poemResponse.Content.ReadAsStringAsync());
                        }
                        else
                        {
                            LastError = "لطفا از گنجور خارج و مجددا به آن وارد شوید.";
                        }
                    }
                }
                else
                {
                    LastError = "لطفا از گنجور خارج و مجددا به آن وارد شوید.";
                }

            }
        }


        public async Task<IActionResult> OnPostAsync()
        {
            PoemMusicTrackViewModel.Approved = Request.Form["approve"].Count == 1;

            await OnGetAsync();

            return Page();
        }
    }
}
