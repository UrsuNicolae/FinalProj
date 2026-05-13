namespace LibraryBot.Dtos
{
    public class KimiRequest
    {
        public string Model { get; set; }
        public double Temperature { get; set; }
        public KimiMessage[] Messages { get; set; }
    }

    public class KimiMessage
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }
}
