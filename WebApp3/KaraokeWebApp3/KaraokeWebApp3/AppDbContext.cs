using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KaraokeWebApp3
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public int UserType { get; set; }
    }

    public enum UserTypeEnum
    {
        Client = 0, Manager = 1,
    }



}
