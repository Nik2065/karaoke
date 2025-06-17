using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace LombardWebApp
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
		public DbSet<User> Users { get; set; }
		public DbSet<Lead> Leads { get; set; }

	}

	//пользователь сайта
	[Table("users")]
	public class User
	{
		public int Id { get; set; }
		public string Username { get; set; }
		public string PasswordHash { get; set; }
		public DateTime? Created {  get; set; }

	}

	//заявки
	[Table("leads")]
	public class Lead
	{
		public int Id { get; set; }
		public string Username { get; set; }
		public string Phone { get; set; }
		public DateTime Created { get; set; }
	}


	public enum UserTypeEnum
	{
		Client = 0, Manager = 1,
	}
}

