namespace CardStorageService.Models.Response.Account
{
    public class CreateAccountResponse : OperationResult
    {
        public string? EMail { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? SecondName { get; set; }
    }
}
