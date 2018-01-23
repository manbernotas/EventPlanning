using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanning.DAL
{
    public class Event
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Duration { get; set; }
        public string Address { get; set; }
        public int MinParticipants { get; set; }
        public int MaxParticipants { get; set; }
        public List<EventActivity> Activities { get; set; }
        public List<Participant> Participants { get; set; }
    }
}