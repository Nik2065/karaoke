using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace KaraokeWebApp3
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Booking> Bookings { get; set; }

		public DbSet<Space> Spaces { get; set; }
	}

    [Table("users")]
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public int UserType { get; set; }
        public string? Phone { get; set; }

    }

    //Бронирования
    [Table("bookings")]
    public class Booking
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int AuthorId { get; set; }
        public DateTime DtBegin { get; set; }
        public DateTime DtEnd { get; set;}
        public DateTime Created { get; set;}

        //public string SpaceName { get; set; }//название зала
        public int SpaceId { get; set; }


	}

    //Бронирования
    [Table("spaces")]
    public class Space
    {
        public int Id { get; set; }
        public string SpaceName { get; set; }//название зала
    }

	public enum UserTypeEnum
    {
        Client = 0, Manager = 1,
    }



}
