using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using RMuseum.Models.Ganjoor.ViewModels;

namespace GanjooRazor.Pages
{
    [IgnoreAntiforgeryToken(Order = 1001)]

    public class SearchModel : PageModel
    {
        public List<GanjoorPoetViewModel> Poets { get; set; }

        public string Query { get; set; }
        public int AuthorId { get; set; }

        public List<GanjoorSearchVerseViewModel> Verses { get; set; }

        public async Task<IActionResult> OnGet()
        {
            Query = Request.Query["s"];
            AuthorId = int.Parse(Request.Query["author"]);

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync($"{APIRoot.Url}/api/ganjoor/poets?includeBio=false");
                response.EnsureSuccessStatusCode();

                Poets = JArray.Parse(await response.Content.ReadAsStringAsync()).ToObject<List<GanjoorPoetViewModel>>();


                var searchQueryResult = await client.GetAsync($"{APIRoot.Url}/api/ganjoor/verse/search?query={Query}&poetId={AuthorId}");
                searchQueryResult.EnsureSuccessStatusCode();

                Verses = JArray.Parse(await searchQueryResult.Content.ReadAsStringAsync()).ToObject<List<GanjoorSearchVerseViewModel>>();

                if (Verses != null)
                {
                    // highlight searched word
                    foreach (var ganjoorSearchVerseViewModel in Verses)
                    {
                        ganjoorSearchVerseViewModel.Text = Regex.Replace(ganjoorSearchVerseViewModel.Text, Query, $"<span class=\"hilite\">{Query}</span>", RegexOptions.IgnoreCase | RegexOptions.RightToLeft);
                    }
                }
            }
            
            var poetName = Poets.SingleOrDefault(p => p.Id == AuthorId);
            if (poetName != null)
            {
                ViewData["Title"] = $"گنجور &raquo; نتایج جستجو برای {Query} &raquo; {poetName?.Name}";
            }
            else
            {
                ViewData["Title"] = $"گنجور &raquo; نتایج جستجو برای {Query}";
            }

            return Page();
        }

    }
}
