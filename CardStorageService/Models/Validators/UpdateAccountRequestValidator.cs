using CardStorageService.Models.Requests.Account;
using FluentValidation;

namespace CardStorageService.Models.Validators
{
    public class UpdateAccountRequestValidator: AbstractValidator<UpdateAccountRequest>
    {
        public UpdateAccountRequestValidator()
        {
            RuleFor(x => x.EMail)
                .NotNull()
                .Length(5, 255)
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
