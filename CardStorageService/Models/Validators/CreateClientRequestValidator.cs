using CardStorageService.Models.Requests.Client;
using FluentValidation;

namespace CardStorageService.Models.Validators
{
    public class CreateClientRequestValidator : AbstractValidator<CreateClientRequest>
    {
        public CreateClientRequestValidator()
        {
            RuleFor(o => o.Name)
                .NotNull()
                .Length(1, 255);
            RuleFor(o => o.Patronymic)
                .NotNull()
                .Length(1, 255);
        }
    }
}
