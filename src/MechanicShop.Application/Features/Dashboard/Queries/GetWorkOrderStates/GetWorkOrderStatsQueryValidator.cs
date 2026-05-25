using FluentValidation;

namespace MechanicShop.Application.Features.Dashboard.Queries.GetWorkOrderStates
{
    public sealed class GetWorkOrderStatsQueryValidator : AbstractValidator<GetWorkOrderStatsQuery>
    {
        public GetWorkOrderStatsQueryValidator()
        {
            RuleFor(request => request.Date)
                .NotEmpty()
                .WithErrorCode("Date_Is_Required")
                .WithMessage("Date is required.");
        }
    }
}
