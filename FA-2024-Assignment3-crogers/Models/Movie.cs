using System.ComponentModel.DataAnnotations;

namespace FA_2024_Assignment3_crogers.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Display(Name = "IMDB Hyperlink")]
        public string? Link { get; set; }

        public string? Genre { get; set; }

        [Display(Name = "Release Date")]
        public DateOnly? ReleaseDate { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name ="Movie Poster")]
        public byte[]? MovieImage { get; set; }

    }
}
