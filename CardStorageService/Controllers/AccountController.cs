using CardStorageService.Data;
using CardStorageService.Models;
using CardStorageService.Models.Dto;
using CardStorageService.Models.Requests.Account;
using CardStorageService.Models.Requests.Client;
using CardStorageService.Models.Response.Account;
using CardStorageService.Models.Response.Client;
using CardStorageService.Services;
using EmployeeService.Models.Validators;
using EmployeeService.Services.Repositories.Impl;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CardStorageService.Controllers
{
    [Authorize]
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
        public IActionResult CreateAccount([FromBody] CreateAccountRequest accountCreateRequest)
        {
            CreateAccountResponse accountCreateResponse = _accountService.CreateAccount(accountCreateRequest);
            return Ok(accountCreateResponse);
        }

        [HttpGet("get")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult GetById([FromQuery] int requestId)
        {
            try
            {
                var client = _accountService.GetAccount(requestId);

                if (client != null)
                {
                    return Ok(new GetAccountResponse
                    {
                        Account = new AccountDto()
                        {
                            AccountId = client.AccountId,
                            EMail = client.EMail,
                            FirstName = client.FirstName,
                            SecondName = client.SecondName,
                            LastName = client.LastName,
                        },
                        ErrorCode = (int)OperationErrorCodes.OperationOk,
                        ErrorMessage = ""
                    });
                }
            }
            catch
            {
            }

            return Ok(new GetAccountResponse
            {
                Account = null,
                ErrorCode = (int)OperationErrorCodes.ReadError,
                ErrorMessage = $"Can not get client by id={requestId}"
            });
        }

        [HttpPost("update")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult Update([FromBody] UpdateAccountRequest request)
        {
            try
            {
                var result = _accountService.UpdateAccount(new Account
                {
                    AccountId = request.AccountId,
                    EMail = request.EMail,
                    FirstName = request.FirstName,
                    SecondName = request.SecondName,
                    LastName = request.LastName,
                    Locked = request.Locked
                });

                if (result > 0)
                {
                    return Ok(new UpdateAccountResponse
                    {
                        Result = result,
                        ErrorCode = (int)OperationErrorCodes.OperationOk,
                        ErrorMessage = ""
                    });
                }
            }
            catch
            {
            }

            return Ok(new UpdateAccountResponse
            {
                Result = 0,
                ErrorCode = (int)OperationErrorCodes.UpdateError,
                ErrorMessage = "Account update error"
            });
        }

        [HttpDelete("delete")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult Delete([FromQuery] int requestId)
        {
            try
            {
                var result = _accountService.Delete(requestId);

                if (result > 0)
                {
                    return Ok(new DeleteAccountResponse
                    {
                        Result = result,
                        ErrorCode = (int)OperationErrorCodes.OperationOk,
                        ErrorMessage = ""
                    });
                }
            }
            catch
            {
            }

            return Ok(new DeleteAccountResponse
            {
                Result = 0,
                ErrorCode = (int)OperationErrorCodes.DeleteError,
                ErrorMessage = $"Can not delete account by id={requestId}"
            });
        }
    }
}
