using CardStorageService.Models.Requests.Card;
using FluentValidation;

namespace CardStorageService.Models.Validators
{
    public class UpdateCardRequestValidator : AbstractValidator<UpdateCardRequest>
    {
        public UpdateCardRequestValidator()
        {
            RuleFor(o => o.ClientId)
                .NotNull()
                .NotEmpty()
                .GreaterThan(0);
            RuleFor(o => o.Number)
                .NotNull()
                .Length(16, 20);
            RuleFor(o => o.Name)
                .NotNull()
                .Length(1, 50);
            RuleFor(o => o.CVV2)
                .NotNull()
                .Length(3, 50);
            RuleFor(o => o.ExpDate)
                .NotNull()
                .NotEmpty()
                .GreaterThan(DateTime.Now);
        }
    }
}
