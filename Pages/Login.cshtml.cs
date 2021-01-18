using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using RSecurityBackend.Models.Auth.ViewModels;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GanjooRazor.Pages
{
    public class LoginModel : PageModel
    {

        [BindProperty]
        public LoginViewModel LoginViewModel { get; set; }

        public bool LoggedIn { get; set; }

        public string UserFriendlyName { get; set; }

        public string LastError { get; set; }

        public string RedirectUrl { get; set; }

        public void OnGet()
        {
            UserFriendlyName = Request.Cookies["Name"];
            LoggedIn = !string.IsNullOrEmpty(UserFriendlyName);
            LastError = "";
            RedirectUrl = Request.Query["redirect"];
            if (string.IsNullOrEmpty(RedirectUrl))
            {
                RedirectUrl = "/";
            }

        }

        public async Task<IActionResult> OnPostAsync()
        {
            RedirectUrl = Request.Query["redirect"];
            if (string.IsNullOrEmpty(RedirectUrl))
            {
                RedirectUrl = "/";
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            LoginViewModel.ClientAppName = "GanjooRazor";
            LoginViewModel.Language = "fa-IR";

            using (HttpClient client = new HttpClient())
            {
                if(string.IsNullOrEmpty(LoginViewModel.Username))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["Token"]);
                    var logoutUrl = $"{APIRoot.Url}/api/users/delsession?userId={Request.Cookies["UserId"]}&sessionId={Request.Cookies["SessionId"]}";
                    

                    var cookieOption = new CookieOptions()
                    {
                        Expires = DateTime.Now.AddDays(-1)
                    };
                    foreach (var cookieName in new string[] { "UserId", "SessionId", "Token", "Username", "Name" })
                    {
                        if(Request.Cookies[cookieName] != null)
                        {
                            Response.Cookies.Append(cookieName, "", cookieOption);
                        }
                    }

                    await client.DeleteAsync(logoutUrl);

                    return Page();
                }
                else
                {
                    var stringContent = new StringContent(JsonConvert.SerializeObject(LoginViewModel), Encoding.UTF8, "application/json");
                    var loginUrl = $"{APIRoot.Url}/api/users/login";
                    var response = await client.PostAsync(loginUrl, stringContent);

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        LastError = response.ToString();
                        return Page();
                    }

                    LoggedOnUserModel loggedOnUser = JsonConvert.DeserializeObject<LoggedOnUserModel>(await response.Content.ReadAsStringAsync());

                    var cookieOption = new CookieOptions()
                    {
                        Expires = DateTime.Now.AddDays(365),
                    };

                    Response.Cookies.Append("UserId", loggedOnUser.User.Id.ToString(), cookieOption);
                    Response.Cookies.Append("SessionId", loggedOnUser.SessionId.ToString(), cookieOption);
                    Response.Cookies.Append("Token", loggedOnUser.Token, cookieOption);
                    Response.Cookies.Append("Username", loggedOnUser.User.Username, cookieOption);
                    Response.Cookies.Append("Name", $"{loggedOnUser.User.FirstName} {loggedOnUser.User.SureName}", cookieOption);
                }

            }

            LastError = "Success!";


            return Redirect(RedirectUrl);
        }
    }
}
