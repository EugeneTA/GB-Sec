using CardStorageService.Data;
using CardStorageService.Models;
using CardStorageService.Models.Dto;
using CardStorageService.Models.Requests.Authentication;
using CardStorageService.Models.Response.Authentication;
using Microsoft.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace CardStorageService.Services.Impl
{
    public class AccountService : IAccountService
    {
        public const string SecretKey = "jkrDkfyqnnf+!RsfgrWdlfkd";
        private readonly IServiceScopeFactory _serviceScopeFactory;

        private readonly Dictionary<string, SessionDto> _sessions =
            new Dictionary<string, SessionDto>();

        public AccountService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public AccountCreateResponse CreateAccount(AccountCreateRequest accountCreateRequest)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            CardStorageServiceDbContext context = scope.ServiceProvider.GetRequiredService<CardStorageServiceDbContext>();

            Account account = FindAccountByLogin(context, accountCreateRequest.EMail);

            if (account != null)
            {
                return new AccountCreateResponse
                {
                    EMail = account.EMail,
                    FirstName = "",
                    LastName ="",
                    SecondName = "",
                    ErrorCode = (int)OperationErrorCodes.AcountAlreadyExist,
                    ErrorMessage = "Account already exist"
                };
            }

            if (string.IsNullOrEmpty(accountCreateRequest.Password))
            {
                return new AccountCreateResponse
                {
                    EMail = account.EMail,
                    FirstName = "",
                    LastName = "",
                    SecondName = "",
                    ErrorCode = (int)OperationErrorCodes.CreateError,
                    ErrorMessage = "Password empty"
                };
            }

            (string passwordSalt, string passwordHash) = CreatePasswordHash(accountCreateRequest.Password);

            account = new Account()
            {
                EMail = accountCreateRequest.EMail,
                FirstName = accountCreateRequest.FirstName,
                SecondName = accountCreateRequest.SecondName,
                LastName = accountCreateRequest.LastName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Locked = false
            };

            context.Accounts.Add(account);
            if (context.SaveChanges() > 0)
            {
                return new AccountCreateResponse
                {
                    EMail = account.EMail,
                    FirstName = account.FirstName,
                    LastName = account.LastName,
                    SecondName = account.SecondName,
                    ErrorCode = (int)OperationErrorCodes.OperationOk,
                    ErrorMessage = ""
                };
            }
            else 
            {
                return new AccountCreateResponse
                {
                    EMail = account.EMail,
                    FirstName = account.FirstName,
                    LastName = account.LastName,
                    SecondName = account.SecondName,
                    ErrorCode = (int)OperationErrorCodes.CreateError,
                    ErrorMessage = ""
                };
            }
        }

        private Account FindAccountByLogin(CardStorageServiceDbContext context, string login)
        {
            return context
                .Accounts
                .FirstOrDefault(account => account.EMail == login);
        }

        public static (string passwordSalt, string passwordHash) CreatePasswordHash(string password)
        {
            // generate random salt 
            byte[] buffer = new byte[16];
            RandomNumberGenerator secureRandom = new RNGCryptoServiceProvider();
            secureRandom.GetBytes(buffer);

            // create hash 
            string passwordSalt = Convert.ToBase64String(buffer);
            string passwordHash = GetPasswordHash(password, passwordSalt);

            // done
            return (passwordSalt, passwordHash);
        }

        public static string GetPasswordHash(string password, string passwordSalt)
        {
            // build password string
            password = $"{password}~{passwordSalt}~{SecretKey}";
            byte[] buffer = Encoding.UTF8.GetBytes(password);

            // compute hash 
            SHA512 sha512 = new SHA512Managed();
            byte[] passwordHash = sha512.ComputeHash(buffer);

            // done
            return Convert.ToBase64String(passwordHash);
        }
    }
}
