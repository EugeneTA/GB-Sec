using CardStorageService.Models.Requests.Account;
using CardStorageService.Models.Requests.Authentication;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace CardStorageService.Models.Validators
{
    public class CreateAccountRequestValidator: AbstractValidator<CreateAccountRequest>
    {
        public CreateAccountRequestValidator()
        {
            RuleFor(x => x.EMail).
                NotNull().
                Length(5, 255)
                .EmailAddress();
            RuleFor(x => x.Password)
                .NotNull()
                .Length(5, 128);
            RuleFor(x => x.FirstName)
                .NotNull()
                .Length(1, 255);
        }
    }
}
