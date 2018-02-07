using System;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UserManagement.DAL;
using UserManagement.Model;
using UserManagement.DTO;

namespace UserManagement.BL
{
    public class UserManager
    {
        private UserContext context;

        private Repository repository;

        public UserManager(UserContext context)
        {
            this.context = context;
            repository = new Repository(this.context);
        }

        /// <summary>
        /// Returns UserNames of all users
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllUserNames()
        {
            return repository.GetUsers().Select(u => u.UserName).ToList();
        }

        /// <summary>
        /// Returns all users
        /// </summary>
        /// <returns></returns>
        public List<User> GetUsers()
        {
            return repository.GetUsers().ToList();
        }

        /// <summary>
        /// Returns partial user information
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PartialUser GetPartialUser(int userId)
        {
            var user = repository.GetUser(userId);

            if (user == null)
            {
                return null;
            }

            return new PartialUser(user.UserName, user.Email, user.Id);
        }

        /// <summary>
        /// Returns user Id by username or email
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int GetUserId(string user)
        {
            return repository.GetUserId(user);
        }

        /// <summary>
        /// Generates random salt string for password Hashing
        /// </summary>
        /// <returns></returns>
        public string GetSalt()
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return Convert.ToBase64String(salt);
        }

        /// <summary>
        /// Generates password hash
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public string GetPasswordHash(string password, string salt)
        {
            byte[] bSalt = Encoding.Unicode.GetBytes(salt);
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: bSalt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hashed;
        }

        /// <summary>
        /// Validates password
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool IsPasswordValid(UserData user)
        {
            var users = repository.GetUsers();
            var userInDatabase = users.FirstOrDefault(u => u.UserName == user.Name);

            if (userInDatabase != null && user.Password != null)
            {
                var passwordHash = GetPasswordHash(user.Password, userInDatabase.Salt);

                if (userInDatabase.Password == passwordHash)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Creates new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool CreateUser(UserData user)
        {
            if (repository.GetUsers().FirstOrDefault(u => u.UserName == user.Name) == null)
            {
                var salt = GetSalt();
                var passwordHash = GetPasswordHash(user.Password, salt);
                var newUser = new User()
                {
                    UserName = user.Name,
                    Password = passwordHash,
                    Email = user.Email,
                    Salt = salt,
                };

                try
                {
                    return repository.SaveUser(newUser);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return false;
        }
    }
}
