using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanning.DAL
{
    public class Activity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public int MinParticipants { get; set; }
        public int MaxParticipants { get; set; }
        public List<EventActivity> EventActivities { get; set; }

        [ForeignKey("ActivityType")]
        public int? ActivityTypeId { get; set; }
    }
}
