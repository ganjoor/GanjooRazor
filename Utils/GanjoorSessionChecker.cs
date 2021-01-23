using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RSecurityBackend.Models.Auth.ViewModels;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GanjooRazor.Utils
{
    /// <summary>
    /// Ganjoor Session Checker
    /// </summary>
    public class GanjoorSessionChecker
    {
        /// <summary>
        /// if user is logged in adds user token to <paramref name="client"/> and then checks user session and if needs renewal, renews it
        /// </summary>
        /// <param name="client"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public static async Task<bool> PrepareClient(HttpClient client, HttpRequest request, HttpResponse response)
        {
            if (string.IsNullOrEmpty(request.Cookies["Token"]) || string.IsNullOrEmpty(request.Cookies["SessionId"]))
                return false;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", request.Cookies["Token"]);
            var r = await client.GetAsync($"{APIRoot.Url}/api/users/checkmysession/?sessionId={request.Cookies["SessionId"]}");
            if (r.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            else
            if(r.StatusCode == HttpStatusCode.Unauthorized)
            {
                var reLoginUrl = $"{APIRoot.Url}/api/users/relogin/{request.Cookies["SessionId"]}";
                var reLoginResponse = await client.PutAsync(reLoginUrl, null);

                if (reLoginResponse.StatusCode != HttpStatusCode.OK)
                {
                    return false;
                }

                LoggedOnUserModel loggedOnUser = JsonConvert.DeserializeObject<LoggedOnUserModel>(await reLoginResponse.Content.ReadAsStringAsync());

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loggedOnUser.Token);

                var cookieOption = new CookieOptions()
                {
                    Expires = DateTime.Now.AddDays(365),
                };

                response.Cookies.Append("UserId", loggedOnUser.User.Id.ToString(), cookieOption);
                response.Cookies.Append("SessionId", loggedOnUser.SessionId.ToString(), cookieOption);
                response.Cookies.Append("Token", loggedOnUser.Token, cookieOption);
                response.Cookies.Append("Username", loggedOnUser.User.Username, cookieOption);
                response.Cookies.Append("Name", $"{loggedOnUser.User.FirstName} {loggedOnUser.User.SureName}", cookieOption);

                return true;

            }
            return false;
        }
    }
}
