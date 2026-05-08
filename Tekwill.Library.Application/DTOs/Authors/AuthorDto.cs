namespace Tekwill.Library.Application.DTOs.Authors
{
    public class AuthorDto : CreateAuthorDto
    {
        public int Id { get; set; }

        public override string ToString()
        {
            return $"AuthorId: {Id}\n" +
                $"AuthorName: {FirstName} + {LastName}\n" +
                $"Birth: {BirthDate.ToString("yyyy:mm:dd")}\n" +
                $"Site: {Site}";
        }
    }
}
