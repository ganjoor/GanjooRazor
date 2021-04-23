using GanjooRazor.Models;
using GanjooRazor.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace GanjooRazor.Areas.Admin.Pages
{
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class BannersModel : PageModel
    {
        /// <summary>
        /// image
        /// </summary>
        [BindProperty]
        public BannerUploadModel Upload { get; set; }

        /// <summary>
        /// last message
        /// </summary>
        public string LastMessage { get; set; }

        public void OnGet()
        {
            LastMessage = "";
        }

        /// <summary>
        /// post
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync(BannerUploadModel Upload)
        {
            LastMessage = "";
            using (HttpClient client = new HttpClient())
            {
                if (await GanjoorSessionChecker.PrepareClient(client, Request, Response))
                {
                    MultipartFormDataContent form = new MultipartFormDataContent();

                    using(MemoryStream stream = new MemoryStream())
                    {
    
                        form.Add(new StringContent(Upload.Alt), "alt");
                        form.Add(new StringContent(Upload.Url), "url");

                        await Upload.Image.CopyToAsync(stream);
                        var fileContent = stream.ToArray();
                        form.Add(new ByteArrayContent(fileContent, 0, fileContent.Length), Upload.Image.FileName, Upload.Image.FileName);

                        HttpResponseMessage response = await client.PostAsync($"{APIRoot.Url}/api/ganjoor/site/banner", form);
                        if (!response.IsSuccessStatusCode)
                        {
                            LastMessage = await response.Content.ReadAsStringAsync();
                        }
                      
                    }
                    
                    
                }
                else
                {
                    LastMessage = "لطفا از گنجور خارج و مجددا به آن وارد شوید.";
                }

            }


            return Page();
        }
    }
}
