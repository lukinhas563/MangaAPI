using FluentValidation;
using MangaAPI.Models.Input;

namespace MangaAPI.Validators
{
    public class CreateUserValidator: AbstractValidator<UserInputModel>
    {
        public CreateUserValidator()
        {
            RuleFor(user => user.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(3).WithMessage("Name must be at least 3 characters long.")
                .MaximumLength(200).WithMessage("Name cannot exceed 200 characters.");

            RuleFor(user => user.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MinimumLength(3).WithMessage("Last name must be at least 3 characters long.")
                .MaximumLength(200).WithMessage("Last name cannot exceed 200 characters.");

            RuleFor(user => user.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
                .MaximumLength(200).WithMessage("Username cannot exceed 200 characters.");

            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Email is required.")
                .MinimumLength(3).WithMessage("Email must be at least 3 characters long.")
                .MaximumLength(200).WithMessage("Email cannot exceed 200 characters.")
                .EmailAddress().WithMessage("Please enter a valid email address.");

            RuleFor(user => user.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .MaximumLength(200).WithMessage("Password cannot exceed 200 characters.");
        }
    }
}
