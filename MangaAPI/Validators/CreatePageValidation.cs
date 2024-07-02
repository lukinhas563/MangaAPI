using FluentValidation;
using MangaAPI.Models.Input;

namespace MangaAPI.Validators
{
    public class CreatePageValidation: AbstractValidator<PageInputModel>
    {
        public CreatePageValidation()
        {
            RuleFor(page => page.Order)
                .NotEmpty().WithMessage("Page is required.")
                .GreaterThanOrEqualTo(0).WithMessage("The value must be at least 0.");

            RuleFor(page => page.Url)
                .NotEmpty().WithMessage("An image is required.");
        }
    }
}
