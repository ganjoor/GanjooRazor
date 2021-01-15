using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using RSecurityBackend.Models.Auth.ViewModels;
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
            }

            LastError = "Success!";

            return Page();
        }
    }
}
