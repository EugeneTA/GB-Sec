using CardStorageService.Models;
using CardStorageService.Models.Requests.Authentication;
using CardStorageService.Models.Response.Authentication;
using CardStorageService.Services;
using EmployeeService.Models.Validators;
using EmployeeService.Services.Repositories.Impl;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CardStorageService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(
            IAccountService accountService)
        {
            _accountService = accountService;
        }

        [AllowAnonymous]
        [HttpPost("create")]
        [ProducesResponseType(typeof(Dictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public IActionResult Login([FromBody] AccountCreateRequest accountCreateRequest)
        {
            AccountCreateResponse accountCreateResponse = _accountService.CreateAccount(accountCreateRequest);
            return Ok(accountCreateResponse);
        }
    }
}
