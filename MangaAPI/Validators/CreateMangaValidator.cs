using FluentValidation;
using MangaAPI.Models.Input;

namespace MangaAPI.Validators
{
    public class CreateMangaValidator: AbstractValidator<MangaInputModel>
    {
        public CreateMangaValidator()
        {
            RuleFor(manga => manga.Cover)
                .NotEmpty().WithMessage("An image is required.");

            RuleFor(manga => manga.Banner)
                .NotEmpty().WithMessage("An image is required.");

            RuleFor(manga => manga.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MinimumLength(3).WithMessage("Title must be at least 3 characters long.")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

            RuleFor(manga => manga.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MinimumLength(3).WithMessage("Description must be at least 3 characters long.")
                .MaximumLength(200).WithMessage("Description cannot exceed 200 characters.");

            RuleFor(manga => manga.Tags)
                .NotEmpty().WithMessage("Tags is required.");

            RuleFor(manga => manga.Release)
                .NotEmpty().WithMessage("Release is required.");

            RuleFor(manga => manga.Status)
                .NotEmpty().WithMessage("Status is required.")
                .MinimumLength(3).WithMessage("Status must be at least 3 characters long.")
                .MaximumLength(200).WithMessage("Status cannot exceed 200 characters.");

            RuleFor(manga => manga.Authors)
                .NotEmpty().WithMessage("Author is required.");

            RuleFor(manga => manga.Artists)
                .NotEmpty().WithMessage("Artist is required.");
        }
    }
}
