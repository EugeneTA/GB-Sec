namespace CardStorageService.Models.Response.Client
{
    public class GetClientResponse: OperationResult
    {
        public ClientDto? Client { get; set; }
    }
}
