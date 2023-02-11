namespace CardStorageService.Models.Requests.Account
{
    public class UpdateAccountRequest
    {
        public int AccountId { get; set; }
        public string? EMail { get; set; }
        public string? Password { get; set; }
        public bool Locked { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? SecondName { get; set; }
    }
}
