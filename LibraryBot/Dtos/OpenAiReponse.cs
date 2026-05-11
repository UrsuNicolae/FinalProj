namespace LibraryBot.Dtos
{
    public class OpenAiReponse
    {
        public string Id { get; set; }

        public List<OpenAiChoices> Choices { get; set; }
    }

    public class OpenAiChoices
    {
        public OpenAiChoicesMessage Message { get; set; }
    }

    public class OpenAiChoicesMessage
    {
        public string Content { get; set; }
    }
}
