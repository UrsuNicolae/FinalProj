namespace Tekwill.Library.Domain.Entities
{
    public class Gen
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<AuthorGen> AuthorGens { get; set; }
    }
}
