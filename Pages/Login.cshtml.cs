using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RSecurityBackend.Models.Auth.ViewModels;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GanjooRazor.Pages
{
    public class LoginModel : PageModel
    {

        [BindProperty]
        public LoginViewModel LoginViewModel { get; set; }

        public string LastError { get; set; }

        public void OnGet()
        {
            LoginViewModel = new LoginViewModel()
            {
                ClientAppName = "GanjooRazor",
                Language = "fa-IR"
            };
            LastError = "";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            using (HttpClient client = new HttpClient())
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

            LastError = "Success!";

            return Page();
        }
    }
}
