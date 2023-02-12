using CardStorageService.Data;
using CardStorageService.Models.Requests.Account;
using CardStorageService.Models.Response.Account;

namespace CardStorageService.Services
{
    public interface IAccountService
    {
        public CreateAccountResponse CreateAccount(CreateAccountRequest accountCreateRequest);
        public Account GetAccount(int id);
        public int UpdateAccount(UpdateAccountRequest data);
        public int Delete(int id);
    }
}
