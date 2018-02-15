using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanning.DAL
{
    public class Event
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public enum EventTypes : int { Public, Private }

        public int UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public Address Address { get; set; }
        public int Type { get; set; }
        public IEnumerable<EventActivity> EventActivities { get; set; }
        public IEnumerable<Participant> Participants { get; set; }
        public IEnumerable<Invitation> Invitations { get; set; }
    }
}