using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KaraokeWebApp3
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


        public async Task<bool> Register(string username, string password, string phone)
        {
            if (await _db.Users.AnyAsync(u => u.Phone == phone))
                return false;

            var user = new User
            {
                Username = username,
                PasswordHash = _hasher.HashPassword(null, password),
                Phone = phone,
                UserType = (int)UserTypeEnum.Client
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RegisterForManager(string username, string password)
        {
            if (await _db.Users.AnyAsync(u => u.Username == username))
                return false;

            var user = new User
            {
                Username = username,
                PasswordHash = _hasher.HashPassword(null, password),
                UserType = (int)UserTypeEnum.Manager
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
        public async Task<User> Authenticate(string phone, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Phone == phone && u.UserType == (int)UserTypeEnum.Client);
            if (user == null) return null;

            var result = _hasher.VerifyHashedPassword(null, user.PasswordHash, password);
            return result == PasswordVerificationResult.Success ? user : null;
        }

        /// <summary>
        /// Авторизация для менеджера
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<User> AuthenticateManager(string username, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username && u.UserType == (int)UserTypeEnum.Manager);
            if (user == null) return null;

            var result = _hasher.VerifyHashedPassword(null, user.PasswordHash, password);
            return result == PasswordVerificationResult.Success ? user : null;
        }
    }
}
