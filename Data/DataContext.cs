using System.Reflection.Emit;
using System.Text.RegularExpressions;
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
        public DbSet<ChatMessage> Messages { get; set; }

        public DbSet<Models.Group> Groups { get; set; }
        public DbSet<Connection> Connections { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            //messages
            builder.Entity<ChatMessage>()
                .HasOne(u => u.Target)
                .WithMany(m => m.MessagesRecieved)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ChatMessage>()
                .HasOne(u => u.Sender)
                .WithMany(m => m.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Friends);

            //builder.Entity<ApplicationUser>()
            //    .HasMany(u => u.FriendRequests);

            //builder.Entity<ApplicationUser>().Ignore(u => u.FriendRequests);
        }
        }

    }
