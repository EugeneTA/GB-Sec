using CardStorageService.Models;
using CardStorageService.Models.Dto;
using CardStorageService.Models.Requests.Authentication;
using CardStorageService.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;

namespace CardStorageService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly IAuthenticateService _authenticateService;
        private readonly IValidator<AuthenticationRequest> _authenticationRequestValidator;

        public AuthenticateController(
            IAuthenticateService authenticateService,
            IValidator<AuthenticationRequest> authenticationRequestValidator)
        {
            _authenticateService = authenticateService;
            _authenticationRequestValidator = authenticationRequestValidator;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(Dictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public IActionResult Login([FromBody] AuthenticationRequest authenticationRequest)
        {
            ValidationResult validationResult = _authenticationRequestValidator.Validate(authenticationRequest);
            if (validationResult.IsValid == false)
                return BadRequest(validationResult.ToDictionary());

            AuthenticationResponse authenticationResponse = _authenticateService.Login(authenticationRequest);
            if (authenticationResponse.Status == AuthenticationStatus.Success)
            {
                Response.Headers.Add("X-Session-Token", authenticationResponse.Session.SessionToken);
            }
            return Ok(authenticationResponse);
        }


        [HttpGet("session")]
        public IActionResult GetSession()
        {
            // Authorization: Bearer XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

            var authorizationHeader = Request.Headers[HeaderNames.Authorization];

            if (AuthenticationHeaderValue.TryParse(authorizationHeader, out var headerValue))
            {
                var scheme = headerValue.Scheme; // Bearer
                var sessionToken = headerValue.Parameter; // Token

                if (string.IsNullOrEmpty(sessionToken))
                    return Unauthorized();

                SessionDto sessionDto = _authenticateService.GetSession(sessionToken);
                if (sessionDto == null)
                    return Unauthorized();

                return Ok(sessionDto);

            }
            return Unauthorized();
        }

    }
}
