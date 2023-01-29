namespace CardStorageService.Models.Response.Client
{
    public class GetClientsResponse : OperationResult
    {
        public IList<ClientDto>? Clients { get; set; }
    }
}
