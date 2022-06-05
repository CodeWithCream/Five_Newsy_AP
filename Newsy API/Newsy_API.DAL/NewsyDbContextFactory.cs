using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Newsy_API.DAL
{
    public class NewsyDbContextFactory : IDesignTimeDbContextFactory<NewsyDbContext>
    {
        NewsyDbContext IDesignTimeDbContextFactory<NewsyDbContext>.CreateDbContext(string[] args)
        {
            //use appsettings in web project
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Newsy_API"))
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<NewsyDbContext>();
            var connectionString = configuration.GetConnectionString("DatabaseConnection");

            builder.UseSqlServer(connectionString);

            return new NewsyDbContext(builder.Options);
        }
    }
}
