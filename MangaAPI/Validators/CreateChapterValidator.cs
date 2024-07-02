using FluentValidation;
using MangaAPI.Models.Input;

namespace MangaAPI.Validators
{
    public class CreateChapterValidator: AbstractValidator<ChapterInputModel>
    {
        public CreateChapterValidator()
        {
            RuleFor(chapter => chapter.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MinimumLength(3).WithMessage("Title must be at least 3 characters long.")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

            RuleFor(chapter => chapter.Number)
                .NotEmpty().WithMessage("Number is required.")
                .GreaterThanOrEqualTo(0).WithMessage("The value must be at least 0.");

            RuleFor(chapter => chapter.Release)
                .NotEmpty().WithMessage("Release is required.");
        }
    }
}
