using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using RMuseum.Models.Auth.ViewModel;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GanjooRazor.Pages
{
    public partial class IndexModel : PageModel
    {
        /// <summary>
        /// صفحهٔ پروفایل کاربر
        /// </summary>
        /// <returns></returns>
        private async Task _GenerateUserProfileHtmlText()
        {
            if (string.IsNullOrEmpty(Request.Query["userid"]))
                return;
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync($"{APIRoot.Url}/api/ganjoor/user/profile/{Request.Query["userid"]}");
                response.EnsureSuccessStatusCode();

                GanjoorUserPublicProfile profile = JsonConvert.DeserializeObject<GanjoorUserPublicProfile>(await response.Content.ReadAsStringAsync());

                ViewData["Title"] = $"گنجور &raquo; دربارهٔ {profile.NickName}";

                GanjoorPage.Title = $"دربارهٔ {profile.NickName}";

                string htmlText = $"<table style=\"width:100%\" class=\"noborder\">{Environment.NewLine}";

                htmlText += $"<tr><td>نام مستعار:</td><td>{profile.NickName}</td></tr>{Environment.NewLine}";
                if(!string.IsNullOrEmpty(profile.Website))
                {
                    htmlText += $"<tr><td colspan=\"2\"><a href=\"{profile.Website}\">تارنما</a></td></tr>{Environment.NewLine}";
                }
                
                if(!string.IsNullOrEmpty(profile.Bio))
                {
                    htmlText += $"<tr><td>درباره:</td><td>{profile.Bio}</td></tr>{Environment.NewLine}";
                }

                htmlText += $"</table>{Environment.NewLine}";

                GanjoorPage.HtmlText = htmlText;
            }
        }
    }
}
