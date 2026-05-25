using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace MechanicShop.Application.Features.Identity.Queries.RefreshTokens
{
    public class RefreshTokenQueryHandler(
        IAppDbContext context,
        ILogger<RefreshTokenQueryHandler> logger,
        IIdentityService identityService,
        ITokenProvider tokenProvider)
        : IRequestHandler<RefreshTokenQuery, Result<TokenResponse>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<RefreshTokenQueryHandler> _logger = logger;
        private readonly IIdentityService _identityService = identityService;
        private readonly ITokenProvider _tokenProvider = tokenProvider;

        public async Task<Result<TokenResponse>> Handle(RefreshTokenQuery query, CancellationToken ct)
        {
            var principals = _tokenProvider.GetPrincipalFromExpiredToken(query.ExpiredAccessToken);

            if(principals is null)
            {
                _logger.LogError("Expired access token is not valid");

                return ApplicationErrors.ExpiredAccessTokenInvalid;
            }

            var userId = principals.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(userId is null)
            {
                _logger.LogError("Invalid userId claim");

                return ApplicationErrors.UserIdClaimInvalid;
            }

            var getUserResult = await _identityService.GetUserByIdAsync(userId);

            if(getUserResult.IsError)
            {
                _logger.LogError("Get user by id error occurred: {ErrorDescription}", getUserResult.TopError.Description);

                return getUserResult.Errors!;
            }

            var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == query.RefreshToken && t.UserId == userId, ct);

            if (refreshToken is null || refreshToken.ExpiresOnUtc < DateTime.UtcNow)
            {
                _logger.LogError("Refresh token has expired");

                return ApplicationErrors.RefreshTokenExpired;
            }

            var generateTokenResult = await _tokenProvider.GenerateJwtTokenAsync(getUserResult.Value, ct);

            if (generateTokenResult.IsError)
            {
                _logger.LogError("Generate token error occurred: {ErrorDescription}", generateTokenResult.TopError.Description);

                return generateTokenResult.Errors!;
            }

            return generateTokenResult.Value; ;
        }
    }
}
