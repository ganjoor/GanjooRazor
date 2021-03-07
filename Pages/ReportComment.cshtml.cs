using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RMuseum.Models.Ganjoor.ViewModels;

namespace GanjooRazor.Pages
{
    public class ReportCommentModel : PageModel
    {
        /// <summary>
        /// is logged on
        /// </summary>
        public bool LoggedIn { get; set; }

        /// <summary>
        /// Last Error
        /// </summary>
        public string LastError { get; set; }

        /// <summary>
        /// Post Success
        /// </summary>
        public bool PostSuccess { get; set; }

        /// <summary>
        /// api model
        /// </summary>
        [BindProperty]
        public GanjoorPostReportCommentViewModel Report { get; set; }

        public void OnGet()
        {
            PostSuccess = false;
            LastError = "";
            LoggedIn = !string.IsNullOrEmpty(Request.Cookies["Token"]);

            Report = new GanjoorPostReportCommentViewModel()
            {
                ReasonCode = "bogus",
                ReasonText = "",
            };

            if (!string.IsNullOrEmpty(Request.Query["CommentId"]))
            {
                Report.CommentId = int.Parse(Request.Query["CommentId"]);
            }
            else
            {
                Report.CommentId = 0;
            }

            


        }
    }
}
