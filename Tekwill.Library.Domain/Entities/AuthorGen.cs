namespace Tekwill.Library.Domain.Entities
{
    public class AuthorGen
    {
        public int AuthorId { get; set; }
        public Author Author { get; set; }

        public int GenId { get; set; }
        public Gen Gen{ get; set; }
    }
}
