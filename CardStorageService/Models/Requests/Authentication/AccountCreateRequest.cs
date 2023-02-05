using System.ComponentModel.DataAnnotations;

namespace CardStorageService.Models.Requests.Authentication
{
    public class AccountCreateRequest
    {
        public string? EMail { get; set; }
        public string? Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? SecondName { get; set; }
    }
}
