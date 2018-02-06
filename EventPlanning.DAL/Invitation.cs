using System;
using System.Collections.Generic;
using System.Text;

namespace EventPlanning.DAL
{
    public class Invitation
    {
        public int EventId { get; set; }
        public int UserId { get; set; }
        public string SentTo { get; set; }
        public DateTime DateSent { get; set; }
        public string Status { get; set; }
    }
}
