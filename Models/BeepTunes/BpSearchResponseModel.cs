namespace GanjooRazor.Models.BeepTunes
{
    /// <summary>
    /// beeptunes artist model
    /// </summary>
    public class BpArtistModel
    {
        /// <summary>
        /// id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// artist name
        /// </summary>
        public string ArtisticName { get; set; }

        /// <summary>
        /// url
        /// </summary>
        public string Url { get { return $"https://beeptunes.com/artist/{Id}"; } }

        /// <summary>
        /// picture
        /// </summary>
        public string Picture { get; set; }
    }
    /// <summary>
    /// beeptunes search response model
    /// </summary>
    public class BpSearchResponseModel
    {
        /// <summary>
        /// artists
        /// </summary>
        public BpArtistModel[] Artists { get; set; }
    }
}
