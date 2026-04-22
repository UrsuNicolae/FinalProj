using FluentValidation;
using Tekwill.Library.Application.DTOs.Books;

namespace Tekwill.Library.Application.DTOs.Authors
{
    public class CreateAuthorDtoValidator : AbstractValidator<CreateAuthorDto>
    {
        public CreateAuthorDtoValidator(IValidator<CreateBookDto> bookValidator)
        {
            RuleFor(a => a.FirstName)
                .NotNull().WithMessage("FirstName is required")
                .NotEmpty().WithMessage("FirstName is required")
                .MaximumLength(50).WithMessage("FirstName max length is 50");
            RuleFor(a => a.LastName)
                .NotNull().WithMessage("LastName is required")
                .NotEmpty().WithMessage("LastName is required")
                .MaximumLength(50).WithMessage("LastName max length is 50");
            RuleFor(a => a.BirthDate)
                .NotNull().WithMessage("Birthdate is required");
            RuleFor(a => a.Site)
                .NotNull().WithMessage("Site is required")
                .NotEmpty().WithMessage("Site is required")
                .MaximumLength(250).WithMessage("Site max length is 250");
            RuleForEach(a => a.Books)
                .SetValidator(bookValidator);

        }
    }
}
