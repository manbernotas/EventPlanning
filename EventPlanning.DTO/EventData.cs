namespace EventPlanning.Model
{
    public class EventData
    {
        public string Name { get; set; }
        public string Date { get; set; }
        public string Duration { get; set; }
        public string Address { get; set; }
        public int MinParticipants { get; set; }
        public int MaxParticipants { get; set; }
        public string[] Activities { get; set; }
    }
}
