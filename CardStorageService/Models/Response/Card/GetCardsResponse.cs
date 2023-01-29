namespace CardStorageService.Models.Response.Card
{
    public class GetCardsResponse: OperationResult
    {
        public IList<CardDto>? Cards { get; set; }
    }
}
