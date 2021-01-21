using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using GanjooRazor.Utils;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using RMuseum.Models.Ganjoor.ViewModels;
using RMuseum.Models.GanjoorAudio.ViewModels;

namespace GanjooRazor.Pages
{
    public class ContentModel : PageModel
    {
        private List<GanjoorPoetViewModel> _poets;
        public List<GanjoorPoetViewModel> Poets { get { return _poets; } }

        public GanjoorPageCompleteViewModel GanjoorPage { get; set; }

        public bool PoetPage { get; set; }

        public bool CatPage { get; set; }

        public bool PoemPage { get; set; }

        /// <summary>
        /// is logged on
        /// </summary>
        public bool LoggedIn { get; set; }

        private void _preparePoemExcerpt(GanjoorPoemSummaryViewModel poem)
        {
            if(poem == null)
            {
                return;
            }
            if (poem.Excerpt.Length > 100)
            {
                poem.Excerpt = poem.Excerpt.Substring(0, 50);
                int n = poem.Excerpt.LastIndexOf(' ');
                if (n >= 0)
                {
                    poem.Excerpt = poem.Excerpt.Substring(0, n) + " ...";
                }
                else
                {
                    poem.Excerpt += "...";
                }
            }
        }

        public string getAudioDesc(PublicRecitationViewModel recitation, bool contributionLink = false)
        {
            string audiodesc = "به خوانش ";
            if(!string.IsNullOrEmpty(recitation.AudioArtistUrl))
            {
                audiodesc += $"<a href='{recitation.AudioArtistUrl}'>{recitation.AudioArtist}</a>";
            }
            else
            {
                audiodesc += $"{recitation.AudioArtist}";
            }

            if(!string.IsNullOrEmpty(recitation.AudioSrc))
            {
                if (!string.IsNullOrEmpty(recitation.AudioSrcUrl))
                {
                    audiodesc += $" <a href='{recitation.AudioSrcUrl}'>{recitation.AudioSrc}</a>";
                }
                else
                {
                    audiodesc += $" {recitation.AudioSrc}";
                }
            }

            audiodesc += $" <small><a href='https://ganjoor.net/audioclip/?a={recitation.Id}' onclick='wpopen(this.href); return false' class='comments-link' title='دریافت'>(دریافت)</a></small>";

            if(contributionLink)
            {
                audiodesc += "<br /> <small>می‌خواهید شما بخوانید؟ <a href='http://ava.ganjoor.net/about/'>اینجا</a> را ببینید.</small>";
            }

            return audiodesc;
        }

        public async Task OnGet()
        {
            LoggedIn = !string.IsNullOrEmpty(Request.Cookies["Token"]);
            PoetPage = false;
            CatPage = false;
            PoemPage = false;
            using (HttpClient client = new HttpClient())
            {

                var response = await client.GetAsync($"{APIRoot.Url}/api/ganjoor/poets?includeBio=false");
                response.EnsureSuccessStatusCode();

                _poets = JArray.Parse(await response.Content.ReadAsStringAsync()).ToObject<List<GanjoorPoetViewModel>>();

                var pageQuery = await client.GetAsync($"{APIRoot.Url}/api/ganjoor/page?url={Request.Path}");
                if(pageQuery.IsSuccessStatusCode)
                {
                    GanjoorPage = JObject.Parse(await pageQuery.Content.ReadAsStringAsync()).ToObject<GanjoorPageCompleteViewModel>();
                    GanjoorPage.HtmlText = GanjoorPage.HtmlText.Replace("https://ganjoor.net/", "/");
                    switch (GanjoorPage.GanjoorPageType)
                    {
                        case GanjoorPageType.PoemPage:
                            _preparePoemExcerpt(GanjoorPage.Poem.Next);
                            _preparePoemExcerpt(GanjoorPage.Poem.Previous);
                            GanjoorPage.PoetOrCat = GanjoorPage.Poem.Category;
                            PoemPage = true;
                            break;
                        case GanjoorPageType.PoetPage:
                            PoetPage = true;
                            break;
                        case GanjoorPageType.CatPage:
                            CatPage = true;
                            break;
                    }
                }

            }

            GoogleBreadCrumbList breadCrumbList = new GoogleBreadCrumbList();

            if (PoetPage)
            {
                ViewData["Title"] = $"گنجور &raquo; {GanjoorPage.PoetOrCat.Poet.Name}";
                breadCrumbList.AddItem(GanjoorPage.PoetOrCat.Poet.Name, GanjoorPage.PoetOrCat.Cat.FullUrl, $"/image/poets/{GanjoorPage.PoetOrCat.Poet.Id}.png");
            }
            else
            if (CatPage)
            {
                string title = $"گنجور &raquo; ";
                bool poetCat = true;
                foreach (var gran in GanjoorPage.PoetOrCat.Cat.Ancestors)
                {
                    title += $"{gran.Title} &raquo; ";
                    breadCrumbList.AddItem(gran.Title, gran.FullUrl, poetCat ? $"/image/poets/{GanjoorPage.PoetOrCat.Poet.Id}.png" : "https://i.ganjoor.net/cat.png");
                    poetCat = false;
                }
                breadCrumbList.AddItem(GanjoorPage.PoetOrCat.Cat.Title, GanjoorPage.PoetOrCat.Cat.FullUrl, "https://i.ganjoor.net/cat.png");
                title += GanjoorPage.PoetOrCat.Cat.Title;
                ViewData["Title"] = title;
            }
            else
            if (PoemPage)
                {
                ViewData["Title"] = $"گنجور &raquo; {GanjoorPage.Poem.FullTitle}";
                bool poetCat = true;
                foreach (var gran in GanjoorPage.Poem.Category.Cat.Ancestors)
                {
                    breadCrumbList.AddItem(gran.Title, gran.FullUrl, poetCat ? $"/image/poets/{GanjoorPage.Poem.Category.Poet.Id}.png" : "https://i.ganjoor.net/cat.png");
                    poetCat = false;
                }
                breadCrumbList.AddItem(GanjoorPage.PoetOrCat.Cat.Title, GanjoorPage.PoetOrCat.Cat.FullUrl, "https://i.ganjoor.net/cat.png");
                breadCrumbList.AddItem(GanjoorPage.Poem.Title, GanjoorPage.Poem.FullUrl, "https://i.ganjoor.net/poem.png");

            }
            else
            {
                
                if(GanjoorPage.PoetOrCat != null)
                {
                    bool poetCat = true;
                    string fullTitle = "گنجور &raquo; ";
                    if(GanjoorPage.PoetOrCat.Cat.Ancestors.Count == 0)
                    {
                        fullTitle += $"{GanjoorPage.PoetOrCat.Poet.Name} &raquo; ";
                    }
                    else
                    foreach (var gran in GanjoorPage.PoetOrCat.Cat.Ancestors)
                    {
                        breadCrumbList.AddItem(gran.Title, gran.FullUrl, poetCat ? $"/image/poets/{GanjoorPage.Poem.Category.Poet.Id}.png" : "https://i.ganjoor.net/cat.png");
                        poetCat = false;
                        fullTitle += $"{gran.Title} &raquo; ";
                    }
                    ViewData["Title"] = $"{fullTitle}{GanjoorPage.Title}";
                    breadCrumbList.AddItem(GanjoorPage.PoetOrCat.Poet.Name, GanjoorPage.PoetOrCat.Cat.FullUrl, $"/image/poets/{GanjoorPage.PoetOrCat.Poet.Id}.png");
                }
                else
                {
                    ViewData["Title"] = $"گنجور &raquo; {GanjoorPage.FullTitle}";
                }
                breadCrumbList.AddItem(GanjoorPage.Title, GanjoorPage.FullUrl, "https://i.ganjoor.net/cat.png");
            }

            ViewData["BrearCrumpList"] = breadCrumbList.ToString();
        }
    }
}
