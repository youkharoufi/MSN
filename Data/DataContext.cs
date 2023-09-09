using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MSN.Models;

namespace MSN.Data
{
    public class DataContext : IdentityDbContext<ApplicationUser>
    {

        public DataContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Photo> Photos { get; set; }

    }

}
