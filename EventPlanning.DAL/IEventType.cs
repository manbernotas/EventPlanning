using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanning.DAL
{
    public interface IEventType
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        int Id { get; set; }

        string Name { get; set; }
        string Description { get; set; }
        int MinParticipants { get; set; }
        int MaxParticipants { get; set; }
        int Quantity { get; set; }
    }
}