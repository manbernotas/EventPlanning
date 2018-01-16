namespace EventPlanning.DAL
{
    public class Activity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MinParticipants { get; set; }
        public int MaxParticipants { get; set; }
        public int Quantity { get; set; }
    }
}
