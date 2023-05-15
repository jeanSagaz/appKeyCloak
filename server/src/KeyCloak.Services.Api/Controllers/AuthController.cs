using Core.Notifications.Interfaces;
using KeyCloak.Services.Api.Models.Request;
using KeyCloak.Services.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace KeyCloak.Services.Api.Controllers
{
    [Route("api/v1/[controller]")]
    public class AuthController : BaseController
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService,
            IDomainNotifier notifier) : base(notifier)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost]
        [Route("{realm}/token")]
        public async Task<IActionResult> GenerateToken([FromBody] LoginRequest login, string realm)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var result = await _authenticationService.GetToken(login, realm);
            return CustomResponse(result);
        }

        [HttpPost]
        [Route("{realm}/refresh-token")]
        public async Task<IActionResult> GenerateRefreshToken([FromBody] RefreshTokenRequest refreshToken, string realm)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var result = await _authenticationService.GetRefreshToken(refreshToken.RefreshToken, realm);
            return CustomResponse(result);
        }
    }
}
