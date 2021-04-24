using System;
using Microsoft.EntityFrameworkCore;

namespace MyFilmMVCV1.Models
{
    public class ApplicationDbContext: DbContext
    {

         public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
            {
            }

        public DbSet<Film> Films { get; set; }
    }
}
