using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanning.DAL
{
    public class Participant
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Nickname { get; set; }
        public string Password { get; set; }
    }
}