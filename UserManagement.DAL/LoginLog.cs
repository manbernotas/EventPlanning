using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.DAL
{
    public class LoginLog
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserId { get; set; }
        public DateTime LoginTime { get; set; }
    }
}
