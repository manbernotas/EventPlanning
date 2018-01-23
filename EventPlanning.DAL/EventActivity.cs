using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanning.DAL
{
    public class EventActivity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int EventId { get; set; }
        public int ActivityId { get; set; }
    }
}
