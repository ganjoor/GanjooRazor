using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RMuseum.Models.Ganjoor.ViewModels;

namespace GanjooRazor.Areas.User.Pages
{
    public class AddSongModel : PageModel
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
        /// poem id
        /// </summary>
        public int PoemId { get; set; }

        public void OnGet()
        {
            PoemMusicTrackViewModel = new PoemMusicTrackViewModel()
            {
                TrackType = RMuseum.Models.Ganjoor.PoemMusicTrackType.BeepTunesOrKhosousi,
                PoemId = 0,
                ArtistName = "محمدرضا شجریان",
                ArtistUrl = "http://beeptunes.com/artist/3403349",
                AlbumName = "",
                AlbumUrl = "",
                TrackName = "",
                TrackUrl = "",
                Approved = false,
                Rejected = false,
                BrokenLink = false,
                Description = ""
            };
        }
    }
}
