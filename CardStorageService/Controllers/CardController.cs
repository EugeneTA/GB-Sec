using Azure.Core;
using CardStorageService.Config;
using CardStorageService.Data;
using CardStorageService.Models;
using CardStorageService.Models.Dto;
using CardStorageService.Models.Requests.Card;
using CardStorageService.Models.Response.Card;
using CardStorageService.Services;
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
        private readonly IOptions<CardControllerConfig> _configuration;

        #endregion

        #region Constructors

        public CardController(
            ILogger<CardController> logger,
            ICardRepositoryService repositoryService,
            IOptions<CardControllerConfig> configuration)
        {
            _logger = logger;
            _repository = repositoryService;
            _configuration = configuration;
        }

        #endregion

        #region Public methods

        [HttpPost("create")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult Create([FromBody] CreateCardRequest request)
        {
            try
            {
                var cardId = _repository.Create(new Data.Card
                {
                    ClientId = request.ClientId,
                    Number = request.Number,
                    Name = request.Name,
                    CVV2 = request.CVV2,
                    ExpDate = request.ExpDate,
                });

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
                        Card = new CardDto()
                        {
                            ClientId = card.ClientId,
                            Number = card.Number,
                            Name = card.Name,
                            CVV2 = card.CVV2,
                            ExpDate = card.ExpDate.ToString("MM/yy")
                        },
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
                        Cards = cards.Select(card => new CardDto 
                        {
                            CardId = card.CardId,
                            ClientId = card.ClientId,
                            Number = card.Number,
                            Name = card.Name,
                            CVV2 = card.CVV2,
                            ExpDate = card.ExpDate.ToString("MM/yy")

                        }).ToList(),
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
                        Cards = cards.Select(card => new CardDto
                        {
                            CardId = card.CardId,
                            ClientId = card.ClientId,
                            Number = card.Number,
                            Name = card.Name,
                            CVV2 = card.CVV2,
                            ExpDate = card.ExpDate.ToString("MM/yy")

                        }).ToList(),
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
                var result = _repository.Update(new Data.Card
                {
                    CardId = request.CardId,
                    ClientId = request.ClientId,
                    Number = request.Number,
                    Name = request.Name,
                    CVV2 = request.CVV2,
                    ExpDate = request.ExpDate,
                });

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

        [HttpPost("delete")]
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
