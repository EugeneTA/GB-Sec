using CardStorageService.Models.Dto;

namespace CardStorageService.Models.Response.Card
{
    public class GetCardResponse: OperationResult
    {
        public CardDto? Card { get; set; }
    }
}
