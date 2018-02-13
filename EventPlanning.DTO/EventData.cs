namespace EventPlanning.Model
{
    public class EventData
    {
        public string Title { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string[] Address { get; set; }
        public string[] Activities { get; set; }
        public string Type { get; set; }
    }
}
