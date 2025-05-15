using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LombardWebApp
{
	public class AuthService
	{
		private readonly AppDbContext _db;
		private readonly IPasswordHasher<User> _hasher;

		public AuthService(AppDbContext db)
		{
			_db = db;
			_hasher = new PasswordHasher<User>();
		}


		public async Task<bool> Register(string username, string password)
		{
			if (await _db.Users.AnyAsync(u => u.Username == username))
				return false;

			var user = new User
			{
				Username = username,
				PasswordHash = _hasher.HashPassword(null, password),
			};

			_db.Users.Add(user);
			await _db.SaveChangesAsync();
			return true;
		}



		/// <summary>
		/// авторизуем клиентов
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public async Task<User> Authenticate(string username, string password)
		{
			var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
			if (user == null) return null;

			var result = _hasher.VerifyHashedPassword(null, user.PasswordHash, password);
			return result == PasswordVerificationResult.Success ? user : null;
		}


	}
}
