using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DataAccess
{
    public class AppDbContext : IdentityDbContext<User>
    {
        protected readonly IConfiguration Configuration;

        public AppDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /*protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to mysql with connection string from app settings
            var connectionString = Configuration.GetConnectionString("DatabaseConnectionStr");
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }*/

        public DbSet<User> Users { get; set; }
    }
}