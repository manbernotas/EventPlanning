using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.DAL
{
    public class LoginAttempt
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserId { get; set; }
        public string IpAddress { get; set; }
        public DateTime LoginTime { get; set; }
    }
}
