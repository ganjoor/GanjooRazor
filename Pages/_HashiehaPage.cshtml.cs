using DNTPersianUtils.Core;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RMuseum.Models.Ganjoor.ViewModels;
using RMuseum.Services.Implementation.ImportedFromDesktopGanjoor;
using RSecurityBackend.Models.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GanjooRazor.Pages
{
    public partial class IndexModel : PageModel
    {
        /// <summary>
        /// صفحهٔ حاشیه‌ها
        /// </summary>
        /// <returns></returns>
        private async Task _GenerateHashiehaHtmlText()
        {
            using (HttpClient client = new HttpClient())
            {
                int pageNumber = 1;
                if (!string.IsNullOrEmpty(Request.Query["page"]))
                {
                    pageNumber = int.Parse(Request.Query["page"]);
                }
                var response = await client.GetAsync($"{APIRoot.Url}/api/ganjoor/comments?PageNumber={pageNumber}&PageSize=20");
                response.EnsureSuccessStatusCode();

                string htmlText = "";

                foreach (var comment in JArray.Parse(await response.Content.ReadAsStringAsync()).ToObject<List<GanjoorCommentFullViewModel>>())
                {
                    htmlText += $"<blockquote>{comment.HtmlComment}{Environment.NewLine}" +
                        $"<p>{comment.AuthorName} <small>در {comment.CommentDate.ToFriendlyPersianDateTextify()}</small> دربارهٔ <a href=\"{comment.Poem.UrlSlug}\">{comment.Poem.Title}</a>" +
                        $"</blockquote>{Environment.NewLine}<hr />{Environment.NewLine}";
                }

                htmlText += "<p style=\"text-align: center;\">";

                string paginnationMetadata = response.Headers.GetValues("paging-headers").FirstOrDefault();
                if (!string.IsNullOrEmpty(paginnationMetadata))
                {
                    PaginationMetadata paginationMetadata = JsonConvert.DeserializeObject<PaginationMetadata>(paginnationMetadata);

                    if (paginationMetadata.totalPages > 1)
                    {
                        if (paginationMetadata.currentPage > 3)
                        {
                            htmlText += $"[<a href=\"/hashieha/?page=1\">صفحهٔ اول</a>] …";
                        }
                        for (int i = (paginationMetadata.currentPage - 2); i <= (paginationMetadata.currentPage + 2); i++)
                        {
                            if (i >= 1 && i <= paginationMetadata.totalPages)
                            {
                                htmlText += " [";
                                if (i == paginationMetadata.currentPage)
                                {
                                    htmlText += GPersianTextSync.Sync(i.ToString());
                                }
                                else
                                {
                                    htmlText += $"<a href=\"/hashieha/?page={i}\">{GPersianTextSync.Sync(i.ToString())}</a>";
                                }
                                htmlText += "] ";
                            }
                        }
                        if (paginationMetadata.totalPages > (paginationMetadata.currentPage + 2))
                        {
                            htmlText += $"… [<a href=\"/hashieha/?page={paginationMetadata.totalPages}\">صفحهٔ آخر</a>]";
                        }
                    }
                }

                htmlText += $"</p>{Environment.NewLine}";

                GanjoorPage.HtmlText = htmlText;
            }
        }
    }
}
