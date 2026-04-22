using FluentValidation;

namespace Tekwill.Library.Application.DTOs.Gens
{
    public class CreateGenDtoValidator : AbstractValidator<CreateGenDto>
    {
        public CreateGenDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotNull().WithMessage("Name is required")
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(50).WithMessage("Name max length is 50");
        }
    }
}
