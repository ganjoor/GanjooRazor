using GanjooRazor.Utils;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using RMuseum.Models.Ganjoor.ViewModels;
using RMuseum.Models.GanjoorAudio.ViewModels;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace GanjooRazor.Pages
{
    public class IndexModel : PageModel
    {
        /// <summary>
        /// is home page
        /// </summary>
        public bool IsHomePage { get; set; }
        /// <summary>
        /// Poets
        /// </summary>
        public List<GanjoorPoetViewModel> Poets { get; set; }

        /// <summary>
        /// Corresponding Ganojoor Page
        /// </summary>
        public GanjoorPageCompleteViewModel GanjoorPage { get; set; }

        /// <summary>
        /// is poet page
        /// </summary>
        public bool IsPoetPage { get; set; }

        /// <summary>
        /// is category page
        /// </summary>
        public bool IsCatPage { get; set; }

        /// <summary>
        /// is poem page
        /// </summary>
        public bool IsPoemPage { get; set; }

        /// <summary>
        /// is logged on
        /// </summary>
        public bool LoggedIn { get; set; }

        /// <summary>
        /// prepare poem except
        /// </summary>
        /// <param name="poem"></param>
        private void _preparePoemExcerpt(GanjoorPoemSummaryViewModel poem)
        {
            if (poem == null)
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

        /// <summary>
        /// get audio description
        /// </summary>
        /// <param name="recitation"></param>
        /// <param name="contributionLink"></param>
        /// <returns></returns>
        public string getAudioDesc(PublicRecitationViewModel recitation, bool contributionLink = false)
        {
            string audiodesc = "به خوانش ";
            if (!string.IsNullOrEmpty(recitation.AudioArtistUrl))
            {
                audiodesc += $"<a href='{recitation.AudioArtistUrl}'>{recitation.AudioArtist}</a>";
            }
            else
            {
                audiodesc += $"{recitation.AudioArtist}";
            }

            if (!string.IsNullOrEmpty(recitation.AudioSrc))
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

            if (contributionLink)
            {
                audiodesc += "<br /> <small>می‌خواهید شما بخوانید؟ <a href='http://ava.ganjoor.net/about/'>اینجا</a> را ببینید.</small>";
            }

            return audiodesc;
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <returns></returns>
        public async Task OnGet()
        {
            LoggedIn = !string.IsNullOrEmpty(Request.Cookies["Token"]);
            IsPoetPage = false;
            IsCatPage = false;
            IsPoemPage = false;
            IsHomePage = Request.Path == "/";
            GoogleBreadCrumbList breadCrumbList = new GoogleBreadCrumbList();

            using (HttpClient client = new HttpClient())
            {

                var response = await client.GetAsync($"{APIRoot.Url}/api/ganjoor/poets?includeBio=false");
                response.EnsureSuccessStatusCode();

                Poets = JArray.Parse(await response.Content.ReadAsStringAsync()).ToObject<List<GanjoorPoetViewModel>>();

                if(!IsHomePage)
                {
                    var pageQuery = await client.GetAsync($"{APIRoot.Url}/api/ganjoor/page?url={Request.Path}");
                    if (pageQuery.IsSuccessStatusCode)
                    {
                        GanjoorPage = JObject.Parse(await pageQuery.Content.ReadAsStringAsync()).ToObject<GanjoorPageCompleteViewModel>();
                        GanjoorPage.HtmlText = GanjoorPage.HtmlText.Replace("https://ganjoor.net/", "/");
                        switch (GanjoorPage.GanjoorPageType)
                        {
                            case GanjoorPageType.PoemPage:
                                _preparePoemExcerpt(GanjoorPage.Poem.Next);
                                _preparePoemExcerpt(GanjoorPage.Poem.Previous);
                                GanjoorPage.PoetOrCat = GanjoorPage.Poem.Category;
                                IsPoemPage = true;
                                break;
                            case GanjoorPageType.PoetPage:
                                IsPoetPage = true;
                                break;
                            case GanjoorPageType.CatPage:
                                IsCatPage = true;
                                break;
                        }
                    }
                }
            }

            if (IsHomePage)
            {
                ViewData["Title"] = "گنجور";
            }
            else
            if (IsPoetPage)
            {
                ViewData["Title"] = $"گنجور &raquo; {GanjoorPage.PoetOrCat.Poet.Name}";
                breadCrumbList.AddItem(GanjoorPage.PoetOrCat.Poet.Name, GanjoorPage.PoetOrCat.Cat.FullUrl, $"/image/poets/{GanjoorPage.PoetOrCat.Poet.Id}.png");
            }
            else
            if (IsCatPage)
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
            if (IsPoemPage)
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
                if (GanjoorPage.PoetOrCat != null)
                {
                    bool poetCat = true;
                    string fullTitle = "گنجور &raquo; ";
                    if (GanjoorPage.PoetOrCat.Cat.Ancestors.Count == 0)
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
