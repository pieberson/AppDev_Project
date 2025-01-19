using System.ComponentModel.DataAnnotations;

namespace AppDev.Models
{
    public class MediaItem
    {
        public int Id { get; set; }             // Unique identifier for each media item

        [Required]
        public string Class { get; set; }       // Media type: Movie, Show, or Book

        [Required]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; }       // Title of the media

        [Range(1900, 2100, ErrorMessage = "YearFinished must be between 1900 and 2100.")]
        public int YearFinished { get; set; }   // Year the media was completed

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }         // Rating given by the user (1-5 stars)

        public string Review { get; set; }      // Optional review

        [Range(1, int.MaxValue, ErrorMessage = "Season must be a positive number.")]
        public int? Season { get; set; }        // Season number (nullable for Movies and Books)

        public override string ToString()
        {
            return $"{Title} ({Class}, {YearFinished}) - {Rating} Stars";
        }
    }
}
