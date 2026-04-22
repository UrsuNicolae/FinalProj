using System.Text.Json.Serialization;
using Tekwill.Library.Application.DTOs.Books;

namespace Tekwill.Library.Application.DTOs.Authors
{
    public class UpdateAuthorDto : AuthorDto
    {
        [JsonIgnore]
        public new ICollection<BookDto>? Books { get; set; }
    }
}
