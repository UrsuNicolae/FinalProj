using FluentValidation;
using Tekwill.Library.Application.DTOs.Gens;

namespace Tekwill.Library.Application.DTOs.Categories
{
    public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
    {
        public CreateCategoryDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotNull().WithMessage("Name is required")
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(50).WithMessage("Name max length is 50");
        }
    }
}
