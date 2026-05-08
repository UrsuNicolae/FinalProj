namespace Tekwill.Library.Application.DTOs.Books
{
    public class BookDto : CreateBookDto
    {
        public int Id { get; set; }

        public override string ToString()
        {
            return $"BookId: {Id}\n" +
                $"BookTitle: {Title}\n" +
                $"ISBN: {ISBN}";
        }

    }
}
