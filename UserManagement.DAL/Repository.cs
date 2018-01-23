using Microsoft.EntityFrameworkCore;
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
    }
}
