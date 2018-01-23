using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanning.DAL
{
    public class Participant
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int EventId { get; set; }
        public int UserId { get; set; }
    }
}