using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace UserManagement.DAL
{
    public class Repository
    {
        private UserContext context;

        public Repository(UserContext context)
        {
            this.context = context;
        }

        public IQueryable<User> GetUsers()
        {
            return context.User;
        }

        public bool SaveUser(User user)
        {
            try
            {
                context.User.Add(user);
                context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return false;
            }

            return true;
        }

        public int GetUserId(string user)
        {
            return context.User.SingleOrDefault(u => u.UserName == user || u.Email == user).Id;
        }

        public User GetUser(int userId)
        {
            return context.User.SingleOrDefault(u => u.Id == userId);
        }

        public bool SaveLoginRecord(LoginAttempt login)
        {
            try
            {
                context.LoginAttempt.Add(login);
                context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return false;
            }

            return true;
        }
    }
}
