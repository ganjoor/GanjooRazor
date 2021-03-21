using GanjooRazor.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using RSecurityBackend.Models.Auth.ViewModels;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GanjooRazor.Areas.Panel.Pages
{
    public class IndexModel : PageModel
    {
        /// <summary>
        /// api model
        /// </summary>

        [BindProperty]
        public RegisterRAppUser UserInfo { get; set; }

        /// <summary>
        /// Last Error
        /// </summary>
        public string LastError { get; set; }

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
                    var userInfoResponse = await client.GetAsync($"{APIRoot.Url}/api/users/{Request.Cookies["UserId"]}");
                    if(userInfoResponse.IsSuccessStatusCode)
                    {
                        PublicRAppUser userInfo = JsonConvert.DeserializeObject<PublicRAppUser>(await userInfoResponse.Content.ReadAsStringAsync());

                        UserInfo = new RegisterRAppUser()
                        {
                            Id = userInfo.Id,
                            Username = userInfo.Username,
                            FirstName = userInfo.FirstName,
                            SureName = userInfo.SureName,
                            NickName = userInfo.NickName,
                            PhoneNumber = userInfo.PhoneNumber,
                            Email = userInfo.Email,
                            Website = userInfo.Website,
                            Bio = userInfo.Bio,
                            RImageId = userInfo.RImageId,
                            Status = userInfo.Status,
                            IsAdmin = false,
                            Password = ""
                        };
                    }
                    else
                    {
                        LastError = await userInfoResponse.Content.ReadAsStringAsync();
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
            LastError = "";
            using (HttpClient client = new HttpClient())
            {
                if (await GanjoorSessionChecker.PrepareClient(client, Request, Response))
                {
                    var isAdminResponse = await client.GetAsync($"{APIRoot.Url}/api/users/isadmin?userId={Request.Cookies["UserId"]}");
                    if (isAdminResponse.IsSuccessStatusCode)
                    {
                        UserInfo.IsAdmin = JsonConvert.DeserializeObject<bool>(await isAdminResponse.Content.ReadAsStringAsync());

                        var putResponse = await client.PutAsync($"{APIRoot.Url}/api/users/{Request.Cookies["UserId"]}", new StringContent(JsonConvert.SerializeObject(UserInfo), Encoding.UTF8, "application/json"));

                        if(!putResponse.IsSuccessStatusCode)
                        {
                            LastError = await putResponse.Content.ReadAsStringAsync();
                        }
                    }
                    else
                    {
                        LastError = await isAdminResponse.Content.ReadAsStringAsync();
                    }
                }
                else
                {
                    LastError = "لطفا از گنجور خارج و مجددا به آن وارد شوید.";
                }
            }

            return Page();
        }
    }
}
