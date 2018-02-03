using System;
using System.Collections.Generic;
using System.Text;

namespace EventPlanning.DAL
{
    public class Invitation
    {
        public int EventId { get; set; }
        public int UserId { get; set; }
        public string SendTo { get; set; }
        public string Status { get; set; }
    }
}
