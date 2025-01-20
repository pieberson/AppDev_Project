using System.ComponentModel.DataAnnotations;

namespace AppDev.Models
{
    public class MediaItem
    {
        // Unique identifier for each media item
        public int Id { get; set; }

        // Media type: Movie, Show, or Book
        [Required(ErrorMessage = "Please select the class of the media.")]
        [Display(Name = "Media Type")]
        public string Class { get; set; }

        // Title of the media (required and with validation for length)
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        [Display(Name = "Media Title")]
        public string Title { get; set; }

        // Year the media was completed
        [Range(1900, 2100, ErrorMessage = "Year Finished must be between 1900 and 2100.")]
        [Display(Name = "Year Finished")]
        public int YearFinished { get; set; }

        // Rating given by the user (1-5 stars)
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        [Display(Name = "Rating (1-5 Stars)")]
        public int Rating { get; set; }

        // Optional review text
        [Display(Name = "Review (Optional)")]
        public string Review { get; set; }

        // Ensure that Season is only entered for Shows (nullable for Movies and Books)
        [Range(1, int.MaxValue, ErrorMessage = "Season must be a positive number.")]
        [Display(Name = "Season Number")]
        public int? Season { get; set; }

        // Custom ToString method for better display in lists or logs
        public override string ToString()
        {
            return $"{Title} ({Class}, {YearFinished}) - {Rating} Stars";
        }
    }
}
