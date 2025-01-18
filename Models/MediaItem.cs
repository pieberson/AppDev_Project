namespace AppDev.Models
{
    public class MediaItem
    {
        public string Class { get; set; }        // Media type: Movie, Show, or Book
        public string Title { get; set; }        // Title of the media
        public int YearFinished { get; set; }   // Year the media was completed
        public int Rating { get; set; }         // Rating given by the user (1-5 stars)
        public string Review { get; set; }      // Optional review
        public int? Season { get; set; }        // Season number (nullable for Movies and Books)
    }
}
