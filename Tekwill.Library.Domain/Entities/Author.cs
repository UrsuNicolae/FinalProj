namespace Tekwill.Library.Domain.Entities
{
    public class Author
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Site { get; set; }

        public ICollection<AuthorGen> AuthorGens{ get; set; }

        public ICollection<Book> Books { get; set; }
    }
}
