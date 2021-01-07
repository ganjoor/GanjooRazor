namespace RMuseum.Models.Ganjoor.ViewModels
{
    /// <summary>
    /// Ganjoor Page Type
    /// </summary>
    public enum GanjoorPageType
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,

        /// <summary>
        /// Poet Page
        /// </summary>
        PoetPage = 1,

        /// <summary>
        /// Cat Page
        /// </summary>
        CatPage = 2,

        /// <summary>
        /// Poem Page
        /// </summary>
        PoemPage = 3,

        /// <summary>
        /// Prosody Similars
        /// </summary>
        ProsodySimilars = 4,

        /// <summary>
        /// Hashieha Page
        /// </summary>
        AllComments = 5,

        /// <summary>
        /// Similar Poems of two poets
        /// </summary>
        TwoPoetsSimilarPoems = 6,

        /// <summary>
        /// Prosody and Statistics
        /// </summary>
        ProsodyAndStats = 7
    }

    /// <summary>
    /// Ganjoor Page View Model
    /// </summary>
    public class GanjoorPageCompleteViewModel
    {
        /// <summary>
        /// id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Ganjoor Page Type
        /// </summary>
        public GanjoorPageType GanjoorPageType { get; set; }

        /// <summary>
        /// title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// cat + parent cats title + title
        /// </summary>
        public string FullTitle { get; set; }

        /// <summary>
        /// url => slug
        /// </summary>
        public string UrlSlug { get; set; }

        /// <summary>
        /// sample: /hafez/ghazal/sh1
        /// </summary>
        public string FullUrl { get; set; }

        /// <summary>
        /// Html Text
        /// </summary>
        public string HtmlText { get; set; }

        /// <summary>
        /// Poet or Cat
        /// </summary>
        public GanjoorPoetCompleteViewModel PoetOrCat { get; set; }

        /// <summary>
        /// Poem
        /// </summary>
        public GanjoorPoemCompleteViewModel Poem { get; set; }

        /// <summary>
        /// Second Poet
        /// </summary>
        public GanjoorPoetViewModel SecondPoet { get; set; }
    }
}
