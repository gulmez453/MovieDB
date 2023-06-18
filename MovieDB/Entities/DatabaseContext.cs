using Microsoft.EntityFrameworkCore;

namespace MovieDB.Entities
{
    public class DatabaseContext : DbContext // haritage dbcontext from enttityframework
    {
        //databasecontext represent database
        public DatabaseContext(DbContextOptions options) : base(options)
        {

        }
        //create dbset for reach database 
        //user movies represente class
        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Rate> Rates { get; set; }
        public DbSet<MovieUser> MoviesUsers { get; set; }

    }
}
