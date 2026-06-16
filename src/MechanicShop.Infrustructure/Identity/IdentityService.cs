using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Identity.Dtos;
using MechanicShop.Domain.Common.Results;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace MechanicShop.Infrustructure.Identity
{
    public class IdentityService(
        UserManager<AppUser> userManager,
        IUserClaimsPrincipalFactory<AppUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService) : IIdentityService
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly IUserClaimsPrincipalFactory<AppUser> _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        private readonly IAuthorizationService _authorizationService = authorizationService;

        public async Task<Result<AppUserDto>> AuthenticateAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if(user is null)
                return Error.NotFound("User_Not_Found", $"User with email {UtilityService.MaskEmail(email)} not found");

            if(!user.EmailConfirmed)
                return Error.Conflict("Email_Not_Confirmed", $"email '{UtilityService.MaskEmail(email)}' not confirmed");

            if(!await _userManager.CheckPasswordAsync(user, password))
                return Error.Conflict("Invalid_Login_Attempt", "Email / Password are incorrect");

            return new AppUserDto(user.Id, user.Email!, await _userManager.GetRolesAsync(user), await _userManager.GetClaimsAsync(user));
        }

        public async Task<Result<AppUserDto>> GetUserByIdAsync(string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId) ?? throw new InvalidOperationException(nameof(UserId));

            var roles = await _userManager.GetRolesAsync(user);

            var claims = await _userManager.GetClaimsAsync(user);

            return new AppUserDto(UserId, user.Email!, roles, claims);
        }

        public async Task<string> GetUserNameAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            return user?.UserName!;
        }
    }
}
