using AutoMapper;
using CardStorageService.Data;
using CardStorageService.Models;
using CardStorageService.Models.Dto;
using CardStorageService.Models.Requests.Account;
using CardStorageService.Models.Requests.Authentication;
using CardStorageService.Models.Response.Account;
using CardStorageService.Models.Validators;
using EmployeeService.Models.Validators;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace CardStorageService.Services.Impl
{
    public class AccountService : IAccountService
    {
        public const string SecretKey = "jkrDkfyqnnf+!RsfgrWdlfkd";
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<AccountService> _logger;
        private readonly IMapper _mapper;

        public AccountService(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<AccountService> logger,
            IMapper mapper
            )
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _mapper = mapper;
        }

        public CreateAccountResponse CreateAccount(CreateAccountRequest accountCreateRequest)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            CardStorageServiceDbContext context = scope.ServiceProvider.GetRequiredService<CardStorageServiceDbContext>();

            Account account = FindAccountByLogin(context, accountCreateRequest.EMail);

            if (account != null)
            {
                _logger.LogError($"Account {accountCreateRequest.EMail} already exist");
                return new CreateAccountResponse
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
                _logger.LogError($"Defined empty account  password");
                return new CreateAccountResponse
                {
                    EMail = account.EMail,
                    FirstName = "",
                    LastName = "",
                    SecondName = "",
                    ErrorCode = (int)OperationErrorCodes.CreateError,
                    ErrorMessage = "Password empty"
                };
            }

            account = _mapper.Map<Account>(accountCreateRequest);
            (account.PasswordSalt, account.PasswordHash) = CreatePasswordHash(accountCreateRequest.Password);

            context.Accounts.Add(account);
            if (context.SaveChanges() > 0)
            {
                return new CreateAccountResponse
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
                _logger.LogError($"Error save new account to database ({account.EMail}, {account.FirstName}, {account.LastName}, {account.SecondName})");
                return new CreateAccountResponse
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

        public Account GetAccount(int id)
        {
            if (id == 0)
            {
                _logger.LogError("GetAccount id is 0");
                throw new InvalidDataException();
            }

            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            CardStorageServiceDbContext context = scope.ServiceProvider.GetRequiredService<CardStorageServiceDbContext>();

            if (context == null)
            {
                _logger.LogError($"GetAccount error. Can not get database context. Account id: {id}");
                throw new InvalidOperationException();
            }

            Account account = context.Accounts.FirstOrDefault(acc => acc.AccountId == id);

            if (account == null)
            {
                _logger.LogError($"GetAccount error. Can not find account with id: {id}");
                throw new InvalidOperationException();
            }

            return account;
        }

        public int UpdateAccount(UpdateAccountRequest data)
        {
            if (data == null)
            {
                _logger.LogError("Update account data empty");
                return 0;
            }

            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            CardStorageServiceDbContext context = scope.ServiceProvider.GetRequiredService<CardStorageServiceDbContext>();

            if (context == null)
            {
                _logger.LogError($"Update account error. Can not get database context.");
                return 0;
            }

            Account account = context.Accounts.FirstOrDefault(acc => acc.AccountId == data.AccountId);

            if (account == null)
            {
                _logger.LogError($"UpdateAccount error. Can not find account with id: {data.AccountId}");
                return 0;
            }

            account.EMail = data.EMail;
            account.Locked = data.Locked;
            account.FirstName = data.FirstName;
            account.SecondName = data.SecondName;
            account.LastName = data.LastName;

            context.Accounts.Update(account);
            return context.SaveChanges();

        }

        public int Delete(int id)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            CardStorageServiceDbContext context = scope.ServiceProvider.GetRequiredService<CardStorageServiceDbContext>();

            if (context == null)
            {
                _logger.LogError($"Delete account error. Can not get database context.");
                return 0;
            }

            Account account = context.Accounts.FirstOrDefault(acc => acc.AccountId == id);

            if (account == null)
            {
                _logger.LogError($"Delete account error. Can not find account with id: {id}");
                return 0;
            }

            context.Accounts.Remove(account);
            return context.SaveChanges();
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
