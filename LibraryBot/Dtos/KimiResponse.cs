namespace LibraryBot.Dtos
{
    public class KimiResponse
    {
        public string Id { get; set; }

        public List<KimiChoice> Choices { get; set; }
    }

    public class KimiChoice
    {
        public KimiChoiceMessage Message { get; set; }
    }

    public class KimiChoiceMessage
    {
        public string Content { get; set; }
    }
}
