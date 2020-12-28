using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using RMuseum.Models.Ganjoor.ViewModels;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace GanjooRazor.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        private List<GanjoorPoetViewModel> _poets;
        public List<GanjoorPoetViewModel> Potes { get { return _poets; } }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public async Task OnGet()
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync($"{APIRoot.Url}/api/ganjoor/poets");
                response.EnsureSuccessStatusCode();

                _poets = JArray.Parse(await response.Content.ReadAsStringAsync()).ToObject<List<GanjoorPoetViewModel>>();
            }
        }
    }
}
