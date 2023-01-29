using CardStorageService.Models.Requests.Client;
using CardStorageService.Models.Response.Client;
using CardStorageService.Models;
using CardStorageService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Azure.Core;

namespace CardStorageService.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/client")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        #region Services

        private readonly ILogger<ClientController> _logger;
        private readonly IClientRepositoryService _repository;

        #endregion

        #region Constructors

        public ClientController(
            ILogger<ClientController> logger,
            IClientRepositoryService repositoryService)
        {
            _logger = logger;
            _repository = repositoryService;
        }

        #endregion

        #region Public methods

        [HttpPost("create")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult Create([FromBody] CreateClientRequest request)
        {
            try
            {
                var clientId = _repository.Create(new Data.Client
                {
                    Name = request.Name,
                    SecondName = request.SecondName,
                    Patronymic = request.Patronymic,
                });

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
            catch
            {
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
                        Client = new ClientDto()
                        {
                            ClientId = client.ClientId,
                            Name = client.Name,
                            SecondName = client.SecondName,
                            Patronymic = client.Patronymic,
                        },
                        ErrorCode = (int)OperationErrorCodes.OperationOk,
                        ErrorMessage = ""
                    });
                }
            }
            catch
            {
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
                    var clientsDto = new List<ClientDto>();

                    foreach (var client in clients)
                    {
                        clientsDto.Add(new ClientDto()
                        {
                            ClientId = client.ClientId,
                            Name = client.Name,
                            SecondName = client.SecondName,
                            Patronymic = client.Patronymic,
                        });
                    };

                    return Ok(new GetClientsResponse
                    {
                        Clients = clientsDto,
                        ErrorCode = (int)OperationErrorCodes.OperationOk,
                        ErrorMessage = ""
                    });
                }
            }
            catch
            {
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
                var result = _repository.Update(new Data.Client
                {
                    ClientId = request.Id,
                    Name = request.Name,
                    SecondName = request.SecondName,
                    Patronymic = request.Patronymic,
                });

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
            catch
            {
            }

            return Ok(new UpdateClientResponse
            {
                Result = 0,
                ErrorCode = (int)OperationErrorCodes.UpdateError,
                ErrorMessage = "Client update error"
            });
        }

        [HttpPost("delete")]
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
            catch
            {
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
