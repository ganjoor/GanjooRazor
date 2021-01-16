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
        /// <summary>
        /// poem id
        /// </summary>
        public int PoemId { get; set; }

        public void OnGet()
        {
            PoemId = int.Parse(Request.Query["p"]);
        }

        
        /// <summary>
        /// search by artists name
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostSearchByArtistNameAsync(string search)
        {
            using (HttpClient client = new HttpClient())
            {
                //Warning: This is a private wrapper around the spotify API, created only for this project and incapable of
                //         responding large number of requests (both server and Spotify user limitations),
                //         so please do not use this proxy in other projects because you will cause this proxy to become unavailable for me
                //         Thanks!
                var response = await client.GetAsync($"http://spotify.ganjoor.net/spotifyapi/search/artists/{HttpUtility.UrlEncode(search)}");

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

        /// <summary>
        /// fill artist albums
        /// </summary>
        /// <param name="artist">is an ID and consists of numeric and non-numeric characters</param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostFillAlbumsAsync(string artist)
        {
            using (HttpClient client = new HttpClient())
            {
                //Warning: This is a private wrapper around the spotify API, created only for this project and incapable of
                //         responding large number of requests (both server and Spotify user limitations),
                //         so please do not use this proxy in other projects because you will cause this proxy to become unavailable for me
                //         Thanks!
                var response = await client.GetAsync($"http://spotify.ganjoor.net/spotifyapi/artists/{artist}/albums");

                NameIdUrlImage[] albums = new NameIdUrlImage[] { };

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    albums = JsonConvert.DeserializeObject<NameIdUrlImage[]>(await response.Content.ReadAsStringAsync());
                }
                return new OkObjectResult(albums);
            }
        }

        /// <summary>
        /// fill album tracks
        /// </summary>
        /// <param name="album">is an ID and consists of numeric and non-numeric characters</param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostFillTracksAsync(string album)
        {
            using (HttpClient client = new HttpClient())
            {
                //Warning: This is a private wrapper around the spotify API, created only for this project and incapable of
                //         responding large number of requests (both server and Spotify user limitations),
                //         so please do not use this proxy in other projects because you will cause this proxy to become unavailable for me
                //         Thanks!
                var response = await client.GetAsync($"http://spotify.ganjoor.net/spotifyapi/albums/{album}/tracks");

                NameIdUrlImage[] tracks = new NameIdUrlImage[] { };

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    tracks = JsonConvert.DeserializeObject<NameIdUrlImage[]>(await response.Content.ReadAsStringAsync());
                }
                return new OkObjectResult(tracks);
            }
        }
    }
}
