using System;

namespace UserManagement.DTO
{
    public class PartialUser
    {
        public PartialUser(string userName, string email, int userId)
        {
            UserName = userName;
            Email = email;
            UserId = userId;
        }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}
