using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newsy_API.AuthenticationModel;
using Newsy_API.Model;

namespace Newsy_API.DAL
{
    public class NewsyDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Author> Authors { get; set; } = null!;
        public DbSet<Article> Articles { get; set; } = null!;

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
                   new IdentityRole
                   {
                       Id = "9c56449f-7a1f-4d22-bbc7-832e84a34fef",
                       ConcurrencyStamp = "4714a715-3823-4faf-8ecb-72dcaaf4c9a2",
                       Name = "Author",
                       NormalizedName = "author"
                   },
                   new IdentityRole
                   {
                       Id = "c85a227d-5d56-4403-b5be-7d96b132ca50",
                       ConcurrencyStamp = "3d0134eb-c25a-4552-8efa-694b7231a127",
                       Name = "Reader",
                       NormalizedName = "reader"
                   }
                   );
        }
    }
}