using System;
using System.Collections.Generic;
using System.Text;

namespace EventPlanning.DAL
{
    public class PartialEvent
    {
        public string Title { get; set; }
        public string Creator { get; set; }
        public int Participants { get; set; }
        public int MaxParticipants { get; set; }
    }
}
