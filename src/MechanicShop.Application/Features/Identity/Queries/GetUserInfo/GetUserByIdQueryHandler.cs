using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Identity.Dtos;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Identity.Queries.GetUserInfo
{
    public class GetUserByIdQueryHandler(
        IIdentityService identityService,
        ILogger<GetUserByIdQueryHandler> logger)
        : IRequestHandler<GetUserByIdQuery, Result<AppUserDto>>
    {
        private readonly IIdentityService _identityService = identityService;
        private readonly ILogger<GetUserByIdQueryHandler> _logger = logger;

        public async Task<Result<AppUserDto>> Handle(GetUserByIdQuery query, CancellationToken ct)
        {
            var getUserByIdResult = await _identityService.GetUserByIdAsync(query.UserId!);

            if(getUserByIdResult.IsError)
            {
                _logger.LogError("User with Id { UserId }{ErrorDetails}", query.UserId, getUserByIdResult.TopError.Description);

                return getUserByIdResult.Errors!;
            }

            return getUserByIdResult.Value;
        }
    }
}
