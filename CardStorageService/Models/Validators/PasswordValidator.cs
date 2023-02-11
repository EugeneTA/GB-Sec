using FluentValidation;

namespace CardStorageService.Models.Validators
{
    public class PasswordValidator: AbstractValidator<string>
    {
        public PasswordValidator()
        {
            RuleFor(x => x)
                .NotNull()
                .NotEmpty()
                .Length(5, 128);
        }
    }
}
