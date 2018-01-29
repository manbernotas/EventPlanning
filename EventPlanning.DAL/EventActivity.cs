using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanning.DAL
{
    public class EventActivity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Event")]
        public int EventId { get; set; }
        [ForeignKey("Activity")]
        public int ActivityId { get; set; }
        public Event Event { get; set; }
        public Activity Activity { get; set; }
    }
}
