using FluentValidation;

namespace Tekwill.Library.Application.DTOs.Books
{
    public class CreateBookDtoValidator : AbstractValidator<CreateBookDto>
    {
        public CreateBookDtoValidator()
        {
            RuleFor(b => b.ISBN)
                .NotNull().WithMessage("ISBN is required")
                .NotEmpty().WithMessage("ISBN is required")
                .MaximumLength(50).WithMessage("ISBN max length is 50");
            
            RuleFor(b => b.Title)
                .NotNull().WithMessage("Title is required")
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(50).WithMessage("Title max length is 50");
        }
    }
}
