using CardStorageService.Models.Requests.Authentication;
using FluentValidation;

namespace EmployeeService.Models.Validators
{
    public class AuthenticationRequestValidator : AbstractValidator<AuthenticationRequest>
    {
        public AuthenticationRequestValidator()
        {
            RuleFor(x => x.Login).
                NotNull().
                Length(5, 255)
                .EmailAddress();
            RuleFor(x => x.Password)
                .NotNull()
                .Length(5, 128);
        }
    }
}
