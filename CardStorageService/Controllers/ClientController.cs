using CardStorageService.Models.Requests.Client;
using CardStorageService.Models.Response.Client;
using CardStorageService.Models;
using CardStorageService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Azure.Core;
using CardStorageService.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using CardStorageService.Config;
using FluentValidation;
using CardStorageService.Models.Requests.Card;
using FluentValidation.Results;
using CardStorageService.Data;
using AutoMapper;

namespace CardStorageService.Controllers
{
    [Authorize]
    //[Route("api/[controller]")]
    [Route("api/client")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        #region Services

        private readonly ILogger<ClientController> _logger;
        private readonly IClientRepositoryService _repository;
        private readonly IMapper _mapper;
        private readonly IOptions<ClientControllerConfig> _configuration;
        private readonly IValidator<CreateClientRequest> _createRequestValidator;
        private readonly IValidator<UpdateClientRequest> _updateRequestValidator;

        #endregion

        #region Constructors

        public ClientController(
            ILogger<ClientController> logger,
            IClientRepositoryService repositoryService,
            IMapper mapper,
            IOptions<ClientControllerConfig> configuration,
            IValidator<CreateClientRequest> createRequestValidator,
            IValidator<UpdateClientRequest> updateRequestValidator)
        {
            _logger = logger;
            _repository = repositoryService;
            _mapper = mapper;
            _configuration = configuration;
            _createRequestValidator = createRequestValidator;
            _updateRequestValidator = updateRequestValidator;
        }

        #endregion

        #region Public methods

        [HttpPost("create")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult Create([FromBody] CreateClientRequest request)
        {
            try
            {
                ValidationResult validationResult = _createRequestValidator.Validate(request);
                if (validationResult.IsValid)
                {
                    // Add Automapper
                    var clientId = _repository.Create(_mapper.Map<Client>(request));

                    if (clientId > 0)
                    {
                        return Ok(new CreateClientResponse
                        {
                            ClientId = clientId,
                            ErrorCode = (int)OperationErrorCodes.OperationOk,
                            ErrorMessage = ""
                        });
                    }
                }
                else
                {
                    return Ok(new CreateClientResponse
                    {
                        ClientId = 0,
                        ErrorCode = (int)OperationErrorCodes.CreateError,
                        ErrorMessage = $"Create client parameters are not valid. {validationResult}"
                    });
                }
            }
            catch (Exception ex)
            {
                if (_configuration.Value.isLogEnabled)
                {
                    _logger.LogError($"Create client error ({request.Name} {request.SecondName} {request.Patronymic}. Error: {ex.Message}");
                }

                return Ok(new CreateClientResponse
                {
                    ClientId = 0,
                    ErrorCode = (int)OperationErrorCodes.DatabaseError,
                    ErrorMessage = "Client create error. Database error."
                });
            }

            return Ok(new CreateClientResponse
            {
                ClientId = 0,
                ErrorCode = (int)OperationErrorCodes.CreateError,
                ErrorMessage = "Client create error"
            });
        }

        [HttpGet("get")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult GetById([FromQuery] int requestId)
        {
            try
            {
                var client = _repository.GetById(requestId);

                if (client != null)
                {
                    return Ok(new GetClientResponse
                    {
                        Client = _mapper.Map<ClientDto>(client),
                        ErrorCode = (int)OperationErrorCodes.OperationOk,
                        ErrorMessage = ""
                    });
                }
            }
            catch (Exception ex)
            {
                if (_configuration.Value.isLogEnabled)
                {
                    _logger.LogError($"Get client by id {requestId} error. Error: {ex.Message}");
                }

                return Ok(new GetClientResponse
                {
                    Client = null,
                    ErrorCode = (int)OperationErrorCodes.DatabaseError,
                    ErrorMessage = $"Get client by id {requestId} error. Database error."
                });
            }

            return Ok(new GetClientResponse
            {
                Client = null,
                ErrorCode = (int)OperationErrorCodes.ReadError,
                ErrorMessage = $"Can not get client by id={requestId}"
            });
        }

        [HttpGet("getall")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            try
            {
                var clients = _repository.GetAll();

                if (clients != null)
                {
                    return Ok(new GetClientsResponse
                    {
                        // Add Automapper
                        Clients = clients.Select(client => _mapper.Map<ClientDto>(client)).ToList(),
                        ErrorCode = (int)OperationErrorCodes.OperationOk,
                        ErrorMessage = ""
                    });
                }
            }
            catch (Exception ex)
            {
                if (_configuration.Value.isLogEnabled)
                {
                    _logger.LogError($"Get clients error. Error: {ex.Message}");
                }

                return Ok(new GetClientsResponse
                {
                    Clients = null,
                    ErrorCode = (int)OperationErrorCodes.DatabaseError,
                    ErrorMessage = $"Get clients error. Database error."
                });
            }

            return Ok(new GetClientsResponse
            {
                Clients = null,
                ErrorCode = (int)OperationErrorCodes.ReadError,
                ErrorMessage = $"Can not get any client"
            });
        }

        [HttpPost("update")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult Update([FromBody] UpdateClientRequest request)
        {
            try
            {
                ValidationResult validationResult = _updateRequestValidator.Validate(request);
                if (validationResult.IsValid)
                {
                    // Add Automapper
                    var result = _repository.Update(_mapper.Map<Client>(request));

                    if (result > 0)
                    {
                        return Ok(new UpdateClientResponse
                        {
                            Result = result,
                            ErrorCode = (int)OperationErrorCodes.OperationOk,
                            ErrorMessage = ""
                        });
                    }
                }
                else
                {
                    return Ok(new UpdateClientResponse
                    {
                        Result = 0,
                        ErrorCode = (int)OperationErrorCodes.UpdateError,
                        ErrorMessage = $"Update client parameters are not valid. {validationResult}"
                    });
                }
            }
            catch (Exception ex)
            {
                if (_configuration.Value.isLogEnabled)
                {
                    _logger.LogError($"Update client by id {request.Id} error. Error: {ex.Message}");
                }
                return Ok(new UpdateClientResponse
                {
                    Result = 0,
                    ErrorCode = (int)OperationErrorCodes.DatabaseError,
                    ErrorMessage = $"Update client by id {request.Id} error. Database error."
                });
            }

            return Ok(new UpdateClientResponse
            {
                Result = 0,
                ErrorCode = (int)OperationErrorCodes.UpdateError,
                ErrorMessage = "Client update error"
            });
        }

        [HttpDelete("delete")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult Delete([FromQuery] int requestId)
        {
            try
            {
                var result = _repository.Delete(requestId);

                if (result > 0)
                {
                    return Ok(new DeleteClientResponse
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
                    _logger.LogError($"Delete client by id {requestId} error. Error: {ex.Message}");
                }
                return Ok(new DeleteClientResponse
                {
                    Result = 0,
                    ErrorCode = (int)OperationErrorCodes.DatabaseError,
                    ErrorMessage = $"Update client by id {requestId} error. Database error."
                });
            }

            return Ok(new DeleteClientResponse
            {
                Result = 0,
                ErrorCode = (int)OperationErrorCodes.DeleteError,
                ErrorMessage = $"Can not delete client by id={requestId}"
            });
        }

        #endregion
    }
}
