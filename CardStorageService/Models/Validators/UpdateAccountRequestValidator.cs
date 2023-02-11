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
        }
    }
}
