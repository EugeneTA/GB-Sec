using AutoMapper;
using Azure.Core;
using CardStorageService.Config;
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
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CardStorageService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        #region Services

        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;
        private readonly IMapper _mapper;
        private readonly IOptions<AccountControllerConfig> _configuration;
        private readonly IValidator<CreateAccountRequest> _createRequestValidator;
        private readonly IValidator<UpdateAccountRequest> _updateRequestValidator;

        #endregion

        #region Constructors

        public AccountController(
            IAccountService accountService, 
            ILogger<AccountController> logger,
            IMapper mapper,
            IOptions<AccountControllerConfig> configuration,
            IValidator<CreateAccountRequest> createRequestValidator,
            IValidator<UpdateAccountRequest> updateRequestValidator)
        {
            _accountService = accountService;
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;
            _createRequestValidator = createRequestValidator;
            _updateRequestValidator = updateRequestValidator;
        }

        #endregion

        #region Public methods

        [AllowAnonymous]
        [HttpPost("create")]
        [ProducesResponseType(typeof(Dictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public IActionResult CreateAccount([FromBody] CreateAccountRequest accountCreateRequest)
        {
            try
            {
                ValidationResult validationResult =  _createRequestValidator.Validate(accountCreateRequest);
                if (validationResult.IsValid)
                {
                    CreateAccountResponse accountCreateResponse = _accountService.CreateAccount(accountCreateRequest);
                    return Ok(accountCreateResponse);
                }
                else
                {
                    return Ok(new CreateAccountResponse
                    {
                        EMail = "",
                        FirstName = "",
                        SecondName = "",
                        LastName = "",
                        ErrorCode = (int)OperationErrorCodes.CreateError,
                        ErrorMessage = $"Create account parameters are not valid. {validationResult}"
                    });
                }
            }
            catch (Exception ex)
            {
                if (_configuration.Value.isLogEnabled)
                {
                    _logger.LogError($"Create account {accountCreateRequest.EMail} error. Error: {ex.Message}");
                }
                return Ok(new CreateAccountResponse
                {
                    EMail = "",
                    FirstName = "",
                    SecondName = "",
                    LastName = "",
                    ErrorCode = (int)OperationErrorCodes.DatabaseError,
                    ErrorMessage = $"Can create account {accountCreateRequest.EMail}. Database error."
                });
            }          
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
                        Account = _mapper.Map<AccountDto>(client),
                        ErrorCode = (int)OperationErrorCodes.OperationOk,
                        ErrorMessage = ""
                    }) ;
                }
            }
            catch (Exception ex)
            {
                if (_configuration.Value.isLogEnabled)
                {
                    _logger.LogError($"Get account by id {requestId} error. Error: {ex.Message}");
                }
                
                return Ok(new GetAccountResponse
                {
                    Account = null,
                    ErrorCode = (int)OperationErrorCodes.DatabaseError,
                    ErrorMessage = $"Can not get client by id={requestId}. Database error."
                });
            }

            return Ok(new GetAccountResponse
            {
                Account = null,
                ErrorCode = (int)OperationErrorCodes.ReadError,
                ErrorMessage = $"Can not get account by id={requestId}. Account not found."
            });
        }

        [HttpPost("update")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult Update([FromBody] UpdateAccountRequest request)
        {
            try
            {
                ValidationResult validationResult = _updateRequestValidator.Validate(request);
                if (validationResult.IsValid)
                {
                    var result = _accountService.UpdateAccount(request);

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
                else
                {
                    return Ok(new UpdateAccountResponse
                    {
                        Result = 0,
                        ErrorCode = (int)OperationErrorCodes.UpdateError,
                        ErrorMessage = $"Update account parameters are not valid. {validationResult}"
                    });
                }


            }
            catch (Exception ex)
            {
                if (_configuration.Value.isLogEnabled)
                {
                    _logger.LogError($"Update account by id {request.AccountId} error. Error: {ex.Message}");
                }
                return Ok(new UpdateAccountResponse
                {
                    Result = 0,
                    ErrorCode = (int)OperationErrorCodes.DatabaseError,
                    ErrorMessage = "Account was not updated. Database error."
                });
            }

            return Ok(new UpdateAccountResponse
            {
                Result = 0,
                ErrorCode = (int)OperationErrorCodes.UpdateError,
                ErrorMessage = "Account was not updated."
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
            catch (Exception ex)
            {
                if (_configuration.Value.isLogEnabled)
                {
                    _logger.LogError($"Delete account by id {requestId} error. Error: {ex.Message}");
                }
                return Ok(new DeleteAccountResponse
                {
                    Result = 0,
                    ErrorCode = (int)OperationErrorCodes.DatabaseError,
                    ErrorMessage = $"Can not delete account by id={requestId}. Database error."
                });
            }

            return Ok(new DeleteAccountResponse
            {
                Result = 0,
                ErrorCode = (int)OperationErrorCodes.DeleteError,
                ErrorMessage = $"Can not delete account by id={requestId}"
            });
        }

        #endregion
    }
}
