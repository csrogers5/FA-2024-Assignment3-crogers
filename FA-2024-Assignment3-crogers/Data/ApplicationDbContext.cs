using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FA_2024_Assignment3_crogers.Models;

namespace FA_2024_Assignment3_crogers.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<FA_2024_Assignment3_crogers.Models.Movie> Movie { get; set; } = default!;
        public DbSet<FA_2024_Assignment3_crogers.Models.Actor> Actor { get; set; } = default!;
        public DbSet<FA_2024_Assignment3_crogers.Models.ActorMovie> ActorMovie { get; set; } = default!;
    }
}
