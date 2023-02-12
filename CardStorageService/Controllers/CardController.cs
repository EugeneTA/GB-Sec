using AutoMapper;
using Azure.Core;
using CardStorageService.Config;
using CardStorageService.Data;
using CardStorageService.Models;
using CardStorageService.Models.Dto;
using CardStorageService.Models.Requests.Account;
using CardStorageService.Models.Requests.Card;
using CardStorageService.Models.Response.Card;
using CardStorageService.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CardStorageService.Controllers
{
    [Authorize]
    //[Route("api/[controller]")]
    [Route("api/card")]
    [ApiController]
    public class CardController : ControllerBase
    {
        #region Services

        private readonly ILogger<CardController> _logger;
        private readonly ICardRepositoryService _repository;
        private readonly IMapper _mapper;
        private readonly IOptions<CardControllerConfig> _configuration;
        private readonly IValidator<CreateCardRequest> _createRequestValidator;
        private readonly IValidator<UpdateCardRequest> _updateRequestValidator;

        #endregion

        #region Constructors

        public CardController(
            ILogger<CardController> logger,
            ICardRepositoryService repositoryService,
            IMapper mapper,
            IOptions<CardControllerConfig> configuration,
            IValidator<CreateCardRequest> createRequestValidator,
            IValidator<UpdateCardRequest> updateRequestValidator)
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
        public IActionResult Create([FromBody] CreateCardRequest request)
        {
            try
            {
                ValidationResult validationResult = _createRequestValidator.Validate(request);
                if (validationResult.IsValid)
                {
                    // Add automapper
                    var cardId = _repository.Create(_mapper.Map<Card>(request));

                    if (cardId != Guid.Empty)
                    {
                        return Ok(new CreateCardResponse
                        {
                            CardId = cardId,
                            ErrorCode = (int)OperationErrorCodes.OperationOk,
                            ErrorMessage = ""
                        });
                    }
                }
                else
                {
                    return Ok(new CreateCardResponse
                    {
                        CardId = Guid.Empty,
                        ErrorCode = (int)OperationErrorCodes.CreateError,
                        ErrorMessage = $"Create card parameters are not valid. {validationResult}"
                    });
                }
            }
            catch (Exception ex)
            {
                if (_configuration.Value.isLogEnabled)
                {
                    _logger.LogError($"Create error card for client id {request.ClientId}. Error: {ex.Message}");
                }
                return Ok(new CreateCardResponse
                {
                    CardId = Guid.Empty,
                    ErrorCode = (int)OperationErrorCodes.DatabaseError,
                    ErrorMessage = "Card create error. Database error."
                });
            }

            return Ok(new CreateCardResponse
            {
                CardId = Guid.Empty,
                ErrorCode = (int)OperationErrorCodes.CreateError,
                ErrorMessage = "Card create error"
            });
        }

        [HttpGet("get")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult GetById([FromQuery] Guid requestId)
        {
            try
            {
                var card = _repository.GetById(requestId);

                if (card != null)
                {
                    return Ok(new GetCardResponse
                    {
                        Card = _mapper.Map<CardDto>(card),
                        ErrorCode = (int)OperationErrorCodes.OperationOk,
                        ErrorMessage = ""
                    });
                }
            }
            catch (Exception ex)
            {
                if (_configuration.Value.isLogEnabled)
                {
                    _logger.LogError($"Get card for id {requestId} error. Error: {ex.Message}");
                }
                return Ok(new GetCardResponse
                {
                    Card = null,
                    ErrorCode = (int)OperationErrorCodes.DatabaseError,
                    ErrorMessage = $"Get card with id {requestId} error. Database error."
                });
            }

            return Ok(new GetCardResponse
            {
                Card = null,
                ErrorCode = (int)OperationErrorCodes.ReadError,
                ErrorMessage = $"Can not get card by id={requestId}"
            });
        }

        [HttpGet("getall")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            try
            {
                var cards = _repository.GetAll();

                if (cards != null)
                {
                    return Ok(new GetCardsResponse
                    {
                        // Add Automapper
                        Cards = cards.Select(card => _mapper.Map<CardDto>(card)).ToList(),
                        ErrorCode = (int)OperationErrorCodes.OperationOk,
                        ErrorMessage = ""
                    });
                }
            }
            catch (Exception ex)
            {
                if (_configuration.Value.isLogEnabled)
                {
                    _logger.LogError($"Get cards error. Error: {ex.Message}");
                }
                return Ok(new GetCardsResponse
                {
                    Cards = null,
                    ErrorCode = (int)OperationErrorCodes.DatabaseError,
                    ErrorMessage = $"Get cards error. Database error."
                });
            }

            return Ok(new GetCardsResponse
            {
                Cards = null,
                ErrorCode = (int)OperationErrorCodes.ReadError,
                ErrorMessage = $"Can not get any card"
            });
        }

        [HttpGet("get-by-client-id")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult GetByclientId([FromQuery] int requestId)
        {
            try
            {
                var cards = _repository.GetByClientId(requestId);

                if (cards != null)
                {
                    return Ok(new GetCardsResponse
                    {
                        // Add Automapper
                        Cards = cards.Select(card => _mapper.Map<CardDto>(card)).ToList(),
                        ErrorCode = (int)OperationErrorCodes.OperationOk,
                        ErrorMessage = ""
                    });
                }
            }
            catch (Exception ex)
            {
                if (_configuration.Value.isLogEnabled)
                {
                    _logger.LogError($"Get cards for client with id {requestId} error. Error: {ex.Message}");
                }
                return Ok(new GetCardsResponse
                {
                    Cards = null,
                    ErrorCode = (int)OperationErrorCodes.DatabaseError,
                    ErrorMessage = $"Get cards for client with id {requestId} error. Database error."
                });
            }

            return Ok(new GetCardsResponse
            {
                Cards = null,
                ErrorCode = (int)OperationErrorCodes.ReadError,
                ErrorMessage = $"Can not get any card"
            });
        }

        [HttpPost("update")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult Update([FromBody] UpdateCardRequest request)
        {
            try
            {
                ValidationResult validationResult = _updateRequestValidator.Validate(request);
                if (validationResult.IsValid)
                {
                    // Add Automapper
                    var result = _repository.Update(_mapper.Map<Card>(request));
                    
                    if (result > 0)
                    {
                        return Ok(new UpdateCardResponse
                        {
                            Result = result,
                            ErrorCode = (int)OperationErrorCodes.OperationOk,
                            ErrorMessage = ""
                        });
                    }
                }
                else
                {
                    return Ok(new UpdateCardResponse
                    {
                        Result = 0,
                        ErrorCode = (int)OperationErrorCodes.UpdateError,
                        ErrorMessage = $"Update card parameters are not valid. {validationResult}"
                    });
                }
            }
            catch (Exception ex)
            {
                if (_configuration.Value.isLogEnabled)
                {
                    _logger.LogError($"Card {request.CardId} update error for client id {request.ClientId}. Error: {ex.Message}");
                }
                return Ok(new UpdateCardResponse
                {
                    Result = 0,
                    ErrorCode = (int)OperationErrorCodes.DatabaseError,
                    ErrorMessage = $"Card {request.CardId} update error for client id {request.ClientId}. Database error."
                });
            }

            return Ok(new UpdateCardResponse
            {
                Result = 0,
                ErrorCode = (int)OperationErrorCodes.UpdateError,
                ErrorMessage = "Card update error"
            });
        }

        [HttpDelete("delete")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult Delete([FromQuery] Guid requestId)
        {
            try
            {
                var result = _repository.Delete(requestId);

                if (result > 0)
                {
                    return Ok(new DeleteCardResponse
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
                    _logger.LogError($"Card {requestId} delete error. Error: {ex.Message}");
                }
                return Ok(new UpdateCardResponse
                {
                    Result = 0,
                    ErrorCode = (int)OperationErrorCodes.DatabaseError,
                    ErrorMessage = $"Card {requestId} delete error. Database error."
                });
            }

            return Ok(new DeleteCardResponse
            {
                Result = 0,
                ErrorCode = (int)OperationErrorCodes.DeleteError,
                ErrorMessage = $"Can not delete card by id={requestId}"
            });
        }

        #endregion
    }
}
