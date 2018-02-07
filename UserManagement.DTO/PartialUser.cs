using System;

namespace UserManagement.DTO
{
    public class PartialUser
    {
        public PartialUser(string userName, string email)
        {
            UserName = userName;
            Email = email;
        }

        public string UserName { get; set; }
        public string Email { get; set; }
    }
}
