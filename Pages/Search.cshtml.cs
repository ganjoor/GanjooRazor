using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DNTPersianUtils.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RMuseum.Models.Ganjoor.ViewModels;
using RSecurityBackend.Models.Generic;

namespace GanjooRazor.Pages
{
    [IgnoreAntiforgeryToken(Order = 1001)]

    public class SearchModel : PageModel
    {
        public List<GanjoorPoetViewModel> Poets { get; set; }

        public string Query { get; set; }
        public int AuthorId { get; set; }

        public List<GanjoorSearchVerseViewModel> Verses { get; set; }

        public string PagingToolsHtml { get; set; }


        public async Task<IActionResult> OnGet()
        {
            Query = Request.Query["s"];
            AuthorId = int.Parse(Request.Query["author"]);

            using (HttpClient client = new HttpClient())
            {
                //todo: use html master layout or make it partial
                // 1. poets 
                var poetsResponse = await client.GetAsync($"{APIRoot.Url}/api/ganjoor/poets?includeBio=false");
                poetsResponse.EnsureSuccessStatusCode();
                Poets = JArray.Parse(await poetsResponse.Content.ReadAsStringAsync()).ToObject<List<GanjoorPoetViewModel>>();
                var poetName = Poets.SingleOrDefault(p => p.Id == AuthorId);
                if (poetName != null)
                {
                    ViewData["Title"] = $"گنجور &raquo; نتایج جستجو برای {Query} &raquo; {poetName?.Name}";
                }
                else
                {
                    ViewData["Title"] = $"گنجور &raquo; نتایج جستجو برای {Query}";
                }

                // 2. search verses
                int pageNumber = 1;
                if (!string.IsNullOrEmpty(Request.Query["page"]))
                {
                    pageNumber = int.Parse(Request.Query["page"]);
                }

                var searchQueryResponse = await client.GetAsync($"{APIRoot.Url}/api/ganjoor/verse/search?query={Query}&poetId={AuthorId}&PageNumber={pageNumber}&PageSize=20");
                searchQueryResponse.EnsureSuccessStatusCode();

                Verses = JArray.Parse(await searchQueryResponse.Content.ReadAsStringAsync()).ToObject<List<GanjoorSearchVerseViewModel>>();
                if (Verses != null)
                {
                    // highlight searched word
                    foreach (var ganjoorSearchVerseViewModel in Verses)
                    {
                        ganjoorSearchVerseViewModel.Text = Regex.Replace(ganjoorSearchVerseViewModel.Text, Query, $"<span class=\"hilite\">{Query}</span>", RegexOptions.IgnoreCase | RegexOptions.RightToLeft);
                    }

                    string paginationMetadata = searchQueryResponse.Headers.GetValues("paging-headers").FirstOrDefault();

                    PagingToolsHtml = GeneratePagingBarHtml(paginationMetadata, $"/search?s={Query}&author={AuthorId}");
                }
            }

            return Page();
        }

        private string GeneratePagingBarHtml(string paginationMetadataJsonValue, string routeStartWithQueryStrings)
        {
            string htmlText = "<p style=\"text-align: center;\">";

            if (!string.IsNullOrEmpty(paginationMetadataJsonValue))
            {
                PaginationMetadata paginationMetadata = JsonConvert.DeserializeObject<PaginationMetadata>(paginationMetadataJsonValue);

                if (paginationMetadata.totalPages > 1)
                {
                    if (paginationMetadata.currentPage > 3)
                    {
                        htmlText += $"[<a href=\"{routeStartWithQueryStrings}&page=1\">صفحهٔ اول</a>] …";
                    }
                    for (int i = (paginationMetadata.currentPage - 2); i <= (paginationMetadata.currentPage + 2); i++)
                    {
                        if (i >= 1 && i <= paginationMetadata.totalPages)
                        {
                            htmlText += " [";
                            if (i == paginationMetadata.currentPage)
                            {
                                htmlText += i.ToPersianNumbers();
                            }
                            else
                            {
                                htmlText += $"<a href=\"{routeStartWithQueryStrings}&page={i}\">{i.ToPersianNumbers()}</a>";
                            }
                            htmlText += "] ";
                        }
                    }
                    if (paginationMetadata.totalPages > (paginationMetadata.currentPage + 2))
                    {
                        htmlText += $"… [<a href=\"{routeStartWithQueryStrings}&page={paginationMetadata.totalPages}\">صفحهٔ آخر</a>]";
                    }
                }
            }

            htmlText += $"</p>{Environment.NewLine}";
            return htmlText;
        }

    }
}
