using Microsoft.Extensions.Configuration;
using System.IO;

namespace GanjooRazor
{
    /// <summary>
    /// API Root
    /// </summary>
    public class APIRoot
    {
        /// <summary>
        /// url
        /// </summary>
        public static string Url
        {
            get
            {
                if (!string.IsNullOrEmpty(_url))
                    return _url;
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json")
                    .Build();
                _url = configuration["APIRoot"];
                return _url;
            }
        }

        private static string _url = "";
    }
}
