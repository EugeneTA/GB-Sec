using CardStorageService.Models.Dto;

namespace CardStorageService.Models.Response.Account
{
    public class GetAccountResponse: OperationResult
    {
        public AccountDto? Account { get; set; }

    }
}
