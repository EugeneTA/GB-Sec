using FluentValidation;

namespace CardStorageService.Models.Validators
{
    public class EmailValidator: AbstractValidator<string>
    {
        public EmailValidator()
        {
            RuleFor(x => x)
                .NotNull()
                .Length(5, 255)
                .EmailAddress();
        }
    }
}
