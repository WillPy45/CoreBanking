using CoreBankingWebApplication.Infraestructure.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreBankingWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly HttpContext _httpContext;
        private readonly IProfileSessionManager _profileSessionManager;

        public LoginController(HttpContext httpContext, IProfileSessionManager profileSessionManager) 
        {
            _httpContext = httpContext;
            _profileSessionManager = profileSessionManager;
        }


        [HttpGet("/login")]
        public async Task<IActionResult> LogIn(LogToProfileRequest request)
        {
            var createSessionResult = await _profileSessionManager.CreateSession(request);

            if (createSessionResult.IsFailure)
            {
                switch (createSessionResult.Error.Code)
                {
                    case SessionCreateError.InvalidPassword:
                        return StatusCode(StatusCodes.Status401Unauthorized, "Invalid password");
                    case SessionCreateError.ProfileNotFound:
                        return StatusCode(StatusCodes.Status404NotFound, "Profile not found");
                    case SessionCreateError.SessionRiskManagerRestriction:
                        return StatusCode(StatusCodes.Status403Forbidden, "Session attempts is locked for security reasons");
                }
            }

            var sessionValue = createSessionResult.Value;

            return Ok(new SessionResponseDto() { 
                AsociatedProfileId = sessionValue.AsociatedProfileId,
                SessionToken = sessionValue.SessionToken,
                SessionStartUtc = sessionValue.SessionStartUtc,
                SessionEndUtc = sessionValue.SessionEndUtc
            }          
            );
        }

        [HttpGet("/logout")]
        public async Task<IActionResult> LogOut()
        {
            throw new NotImplementedException();
        }

    }
}
