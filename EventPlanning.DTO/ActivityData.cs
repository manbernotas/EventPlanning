using System;
using System.Collections.Generic;
using System.Text;

namespace EventPlanning.Model
{
    public class ActivityData
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int MinParticipants { get; set; }
        public int MaxParticipants { get; set; }
        public string ActivityType { get; set; }
    }
}
