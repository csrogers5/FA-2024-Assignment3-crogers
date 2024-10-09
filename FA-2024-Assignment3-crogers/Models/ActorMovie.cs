using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FA_2024_Assignment3_crogers.Models
{
    public class ActorMovie
    {

        public int Id { get; set; }


        [ForeignKey("Actor")]
        [Display (Name ="Actor Name")]
        public int? ActorId { get; set; } 

        public Actor? Actor { get; set; }

        [ForeignKey("Movie")]
        [Display(Name = "Movie Title")]
        public int? MovieId { get; set; }

        public Movie? Movie { get; set; }


    }
}
