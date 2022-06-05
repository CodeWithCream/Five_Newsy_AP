using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newsy_API.AuthenticationModel;
using Newsy_API.Model;

namespace Newsy_API.DAL
{
    public class NewsyDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Author>? Authors { get; set; }

        public DbSet<Article>? Articles { get; set; }

        public NewsyDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().ToTable("Users");
            builder.Entity<Author>(entity =>
            {
                entity.HasMany(author => author.Articles)
                    .WithOne(article => article.Author)
                    .HasForeignKey(article => article.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("AspNetUsers");
                entity.HasOne(applicationUser => applicationUser.User)
                   .WithOne()
                   .HasForeignKey<ApplicationUser>(applicationUser => applicationUser.UserRefId);
                entity.Property(applicationUser => applicationUser.IsActivated);
            });

            builder.Entity<IdentityRole>()
               .HasData(
                   new IdentityRole { Name = "Author", NormalizedName = "author" },
                   new IdentityRole { Name = "Reader", NormalizedName = "reader" }
                   );
        }
    }
}