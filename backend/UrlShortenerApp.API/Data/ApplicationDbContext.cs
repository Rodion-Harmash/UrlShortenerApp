using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UrlShortenerApp.API.Models;

namespace UrlShortenerApp.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ShortUrl> ShortUrls { get; set; }
        public DbSet<AboutInfo> AboutInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ShortUrl>()
                .HasIndex(x => x.ShortCode)
                .IsUnique();

            builder.Entity<ShortUrl>()
                .HasOne(x => x.CreatedBy)
                .WithMany(u => u.ShortUrls)
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AboutInfo>()
                .HasOne(x => x.EditedBy)
                .WithMany(u => u.EditedAbouts)
                .HasForeignKey(x => x.EditedById)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
