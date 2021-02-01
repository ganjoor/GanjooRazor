using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using RSecurityBackend.Models.Auth.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
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

                List<string> permissions = new List<string>();
                foreach (var securableItem in loggedOnUser.SecurableItem)
                    foreach (var operation in securableItem.Operations)
                    {
                        if (operation.Status)
                        {
                            permissions.Add($"{securableItem.ShortName}-{operation.ShortName}");
                        }
                    }
                response.Cookies.Append("Permissions", JsonConvert.SerializeObject(permissions.ToArray()), cookieOption);

                return true;

            }
            return false;
        }

        /// <summary>
        /// has permission
        /// </summary>
        /// <param name="request"></param>
        /// <param name="secuableShortName"></param>
        /// <param name="operationShortName"></param>
        /// <returns></returns>
        public static bool IsPermitted(HttpRequest request, string secuableShortName, string operationShortName)
        {
            string json = request.Cookies["Permissions"];

            if (string.IsNullOrEmpty(json))
                return false;

            string[] permissions = JsonConvert.DeserializeObject<string[]>(json);
            return permissions.Where(p => p == $"{secuableShortName}-{operationShortName}").SingleOrDefault() != null;
        }

        /// <summary>
        /// apply permissions to view data
        /// </summary>
        /// <param name="request"></param>
        /// <param name="viewData"></param>
        public static void ApplyPermissionsToViewData(HttpRequest request, ViewDataDictionary viewData)
        {
            string json = request.Cookies["Permissions"];

            if (string.IsNullOrEmpty(json))
                return;

            string[] permissions = JsonConvert.DeserializeObject<string[]>(json);
            foreach(string permission in permissions)
            {
                viewData[permission] = true;
            }
        }
    }
}
