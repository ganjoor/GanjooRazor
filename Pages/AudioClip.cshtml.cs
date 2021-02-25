using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using RMuseum.Models.Ganjoor.ViewModels;
using RMuseum.Models.GanjoorAudio.ViewModels;

namespace GanjooRazor.Pages
{
    public class AudioClipModel : PageModel
    {

        /// <summary>
        /// recitation
        /// </summary>
        public PublicRecitationViewModel Recitation { get; set; }

        /// <summary>
        /// poem
        /// </summary>
        public GanjoorPoemCompleteViewModel Poem { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync($"{APIRoot.Url}/api/audio/published/{Request.Query["a"]}");
                response.EnsureSuccessStatusCode();

                Recitation = JsonConvert.DeserializeObject<PublicRecitationViewModel>(await response.Content.ReadAsStringAsync());

                var responsePoem = await client.GetAsync($"{APIRoot.Url}/api/ganjoor/poem/{Recitation.PoemId}");
                responsePoem.EnsureSuccessStatusCode();

                Poem = JsonConvert.DeserializeObject<GanjoorPoemCompleteViewModel>(await response.Content.ReadAsStringAsync());

            }

            return Page();

        }
    }
}
