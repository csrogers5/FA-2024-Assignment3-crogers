using System.ComponentModel.DataAnnotations;

namespace FA_2024_Assignment3_crogers.Models
{
    public class Actor
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Gender { get; set; }

        public int? Age { get; set; }

        [Display(Name = "IMDB Hyperlink")]
        public string? Link { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "Actor Photo")]
        public byte[]? ActorImage { get; set; }


    }
}
