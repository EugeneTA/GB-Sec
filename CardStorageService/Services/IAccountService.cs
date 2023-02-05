using CardStorageService.Data;
using CardStorageService.Models.Requests.Authentication;
using CardStorageService.Models.Response.Authentication;

namespace CardStorageService.Services
{
    public interface IAccountService
    {
        public AccountCreateResponse CreateAccount(AccountCreateRequest accountCreateRequest);
    }
}
