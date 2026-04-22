using Tekwill.Library.Application.DTOs.Books;

namespace Tekwill.Library.Application.DTOs.Authors
{
    public class CreateAuthorDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Site { get; set; }

        public ICollection<BookDto> Books { get; set; }
    }
}
