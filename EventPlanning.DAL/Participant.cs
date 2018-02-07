using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanning.DAL
{
    public class Participant
    {
        [ForeignKey("Event")]
        public int EventId { get; set; }

        public int UserId { get; set; }
    }
}